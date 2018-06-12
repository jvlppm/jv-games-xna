namespace Jv.Games.Xna.Context
{
    using Microsoft.Xna.Framework;
    using System;
    using System.Runtime.ExceptionServices;

    public class OperationStatus : IOperationStatus
    {
        #region Attributes

        Action _waitingForCompletion;

        #endregion

        #region Properties

        public bool IsCompleted => State != OperationState.NotCompleted;

        public bool IsFaulted => State == OperationState.Faulted;

        public bool IsCanceled => State == OperationState.Canceled;

        public OperationState State { get; private set; }

        public Exception Error { get; private set; }

        #endregion

        #region Public Methods

        public void SetCompleted()
        {
            State = OperationState.Succeeded;
            NotifyCompletion();
        }

        public void Cancel()
        {
            State = OperationState.Canceled;
            NotifyCompletion();
        }

        public void SetError(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException(nameof(ex));

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

        #region Private Methods

        void NotifyCompletion()
        {
            _waitingForCompletion?.Invoke();
            _waitingForCompletion = null;
        }

        #endregion
    }

    public class OperationStatus<T> : OperationStatus, IOperationStatus<T>
    {
        T _result;

        public void SetResult(T result)
        {
            _result = result;
            SetCompleted();
        }

        public new T GetResult()
        {
            base.GetResult();
            return _result;
        }
    }
}
