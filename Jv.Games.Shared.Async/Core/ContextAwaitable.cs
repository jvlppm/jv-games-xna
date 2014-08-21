using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async
{
    public interface ISoftSynchronizationContext
    {
        void Post(Action action);
    }

    public class ContextOperationAwaiter : INotifyCompletion
    {
        public readonly IAsyncOperation Operation;
        public readonly ISoftSynchronizationContext Context;

        public bool IsCompleted { get { return Operation.IsCompleted; } }
        public bool IsFaulted { get { return Operation.IsFaulted; } }
        public bool IsCanceled { get { return Operation.IsCanceled; } }
        public Exception Error { get { return Operation.Error; } }

        public ContextOperationAwaiter(IAsyncOperation operation, ISoftSynchronizationContext context)
        {
            Operation = operation;
            Context = context;
        }

        #region INotifyCompletion implementation

        public void OnCompleted(Action continuation)
        {
            Operation.Completed += (s, e) => Context.Post(continuation);
        }

        public void GetResult() { Operation.GetResult(); }
        #endregion
    }

    public class ContextOperationAwaiter<T> : INotifyCompletion
    {
        public readonly IAsyncOperation<T> Operation;
        public readonly ISoftSynchronizationContext Context;

        public bool IsCompleted { get { return Operation.IsCompleted; } }
        public bool IsFaulted { get { return Operation.IsFaulted; } }
        public bool IsCanceled { get { return Operation.IsCanceled; } }
        public Exception Error { get { return Operation.Error; } }

        public ContextOperationAwaiter(IAsyncOperation<T> operation, ISoftSynchronizationContext context)
        {
            Operation = operation;
            Context = context;
        }

        #region INotifyCompletion implementation

        public void OnCompleted(Action continuation)
        {
            Operation.Completed += (s, e) => Context.Post(continuation);
        }

        public T GetResult() { return Operation.GetResult(); }
        #endregion
    }

    public class ContextTaskAwaiter : INotifyCompletion
    {
        readonly ISoftSynchronizationContext _context;
        readonly TaskAwaiter _taskAwaiter;

        public bool IsCompleted { get { return _taskAwaiter.IsCompleted; } }

        public ContextTaskAwaiter(Task task, ISoftSynchronizationContext context)
        {
            _taskAwaiter = task.GetAwaiter();
            _context = context;
        }

        public void GetResult() { _taskAwaiter.GetResult(); }

        public void OnCompleted(Action continuation)
        {
            _taskAwaiter.OnCompleted(() => _context.Post(continuation));
        }
    }

    public class ContextTaskAwaiter<T> : INotifyCompletion
    {
        readonly ISoftSynchronizationContext _context;
        readonly TaskAwaiter<T> _taskAwaiter;

        public bool IsCompleted { get { return _taskAwaiter.IsCompleted; } }

        public ContextTaskAwaiter(Task<T> task, ISoftSynchronizationContext context)
        {
            _taskAwaiter = task.GetAwaiter();
            _context = context;
        }

        public T GetResult() { return _taskAwaiter.GetResult(); }

        public void OnCompleted(Action continuation)
        {
            _taskAwaiter.OnCompleted(() => _context.Post(continuation));
        }
    }

    public class ContextTaskAwaitable
    {
        protected readonly Task Task;
        protected readonly ISoftSynchronizationContext Context;

        public static explicit operator Task(ContextTaskAwaitable cta)
        {
            return cta.Task;
        }

        public ContextTaskAwaitable(Task task, ISoftSynchronizationContext context)
        {
            Task = task;
            Context = context;
        }

        public ContextTaskAwaiter GetAwaiter()
        {
            return new ContextTaskAwaiter(Task, Context);
        }
    }

    public class ContextTaskAwaitable<T> : ContextTaskAwaitable
    {
        public static explicit operator Task<T>(ContextTaskAwaitable<T> cta)
        {
            return (Task<T>)cta.Task;
        }

        public ContextTaskAwaitable(Task<T> task, ISoftSynchronizationContext context)
            : base(task, context)
        {
        }

        public new ContextTaskAwaiter<T> GetAwaiter()
        {
            return new ContextTaskAwaiter<T>((Task<T>)Task, Context);
        }
    }

    public class ContextOperationAwaitable
    {
        protected readonly IAsyncOperation Operation;
        protected readonly ISoftSynchronizationContext Context;

        public ContextOperationAwaitable(IAsyncOperation operation, ISoftSynchronizationContext context)
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
            Operation.Completed += (s, e) =>
            {
                if (Operation.IsFaulted)
                    tcs.SetException(Operation.Error);
                else if (Operation.IsCanceled)
                    tcs.SetCanceled();
                else
                    tcs.SetResult(true);
            };
            return tcs.Task;
        }
    }

    public class ContextOperationAwaitable<T> : ContextOperationAwaitable
    {
        public ContextOperationAwaitable(IAsyncOperation<T> operation, ISoftSynchronizationContext context)
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
            Operation.Completed += (s, e) =>
            {
                if (Operation.IsFaulted)
                    tcs.SetException(Operation.Error);
                else if (Operation.IsCanceled)
                    tcs.SetCanceled();
                else
                    tcs.SetResult(((IAsyncOperation<T>)Operation).GetResult());
            };
            return tcs.Task;
        }
    }

    public static class Context
    {
        public static ISoftSynchronizationContext Current;

        public static ContextOperationAwaitable On(this IAsyncOperation operation, ISoftSynchronizationContext context)
        {
            return new ContextOperationAwaitable(operation, context);
        }

        public static ContextOperationAwaitable<T> On<T>(this IAsyncOperation<T> operation, ISoftSynchronizationContext context)
        {
            return new ContextOperationAwaitable<T>(operation, context);
        }

        public static ContextTaskAwaitable On(this Task task, ISoftSynchronizationContext context)
        {
            return new ContextTaskAwaitable(task, context);
        }

        public static ContextTaskAwaitable<T> On<T>(this Task<T> task, ISoftSynchronizationContext context)
        {
            return new ContextTaskAwaitable<T>(task, context);
        }

        public static ContextTaskAwaitable<Task<T>> WhenAny<T>(this ISoftSynchronizationContext context, params Task<T>[] tasks)
        {
#if NET_40
            return AsyncBridge.WhenAny(tasks).On(context);
#else
            return Task.WhenAny(tasks).On(context);
#endif
        }

        public static ContextTaskAwaitable<Task> WhenAny(this ISoftSynchronizationContext context, params Task[] tasks)
        {
#if NET_40
            return AsyncBridge.WhenAny(tasks).On(context);
#else
            return Task.WhenAny(tasks).On(context);
#endif
        }

        public static ContextTaskAwaitable<T[]> WhenAll<T>(this ISoftSynchronizationContext context, params Task<T>[] tasks)
        {
#if NET_40
            return AsyncBridge.WhenAll(tasks).On(context);
#else
            return Task.WhenAll(tasks).On(context);
#endif
        }

        public static ContextTaskAwaitable WhenAll(this ISoftSynchronizationContext context, params Task[] tasks)
        {
#if NET_40
            return AsyncBridge.WhenAll(tasks).On(context);
#else
            return Task.WhenAll(tasks).On(context);
#endif
        }
    }
}
