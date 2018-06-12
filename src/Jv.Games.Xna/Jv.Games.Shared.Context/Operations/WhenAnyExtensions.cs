namespace Jv.Games.Xna.Context
{
    using System;

    public static class WhenAnyExtensions
    {
        public static ContextOperation<ContextOperation> WhenAny(this IContext context, params ContextOperation[] operations)
        {
            if (operations.Length <= 0)
                throw new ArgumentException("No operations specified", nameof(operations));

            var operation = new OperationStatus<ContextOperation>();

            foreach (var op in operations)
                op.Operation.OnCompleted(() => operation.SetResult(op));

            return new ContextOperation<ContextOperation>(context, operation);
        }

        public static ContextOperation<ContextOperation<T>> WhenAny<T>(this IContext context, params ContextOperation<T>[] operations)
        {
            if (operations.Length <= 0)
                throw new ArgumentException("No operations specified", nameof(operations));

            var operation = new OperationStatus<ContextOperation<T>>();

            foreach (var op in operations)
                op.Operation.OnCompleted(() => operation.SetResult(op));

            return new ContextOperation<ContextOperation<T>>(context, operation);
        }
    }
}
