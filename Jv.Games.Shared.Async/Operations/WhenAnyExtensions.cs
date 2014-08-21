using System;

namespace Jv.Games.Xna.Async
{
    using System;

    public static class WhenAnyExtensions
    {
        public static ContextOperation<ContextOperation> WhenAny(this AsyncContext context, params ContextOperation[] operations)
        {
            if (operations.Length <= 0)
                throw new ArgumentException("No operations specified", "operations");

            var operation = new DummyOperation<ContextOperation>();

            foreach (var op in operations)
                op.GetAwaiter().OnCompleted(() => operation.SetResult(op));

            return context.Run(operation);
        }

        public static ContextOperation<ContextOperation<T>> WhenAny<T>(this AsyncContext context, params ContextOperation<T>[] operations)
        {
            if (operations.Length <= 0)
                throw new ArgumentException("No operations specified", "operations");

            var operation = new DummyOperation<ContextOperation<T>>();

            foreach (var op in operations)
                op.GetAwaiter().OnCompleted(() => operation.SetResult(op));

            return context.Run(operation);
        }
    }
}
