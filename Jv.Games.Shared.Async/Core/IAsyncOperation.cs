using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using System;
using System.Runtime.ExceptionServices;
using System.Collections.Generic;

namespace Jv.Games.Xna.Async
{
    public interface IOperation
    {
        /// <summary>
        /// Updates the operation.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        /// <returns>True if the operation can continue (its not complete).</returns>
        bool Continue(GameTime gameTime);
        void Cancel();
    }

    public interface IAsyncOperation : IOperation
    {
        bool IsCompleted { get; }
        event EventHandler Completed;
        void GetResult();
    }

    public interface IAsyncOperation<T> : IAsyncOperation
    {
        new T GetResult();
    }

    public abstract class AsyncOperation : IAsyncOperation
    {
        #region Attributes
        Exception _error;
        #endregion

        #region Properties
        public bool IsCompleted { get; protected set; }
        #endregion

        #region Events
        event EventHandler _waitingForCompletion;
        public event EventHandler Completed
        {
            add { _waitingForCompletion += value; }
            remove
            {
                if (IsCompleted)
                    value(this, EventArgs.Empty);
                else
                    _waitingForCompletion -= value;
            }
        }
        #endregion

        #region Protected Methods
        protected void SetCompleted()
        {
            IsCompleted = true;
            NotifyCompletion();
        }

        public virtual void Cancel()
        {
            SetError(new OperationCanceledException());
        }

        protected void SetError(Exception ex)
        {
            _error = ex;
            IsCompleted = true;
            NotifyCompletion();
        }
        #endregion

        #region IAsyncOperation implementation
        public void GetResult()
        {
            if (!IsCompleted)
                throw new InvalidOperationException();
            if (_error != null)
                ExceptionDispatchInfo.Capture(_error).Throw();
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
            _waitingForCompletion(this, EventArgs.Empty);
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
