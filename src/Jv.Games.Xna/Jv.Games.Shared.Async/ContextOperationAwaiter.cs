namespace Jv.Games.Xna.Async
{
    using System;
    using System.Runtime.CompilerServices;
    using Jv.Games.Xna.Context;

    public class ContextOperationAwaiter : INotifyCompletion
    {
        readonly IOperationStatus _operation;
        readonly IContext _context;

        public bool IsCompleted => _operation.IsCompleted;

        public bool IsFaulted => _operation.IsFaulted;

        public bool IsCanceled => _operation.IsCanceled;

        public Exception Error => _operation.Error;

        public ContextOperationAwaiter(IOperationStatus operation, IContext context)
        {
            _operation = operation;
            _context = context;
        }

        #region INotifyCompletion implementation

        public void GetResult() => _operation.GetResult();

        public void OnCompleted(Action continuation) => _operation.OnCompleted(() => _context.Post(continuation));

        #endregion
    }

    public class ContextOperationAwaiter<T> : INotifyCompletion
    {
        readonly IOperationStatus<T> _operation;
        readonly IContext _context;

        public bool IsCompleted => _operation.IsCompleted;

        public bool IsFaulted => _operation.IsFaulted;

        public bool IsCanceled => _operation.IsCanceled;

        public Exception Error => _operation.Error;

        public ContextOperationAwaiter(IOperationStatus<T> operation, IContext context)
        {
            _operation = operation;
            _context = context;
        }

        #region INotifyCompletion implementation

        public T GetResult() => _operation.GetResult();

        public void OnCompleted(Action continuation) => _operation.OnCompleted(() => _context.Post(continuation));

        #endregion
    }
}
