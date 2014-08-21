using System;

namespace Jv.Games.Xna.Async.Extensions
{
    public static class WhenAnyExtensions
    {
        public static ContextOperationAwaitable<ContextOperationAwaitable> WhenAny(this AsyncContext context, params ContextOperationAwaitable[] operations)
        {
            if (operations.Length <= 0)
                throw new ArgumentException("No operations specified", "operations");

            var operation = new DummyOperation<ContextOperationAwaitable>();

            foreach (var op in operations)
                op.GetAwaiter().OnCompleted(() => operation.SetResult(op));

            return context.Run(operation);
        }

        public static ContextOperationAwaitable<ContextOperationAwaitable<T>> WhenAny<T>(this AsyncContext context, params ContextOperationAwaitable<T>[] operations)
        {
            if (operations.Length <= 0)
                throw new ArgumentException("No operations specified", "operations");

            var operation = new DummyOperation<ContextOperationAwaitable<T>>();

            foreach (var op in operations)
                op.GetAwaiter().OnCompleted(() => operation.SetResult(op));

            return context.Run(operation);
        }
    }
}
