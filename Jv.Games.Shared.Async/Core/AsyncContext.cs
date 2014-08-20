using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jv.Games.Xna.Base;
using Microsoft.Xna.Framework;

namespace Jv.Games.Xna.Async
{
    public class AsyncContext : ISoftSynchronizationContext
    {
        #region Attributes
        readonly List<IAsyncOperation> _timers;
        readonly Queue<Action<GameTime>> _updateJobs;
        readonly Queue<Action> _jobs;
        object _jobsLock = new object();
        volatile bool haveJobs = false;
        #endregion

        #region Constructors
        public AsyncContext()
        {
            _timers = new List<IAsyncOperation>();
            _jobs = new Queue<Action>();
            _updateJobs = new Queue<Action<GameTime>>();
        }
        #endregion

        #region Public Methods
        public void Update(GameTime gameTime)
        {
            var oldContext = Context.Current;
            Context.Current = this;

            try
            {
                for (int i = _timers.Count - 1; i >= 0; i--)
                {
                    if (!_timers[i].Continue(gameTime))
                        _timers.RemoveAt(i);
                }

                if (haveJobs)
                {
                    lock (_jobsLock)
                    {
                        while (_jobs.Count > 0)
                            _jobs.Dequeue()();

                        while (_updateJobs.Count > 0)
                            _updateJobs.Dequeue()(gameTime);
                        haveJobs = false;
                    }
                }
            }
            finally
            {
                Context.Current = oldContext;
            }
        }

        public ContextOperationAwaitable<T> Run<T>(IAsyncOperation<T> operation)
        {
            _timers.Add(operation);
            return operation.On(this);
        }

        public ContextOperationAwaitable Run(IAsyncOperation operation)
        {
            _timers.Add(operation);
            return operation.On(this);
        }

        public void Post(Action<GameTime> action)
        {
            lock (_jobsLock)
            {
                _updateJobs.Enqueue(action);
                haveJobs = true;
            }
        }

        public void Post(Action action)
        {
            lock (_jobsLock)
            {
                _jobs.Enqueue(action);
                haveJobs = true;
            }
        }

        public void Send(Action action)
        {
            var oldContext = Context.Current;
            Context.Current = this;

            try
            {
                action();
            }
            finally
            {
                Context.Current = oldContext;
            }
        }

        public void Send(Action<AsyncContext> action)
        {
            var oldContext = Context.Current;
            Context.Current = this;

            try
            {
                action(this);
            }
            finally
            {
                Context.Current = oldContext;
            }
        }

        public void Send(Action<GameTime> action, GameTime gameTime)
        {
            var oldContext = Context.Current;
            Context.Current = this;

            try
            {
                action(gameTime);
            }
            finally
            {
                Context.Current = oldContext;
            }
        }
        #endregion
    }
}
