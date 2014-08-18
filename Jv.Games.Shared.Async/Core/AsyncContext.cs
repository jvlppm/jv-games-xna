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
        readonly ConcurrentQueue<Action<GameTime>> _updateJobs;
        readonly ConcurrentQueue<Action> _jobs;
        #endregion

        #region Constructors
        public AsyncContext()
        {
            _timers = new List<IAsyncOperation>();
            _jobs = new ConcurrentQueue<Action>();
            _updateJobs = new ConcurrentQueue<Action<GameTime>>();
        }
        #endregion

        #region Public Methods
        public void Update(GameTime gameTime)
        {
            using (Activate())
            {
                _timers.RemoveAll(t => !t.Continue(gameTime));

                Action job;
                while (_jobs.TryDequeue(out job))
                    job();

                Action<GameTime> updateJob;
                while (_updateJobs.TryDequeue(out updateJob))
                    updateJob(gameTime);
            }
        }

        public ContextTaskAwaitable<T> Run<T>(IAsyncOperation<T> operation)
        {
            _timers.Add(operation);
            return operation.Task.On(this);
        }

        public ContextTaskAwaitable Run(IAsyncOperation operation)
        {
            _timers.Add(operation);
            return operation.Task.On(this);
        }

        public void Post(Action<GameTime> action)
        {
            _updateJobs.Enqueue(action);
        }

        public void Post(Action action)
        {
            _jobs.Enqueue(action);
        }

        public void Send(Action action)
        {
            using (Activate())
                action();
        }

        public void Send(Action<AsyncContext> action)
        {
            using (Activate())
                action(this);
        }

        public void Send(Action<GameTime> action, GameTime gameTime)
        {
            using (Activate())
                action(gameTime);
        }
        #endregion

        #region Private Methods
        internal IDisposable Activate()
        {
            var oldContext = Context.Current;
            Context.Current = this;
            return Disposable.Create(() => Context.Current = oldContext);
        }
        #endregion
    }
}
