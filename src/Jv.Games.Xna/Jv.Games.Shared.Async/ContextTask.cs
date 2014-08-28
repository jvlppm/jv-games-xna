namespace Jv.Games.Xna.Async
{
    using System.Threading.Tasks;
    using Jv.Games.Xna.Context;

    public class ContextTask
    {
        protected readonly Task Task;
        protected readonly IContext Context;

        public static explicit operator Task(ContextTask cta)
        {
            return cta.Task;
        }

        public ContextTask(Task task, IContext context)
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

        public ContextTask(Task<T> task, IContext context)
            : base(task, context)
        {
        }

        public new ContextTaskAwaiter<T> GetAwaiter()
        {
            return new ContextTaskAwaiter<T>((Task<T>)Task, Context);
        }
    }
}

