namespace Jv.Games.Xna.Async
{
    using System;
    using System.Runtime.CompilerServices;
    using Jv.Games.Xna.Context;

    public class ContextOperationAwaiter : INotifyCompletion
    {
        readonly IGameOperation _operation;
        readonly IContext _context;

        public bool IsCompleted { get { return _operation.IsCompleted; } }

        public bool IsFaulted { get { return _operation.IsFaulted; } }

        public bool IsCanceled { get { return _operation.IsCanceled; } }

        public Exception Error { get { return _operation.Error; } }

        public ContextOperationAwaiter(IGameOperation operation, IContext context)
        {
            _operation = operation;
            _context = context;
        }

        #region INotifyCompletion implementation

        public void OnCompleted(Action continuation)
        {
            _operation.OnCompleted(() => _context.Post(continuation));
        }

        public void GetResult()
        {
            _operation.GetResult();
        }

        #endregion
    }

    public class ContextOperationAwaiter<T> : INotifyCompletion
    {
        readonly IGameOperation<T> _operation;
        readonly IContext _context;

        public bool IsCompleted { get { return _operation.IsCompleted; } }

        public bool IsFaulted { get { return _operation.IsFaulted; } }

        public bool IsCanceled { get { return _operation.IsCanceled; } }

        public Exception Error { get { return _operation.Error; } }

        public ContextOperationAwaiter(IGameOperation<T> operation, IContext context)
        {
            _operation = operation;
            _context = context;
        }

        #region INotifyCompletion implementation

        public void OnCompleted(Action continuation)
        {
            _operation.OnCompleted(() => _context.Post(continuation));
        }

        public T GetResult()
        {
            return _operation.GetResult();
        }

        #endregion
    }
}
