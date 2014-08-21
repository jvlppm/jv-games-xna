namespace Jv.Games.Xna.Async
{
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;

    public interface IAsyncContext
    {
        void Post(Action action);
    }

    public class AsyncContext : IAsyncContext
    {
        #region Attributes
        readonly List<IAsyncOperation> _runningOperations;
        readonly Queue<Action<GameTime>> _updateJobs;
        volatile bool haveJobs = false;
        #endregion

        #region Constructors
        public AsyncContext()
        {
            _runningOperations = new List<IAsyncOperation>();
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
                for (int i = _runningOperations.Count - 1; i >= 0; i--)
                {
                    if (!_runningOperations[i].Continue(gameTime))
                        _runningOperations.RemoveAt(i);
                }

                if (haveJobs)
                {
                    lock (_updateJobs)
                    {
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

        public ContextOperation<T> Run<T>(IAsyncOperation<T> operation)
        {
            _runningOperations.Add(operation);
            return operation.On(this);
        }

        public ContextOperation Run(IAsyncOperation operation)
        {
            _runningOperations.Add(operation);
            return operation.On(this);
        }

        public void Post(Action<GameTime> action)
        {
            lock (_updateJobs)
            {
                _updateJobs.Enqueue(action);
                haveJobs = true;
            }
        }

        public void Post(Action action)
        {
            lock (_updateJobs)
            {
                _updateJobs.Enqueue(gt => action());
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
