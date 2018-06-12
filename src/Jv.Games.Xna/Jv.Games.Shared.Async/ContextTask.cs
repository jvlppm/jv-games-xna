namespace Jv.Games.Xna.Async
{
    using System.Threading.Tasks;
    using Jv.Games.Xna.Context;

    public interface IContextTask
    {
        Task Task { get; }
        IContext Context { get; }
    }

    public interface IContextTask<T> : IContextTask
    {
        new Task<T> Task { get; }
    }

    public struct ContextTask : IContextTask
    {
        public Task Task { get; }
        public IContext Context { get; }

        public ContextTask(Task task, IContext context)
        {
            Task = task;
            Context = context;
        }

        public static explicit operator Task(ContextTask cta) => cta.Task;

        public ContextTaskAwaiter GetAwaiter() => new ContextTaskAwaiter(Task, Context);
    }

    public struct ContextTask<T> : IContextTask
    {
        public Task<T> Task { get; }
        public IContext Context { get; }

        Task IContextTask.Task => Task;

        public ContextTask(Task<T> task, IContext context)
        {
            Task = task;
            Context = context;
        }

        public static explicit operator Task<T>(ContextTask<T> cta) => cta.Task;

        public ContextTaskAwaiter<T> GetAwaiter() => new ContextTaskAwaiter<T>(Task, Context);
    }
}

