namespace Jv.Games.Xna.Context
{
    public interface IOperationContext
    {
        IOperationStatus Operation { get; }
        IContext Context { get; }
    }

    public interface IOperationContext<out T> : IOperationContext
    {
        new IOperationStatus<T> Operation { get; }
    }

    public struct ContextOperation : IOperationContext
    {
        public IOperationStatus Operation { get; }
        public IContext Context { get; }

        public ContextOperation(IContext context, IOperationStatus operation)
        {
            Context = context;
            Operation = operation;
        }
    }

    public struct ContextOperation<T> : IOperationContext<T>
    {
        public IOperationStatus<T> Operation { get; }
        public IContext Context { get; }

        IOperationStatus IOperationContext.Operation => Operation;
        IOperationStatus<T> IOperationContext<T>.Operation => Operation;

        public ContextOperation(IContext context, IOperationStatus<T> operation)
        {
            Context = context;
            Operation = operation;
        }
    }
}
