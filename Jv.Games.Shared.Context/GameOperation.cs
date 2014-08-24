namespace Jv.Games.Xna.Context
{
    using Microsoft.Xna.Framework;
    using System;
    using System.Runtime.ExceptionServices;

    public abstract class GameOperation : IGameOperation
    {
        #region Attributes

        Action _waitingForCompletion;

        #endregion

        #region Properties

        public bool IsCompleted { get { return State != OperationState.NotCompleted; } }

        public bool IsFaulted { get { return State == OperationState.Faulted; } }

        public bool IsCanceled { get { return State == OperationState.Canceled; } }

        public OperationState State { get; private set; }

        public Exception Error { get; protected set; }

        #endregion

        #region Protected Methods

        protected void SetCompleted()
        {
            State = OperationState.Succeeded;
            NotifyCompletion();
        }

        public virtual void Cancel()
        {
            State = OperationState.Canceled;
            NotifyCompletion();
        }

        protected void SetError(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");

            Error = ex;
            State = OperationState.Faulted;
            NotifyCompletion();
        }

        #endregion

        #region IAsyncOperation implementation

        public void GetResult()
        {
            if (State == OperationState.NotCompleted)
                throw new InvalidOperationException();
            if (State == OperationState.Faulted)
            {
                #if !NET_40
                ExceptionDispatchInfo.Capture(Error).Throw();
                #else
                throw Error;
                #endif
            }
            if (State == OperationState.Canceled)
                throw new OperationCanceledException();
        }

        public void OnCompleted(Action continuation)
        {
            if (IsCompleted)
                continuation();
            else
                _waitingForCompletion += continuation;
        }

        #endregion

        #region IOperation implementation

        public abstract bool Continue(GameTime gameTime);

        #endregion

        #region Private Methods

        void NotifyCompletion()
        {
            if (_waitingForCompletion == null)
                return;
            _waitingForCompletion();
            _waitingForCompletion = null;
        }

        #endregion
    }

    public abstract class GameOperation<T> : GameOperation, IGameOperation<T>
    {
        T _result;

        protected void SetResult(T result)
        {
            _result = result;
            SetCompleted();
        }

        new public T GetResult()
        {
            base.GetResult();
            return _result;
        }
    }
}
