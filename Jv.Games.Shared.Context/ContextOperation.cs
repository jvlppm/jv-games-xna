namespace Jv.Games.Xna.Context
{
    public class ContextOperation
    {
        public readonly IGameOperation Operation;
        public readonly IContext Context;

        public ContextOperation(IGameOperation operation, IContext context)
        {
            Operation = operation;
            Context = context;
        }
    }

    public class ContextOperation<T> : ContextOperation
    {
        public ContextOperation(IGameOperation<T> operation, IContext context)
            : base(operation, context)
        {
        }
    }
}

