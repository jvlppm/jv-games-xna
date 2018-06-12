namespace Jv.Games.Xna.Context
{
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;

    public class Context : IContext
    {
        #region Attributes

        readonly List<IGameOperation> _runningOperations;
        readonly Queue<Action<GameTime>> _updateJobs;
        volatile bool haveJobs;
        int _lastOperationIndex;

        #endregion

        #region Constructors

        public Context()
        {
            _lastOperationIndex = -1;
            _runningOperations = new List<IGameOperation>();
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

        public ContextOperation<T> Run<T>(IGameOperation<T> operation)
        {
            _runningOperations.Add(operation);
            _lastOperationIndex++;
            return new ContextOperation<T>(this, operation.Status);
        }

        public ContextOperation Run(IGameOperation operation)
        {
            _runningOperations.Add(operation);
            _lastOperationIndex++;
            return new ContextOperation(this, operation.Status);
        }

        public void Remove(IGameOperation operation)
        {
            _runningOperations.Remove(operation);
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

        #region Private Methods
        void ContinueOperations(GameTime gameTime)
        {
            for (int i = _lastOperationIndex; i >= 0; i--)
            {
                var current = _runningOperations[i];
                current.Continue(gameTime);
                if (current.Status.IsCompleted)
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
        #endregion
    }
}
