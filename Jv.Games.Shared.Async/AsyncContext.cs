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
        int _lastOperationIndex;
        #endregion

        #region Constructors
        public AsyncContext()
        {
            _lastOperationIndex = -1;
            _runningOperations = new List<IAsyncOperation>();
            _updateJobs = new Queue<Action<GameTime>>();
        }
        #endregion

        #region Public Methods
        public void Update(GameTime gameTime)
        {
            if (_lastOperationIndex >= 0)
                ContinueOperations(gameTime);

            if (haveJobs)
                RunPendingJobs(gameTime);
        }

        void ContinueOperations(GameTime gameTime)
        {
            for (int i = _lastOperationIndex; i >= 0; i--)
            {
                if (!_runningOperations[i].Continue(gameTime))
                {
                    _runningOperations.RemoveAt(i);
                    _lastOperationIndex--;
                }
            }
        }
        void RunPendingJobs(GameTime gameTime)
        {
            lock (_updateJobs)
            {
                while (_updateJobs.Count > 0)
                    _updateJobs.Dequeue()(gameTime);
                haveJobs = false;
            }
        }
        public ContextOperation<T> Run<T>(IAsyncOperation<T> operation)
        {
            _runningOperations.Add(operation);
            _lastOperationIndex++;
            return operation.On(this);
        }

        public ContextOperation Run(IAsyncOperation operation)
        {
            _runningOperations.Add(operation);
            _lastOperationIndex++;
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
        #endregion
    }
}
