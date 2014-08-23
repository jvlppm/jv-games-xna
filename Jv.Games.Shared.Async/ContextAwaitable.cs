namespace Jv.Games.Xna.Async
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    #region Context Operations
    public class ContextOperationAwaiter : INotifyCompletion
    {
        readonly IAsyncOperation _operation;
        readonly IAsyncContext _context;

        public bool IsCompleted { get { return _operation.IsCompleted; } }
        public bool IsFaulted { get { return _operation.IsFaulted; } }
        public bool IsCanceled { get { return _operation.IsCanceled; } }
        public Exception Error { get { return _operation.Error; } }

        public ContextOperationAwaiter(IAsyncOperation operation, IAsyncContext context)
        {
            _operation = operation;
            _context = context;
        }

        #region INotifyCompletion implementation

        public void OnCompleted(Action continuation)
        {
            _operation.OnCompleted(() => _context.Post(continuation));
        }

        public void GetResult() { _operation.GetResult(); }
        #endregion
    }

    public class ContextOperationAwaiter<T> : INotifyCompletion
    {
        readonly IAsyncOperation<T> _operation;
        readonly IAsyncContext _context;

        public bool IsCompleted { get { return _operation.IsCompleted; } }
        public bool IsFaulted { get { return _operation.IsFaulted; } }
        public bool IsCanceled { get { return _operation.IsCanceled; } }
        public Exception Error { get { return _operation.Error; } }

        public ContextOperationAwaiter(IAsyncOperation<T> operation, IAsyncContext context)
        {
            _operation = operation;
            _context = context;
        }

        #region INotifyCompletion implementation

        public void OnCompleted(Action continuation)
        {
            _operation.OnCompleted(() => _context.Post(continuation));
        }

        public T GetResult() { return _operation.GetResult(); }
        #endregion
    }

    public class ContextOperation
    {
        protected readonly IAsyncOperation Operation;
        protected readonly IAsyncContext Context;

        public ContextOperation(IAsyncOperation operation, IAsyncContext context)
        {
            Operation = operation;
            Context = context;
        }

        public ContextOperationAwaiter GetAwaiter()
        {
            return new ContextOperationAwaiter(Operation, Context);
        }

        public Task AsTask()
        {
            var tcs = new TaskCompletionSource<bool>();
            Operation.OnCompleted(() =>
            {
                if (Operation.IsFaulted)
                    tcs.SetException(Operation.Error);
                else if (Operation.IsCanceled)
                    tcs.SetCanceled();
                else
                    tcs.SetResult(true);
            });
            return tcs.Task;
        }
    }

    public class ContextOperation<T> : ContextOperation
    {
        public ContextOperation(IAsyncOperation<T> operation, IAsyncContext context)
            : base(operation, context)
        {
        }

        public new ContextOperationAwaiter<T> GetAwaiter()
        {
            return new ContextOperationAwaiter<T>((IAsyncOperation<T>)Operation, Context);
        }

        public new Task<T> AsTask()
        {
            var tcs = new TaskCompletionSource<T>();
            Operation.OnCompleted(() =>
            {
                if (Operation.IsFaulted)
                    tcs.SetException(Operation.Error);
                else if (Operation.IsCanceled)
                    tcs.SetCanceled();
                else
                    tcs.SetResult(((IAsyncOperation<T>)Operation).GetResult());
            });
            return tcs.Task;
        }
    }
    #endregion

    #region Context Tasks
    public class ContextTaskAwaiter : INotifyCompletion
    {
        readonly Task _task;
        readonly IAsyncContext _context;

        public bool IsCompleted { get { return _task.IsCompleted; } }

        public ContextTaskAwaiter(Task task, IAsyncContext context)
        {
            _task = task;
            _context = context;
        }

        public void GetResult() { _task.Wait(); }

        public void OnCompleted(Action continuation)
        {
            _task.ContinueWith(t => _context.Post(continuation), TaskContinuationOptions.ExecuteSynchronously);
        }
    }

    public class ContextTaskAwaiter<T> : INotifyCompletion
    {
        readonly IAsyncContext _context;
        readonly Task<T> _task;

        public bool IsCompleted { get { return _task.IsCompleted; } }

        public ContextTaskAwaiter(Task<T> task, IAsyncContext context)
        {
            _task = task;
            _context = context;
        }

        public T GetResult() { return _task.Result; }

        public void OnCompleted(Action continuation)
        {
            _task.ContinueWith(t => _context.Post(continuation), TaskContinuationOptions.ExecuteSynchronously);
        }
    }

    public class ContextTask
    {
        protected readonly Task Task;
        protected readonly IAsyncContext Context;

        public static explicit operator Task(ContextTask cta)
        {
            return cta.Task;
        }

        public ContextTask(Task task, IAsyncContext context)
        {
            Task = task;
            Context = context;
        }

        public ContextTaskAwaiter GetAwaiter()
        {
            return new ContextTaskAwaiter(Task, Context);
        }
    }

    public class ContextTask<T> : ContextTask
    {
        public static explicit operator Task<T>(ContextTask<T> cta)
        {
            return (Task<T>)cta.Task;
        }

        public ContextTask(Task<T> task, IAsyncContext context)
            : base(task, context)
        {
        }

        public new ContextTaskAwaiter<T> GetAwaiter()
        {
            return new ContextTaskAwaiter<T>((Task<T>)Task, Context);
        }
    }
    #endregion

    #region Extensions
    public static class ContextExtensions
    {
        public static ContextOperation On(this IAsyncOperation operation, IAsyncContext context)
        {
            return new ContextOperation(operation, context);
        }

        public static ContextOperation<T> On<T>(this IAsyncOperation<T> operation, IAsyncContext context)
        {
            return new ContextOperation<T>(operation, context);
        }

        public static ContextTask On(this Task task, IAsyncContext context)
        {
            return new ContextTask(task, context);
        }

        public static ContextTask<T> On<T>(this Task<T> task, IAsyncContext context)
        {
            return new ContextTask<T>(task, context);
        }

        public static ContextTask<Task<T>> WhenAny<T>(this IAsyncContext context, params Task<T>[] tasks)
        {
#if NET_40
            return AsyncBridge.WhenAny(tasks).On(context);
#else
            return Task.WhenAny(tasks).On(context);
#endif
        }

        public static ContextTask<Task> WhenAny(this IAsyncContext context, params Task[] tasks)
        {
#if NET_40
            return AsyncBridge.WhenAny(tasks).On(context);
#else
            return Task.WhenAny(tasks).On(context);
#endif
        }

        public static ContextTask<T[]> WhenAll<T>(this IAsyncContext context, params Task<T>[] tasks)
        {
#if NET_40
            return AsyncBridge.WhenAll(tasks).On(context);
#else
            return Task.WhenAll(tasks).On(context);
#endif
        }

        public static ContextTask WhenAll(this IAsyncContext context, params Task[] tasks)
        {
#if NET_40
            return AsyncBridge.WhenAll(tasks).On(context);
#else
            return Task.WhenAll(tasks).On(context);
#endif
        }
    }
#endregion
}
