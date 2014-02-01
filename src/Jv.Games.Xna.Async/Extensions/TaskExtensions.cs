using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async
{
    public static class TaskExtensions
    {
        public static Task<TResult> Select<T, TResult>(this Task<T> task, Func<T, TResult> selector)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");

            var tcs = new TaskCompletionSource<TResult>();

            task.ContinueWith(t =>
            {
                if (t.IsCanceled)
                    tcs.SetCanceled();
                else if (t.IsFaulted)
                    tcs.SetException(t.Exception.InnerException);
                else
                    tcs.SetResult(selector(t.Result));
            }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        public static Task<T> Select<T>(this Task task, T result)
        {
            var tcs = new TaskCompletionSource<T>();

            task.ContinueWith(t =>
            {
                if (t.IsCanceled)
                    tcs.SetCanceled();
                else if (t.IsFaulted)
                    tcs.SetException(t.Exception.InnerException);
                else
                    tcs.SetResult(result);
            }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        public static Task<T> Finally<T>(this Task<T> task, Action<Task<T>> action)
        {
            var tcs = new TaskCompletionSource<T>();

            task.ContinueWith(t =>
            {
                action(t);

                if (t.IsCanceled)
                    tcs.SetCanceled();
                else if (t.IsFaulted)
                    tcs.SetException(t.Exception.InnerException);
                else
                    tcs.SetResult(t.Result);

            }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        public static Task Finally(this Task task, Action<Task> action)
        {
            return Finally(task.Select(true), (Action<Task<bool>>)action);
        }
    }
}
