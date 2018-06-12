namespace Jv.Games.Xna.Context
{
    using System;
    using System.Collections.Generic;

    public static class WhenAllExtensions
    {
        public static ContextOperation WhenAll(this IContext context, params ContextOperation[] operations)
        {
            var operation = new OperationStatus();
            if (operations.Length <= 0)
            {
                operation.SetCompleted();
                return new ContextOperation(context, operation);
            }

            var remaining = new List<ContextOperation>(operations);

            List<Exception> errors = new List<Exception>();
            bool canceled = false;

            foreach (var op in operations)
            {
                op.Operation.OnCompleted(() =>
                {
                    lock (remaining)
                    {
                        remaining.Remove(op);
                        if (op.Operation.IsFaulted)
                            errors.Add(op.Operation.Error);
                        else if (op.Operation.IsCanceled)
                            canceled = true;

                        if (remaining.Count <= 0)
                        {
                            if (errors.Count > 0)
                                operation.SetError(new AggregateException(errors).Flatten());
                            else if (canceled)
                                operation.Cancel();
                            else
                                operation.SetCompleted();
                        }
                    }
                });
            }

            return new ContextOperation(context, operation);
        }

        public static ContextOperation<T[]> WhenAll<T>(this IContext context, params ContextOperation<T>[] operations)
        {
            var operation = new OperationStatus<T[]>();
            if (operations.Length <= 0)
            {
                operation.SetResult(new T[0]);
                return new ContextOperation<T[]>(context, operation);
            }

            var remaining = new List<ContextOperation<T>>(operations);

            List<Exception> errors = new List<Exception>();
            bool canceled = false;
            var results = new T[operations.Length];

            int opIndexCount = 0;
            foreach (var op in operations)
            {
                int curOpIndex = opIndexCount;

                op.Operation.OnCompleted(() =>
                {
                    lock (remaining)
                    {
                        remaining.Remove(op);
                        if (op.Operation.IsFaulted)
                            errors.Add(op.Operation.Error);
                        else if (op.Operation.IsCanceled)
                            canceled = true;
                        else
                            results[curOpIndex] = op.Operation.GetResult();

                        if (remaining.Count <= 0)
                        {
                            if (errors.Count > 0)
                                operation.SetError(new AggregateException(errors).Flatten());
                            else if (canceled)
                                operation.Cancel();
                            else
                                operation.SetResult(results);
                        }
                    }
                });

                opIndexCount++;
            }

            return new ContextOperation<T[]>(context, operation);
        }
    }
}
