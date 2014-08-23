﻿namespace Jv.Games.Xna.Async
{
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;

    public static class WhenAllExtensions
    {
        public static ContextOperation WhenAll(this AsyncContext context, params ContextOperation[] operations)
        {
            var operation = new DummyOperation();
            if (operations.Length <= 0)
            {
                operation.SetCompleted();
                return context.Run(operation);
            }

            var remaining = new List<ContextOperation>(operations);

            List<Exception> errors = new List<Exception>();
            bool canceled = false;

            foreach (var op in operations)
            {
                var awaiter = op.GetAwaiter();
                awaiter.OnCompleted(() =>
                {
                    lock (remaining)
                    {
                        remaining.Remove(op);
                        if (awaiter.IsFaulted)
                            errors.Add(awaiter.Error);
                        else if (awaiter.IsCanceled)
                            canceled = true;

                        if (remaining.Count <= 0)
                        {
                            if (errors.Count > 0)
                                operation.SetError(new AggregateException(errors));
                            else if (canceled)
                                operation.Cancel();
                            else
                                operation.SetCompleted();
                        }
                    }
                });
            }

            return context.Run(operation);
        }

        public static ContextOperation<T[]> WhenAll<T>(this AsyncContext context, params ContextOperation<T>[] operations)
        {
            var operation = new DummyOperation<T[]>();
            if (operations.Length <= 0)
            {
                operation.SetResult(new T[0]);
                return context.Run(operation);
            }

            var remaining = new List<ContextOperation>(operations);

            List<Exception> errors = new List<Exception>();
            bool canceled = false;
            var results = new T[operations.Length];

            int opIndexCount = 0;
            foreach (var op in operations)
            {
                int curOpIndex = opIndexCount;

                var awaiter = op.GetAwaiter();
                awaiter.OnCompleted(() =>
                {
                    lock (remaining)
                    {
                        remaining.Remove(op);
                        if (awaiter.IsFaulted)
                            errors.Add(awaiter.Error);
                        else if (awaiter.IsCanceled)
                            canceled = true;
                        else
                            results[curOpIndex] = awaiter.GetResult();

                        if (remaining.Count <= 0)
                        {
                            if (errors.Count > 0)
                                operation.SetError(new AggregateException(errors));
                            else if (canceled)
                                operation.Cancel();
                            else
                                operation.SetResult(results);
                        }
                    }
                });

                opIndexCount++;
            }

            return context.Run(operation);
        }
    }
}