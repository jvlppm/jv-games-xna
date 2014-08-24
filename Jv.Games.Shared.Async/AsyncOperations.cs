namespace Jv.Games.Xna.Async
{
    using Microsoft.Xna.Framework;
    using System;
    using System.Runtime.ExceptionServices;

    public abstract class AsyncOperation : IAsyncOperation
    {
        #region Attributes
        Action _waitingForCompletion;
        #endregion

        #region Properties
        public bool IsCompleted { get; protected set; }
        public bool IsFaulted { get; protected set; }
        public bool IsCanceled { get; protected set; }
        public Exception Error { get; protected set; }
        #endregion

        #region Protected Methods
        protected void SetCompleted()
        {
            IsCanceled = false;
            IsFaulted = false;
            IsCompleted = true;
            NotifyCompletion();
        }

        public virtual void Cancel()
        {
            IsCanceled = true;
            IsFaulted = false;
            IsCompleted = true;
            NotifyCompletion();
        }

        protected void SetError(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");

            Error = ex;
            IsCanceled = false;
            IsFaulted = true;
            IsCompleted = true;
            NotifyCompletion();
        }
        #endregion

        #region IAsyncOperation implementation
        public void GetResult()
        {
            if (!IsCompleted)
                throw new InvalidOperationException();
            if (IsFaulted)
            {
#if !NET_40
                ExceptionDispatchInfo.Capture(Error).Throw();
#else
                throw Error;
#endif
            }
            if (IsCanceled)
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

    public abstract class AsyncOperation<T> : AsyncOperation, IAsyncOperation<T>
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
