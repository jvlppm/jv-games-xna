using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
    public interface ISoftSynchronizationContext
    {
        void Post(Action action);
    }

    public class ContextTaskAwaiter : INotifyCompletion
    {
        private ISoftSynchronizationContext _context;
        private TaskAwaiter _taskAwaiter;

        public bool IsCompleted { get { return _taskAwaiter.IsCompleted; } }

        public ContextTaskAwaiter(Task task, ISoftSynchronizationContext context)
        {
            _taskAwaiter = task.GetAwaiter();
            _context = context;
        }

        public void GetResult() { _taskAwaiter.GetResult(); }

        public void OnCompleted(Action continuation)
        {
            _taskAwaiter.OnCompleted(delegate
            {
                _context.Post(continuation);
            });
        }
    }

    public class ContextTaskAwaiter<T> : INotifyCompletion
    {
        private ISoftSynchronizationContext _context;
        private TaskAwaiter<T> _taskAwaiter;

        public bool IsCompleted { get { return _taskAwaiter.IsCompleted; } }

        public ContextTaskAwaiter(Task<T> task, ISoftSynchronizationContext context)
        {
            _taskAwaiter = task.GetAwaiter();
            _context = context;
        }

        public T GetResult() { return _taskAwaiter.GetResult(); }

        public void OnCompleted(Action continuation)
        {
            _taskAwaiter.OnCompleted(delegate
            {
                _context.Post(continuation);
            });
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

    public static class TaskEx
    {
        public static ISoftSynchronizationContext CurrentContext;

        public static ContextTaskAwaitable On(this Task task, ISoftSynchronizationContext context)
        {
            return new ContextTaskAwaitable(task, context);
        }

        public static ContextTaskAwaitable<T> On<T>(this Task<T> task, ISoftSynchronizationContext context)
        {
            return new ContextTaskAwaitable<T>(task, context);
        }

        public static ContextTaskAwaitable<Task> WhenAny(params Task[] tasks)
        {
            return WhenAnyOn(CurrentContext, tasks);
        }

        public static ContextTaskAwaitable<Task<T>> WhenAny<T>(params Task<T>[] tasks)
        {
            return WhenAnyOn(CurrentContext, tasks);
        }

        public static ContextTaskAwaitable WhenAll(params Task[] tasks)
        {
            return WhenAllOn(CurrentContext, tasks);
        }

        public static ContextTaskAwaitable<T[]> WhenAll<T>(params Task<T>[] tasks)
        {
            return WhenAllOn(CurrentContext, tasks);
        }

        public static ContextTaskAwaitable Delay(TimeSpan delay)
        {
            return DelayOn(CurrentContext, delay);
        }

        public static ContextTaskAwaitable<Task<T>> WhenAnyOn<T>(ISoftSynchronizationContext context, params Task<T>[] tasks)
        {
#if ASYNC_BRIDGE
            return AsyncBridge.WhenAny(tasks).On(context);
#else
            return Task.WhenAny(tasks).On(context);
#endif
        }

        public static ContextTaskAwaitable<Task> WhenAnyOn(ISoftSynchronizationContext context, params Task[] tasks)
        {
#if ASYNC_BRIDGE
            return AsyncBridge.WhenAny(tasks).On(context);
#else
            return Task.WhenAny(tasks).On(context);
#endif
        }

        public static ContextTaskAwaitable<T[]> WhenAllOn<T>(ISoftSynchronizationContext context, params Task<T>[] tasks)
        {
#if ASYNC_BRIDGE
            return AsyncBridge.WhenAll(tasks).On(context);
#else
            return Task.WhenAll(tasks).On(context);
#endif
        }

        public static ContextTaskAwaitable WhenAllOn(ISoftSynchronizationContext context, params Task[] tasks)
        {
#if ASYNC_BRIDGE
            return AsyncBridge.WhenAll(tasks).On(context);
#else
            return Task.WhenAll(tasks).On(context);
#endif
        }

        public static ContextTaskAwaitable DelayOn(ISoftSynchronizationContext context, TimeSpan delay)
        {
#if ASYNC_BRIDGE
            return AsyncBridge.Delay(delay).On(context);
#else
            return Task.Delay(delay).On(context);
#endif
        }
    }
}
