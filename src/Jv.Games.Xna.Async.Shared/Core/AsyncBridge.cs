// Copyright (c) 2012 Daniel Grunwald
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.


// OpenTK breaks under windows when the current synchronization context is changed,
// even if the code is still running under the Main Thread, so, to avoid this issue,
// ISoftSynchronizationContext was created, and a custom AsyncBridge is embedded into
// this project.

// Its modifications are:
//     Custom TaskAwaiter, so it will enqueue continuation under TaskAwaiter.CurrentContext;
//     Synchronous continuations, so no game frames are lost when concluding tasks.

// As a limitation, games that want to use await need to target .Net Framework 4.0,
// this will ensure that the extension method GetAwaiter() is called, instead of the
// built in one.

// This makes games compatible with .Net Framework 4.0 under Windows or Mono (2.10+).

#if ASYNC_BRIDGE

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading.Tasks;

// This is a minimal implementation of the stuff needed by the C# 5 Beta
// compiler for 'async'/'await' feature.
// Simply add this file to a .NET 4.0 project, and 'async'/'await' will work
// in your code without requiring your users to install .NET 4.5.
// However, you still need the C# 5 compiler for compiling your code.
// Caveats:
// - Stack trace is lost when rethrowing exceptions
// - Unhandled Exceptions in 'async void' methods are re-thrown immediately
// and not posted to the synchronization context
// - Flowing ExecutionContext is not supported
// --> do not use this in security critical code (APTCA)
// - The code is not optimized. Tasks are always created, repeated boxing may occur.

using System.Collections.Generic;
using System.Linq;
using Jv.Games.Xna.Async;

namespace System.Threading.Tasks
{
    public static class AsyncBridge
    {
        public static TaskAwaiter GetAwaiter(this Task task)
        {
            return new TaskAwaiter(task);
        }

        public static TaskAwaiter<T> GetAwaiter<T>(this Task<T> task)
        {
            return new TaskAwaiter<T>(task);
        }

        /// <summary>
        /// An already completed task.
        /// </summary>
        private static Task s_preCompletedTask = FromResult(false);
        /// <summary>
        /// An already canceled task.
        /// </summary>
        private static Task s_preCanceledTask = ((Func<Task>)(() =>
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.TrySetCanceled();
            return (Task)tcs.Task;
        }))();
        private const string ArgumentOutOfRange_TimeoutNonNegativeOrMinusOne = "The timeout must be non-negative or -1, and it must be less than or equal to Int32.MaxValue.";

        static AsyncBridge()
        {
        }

        /// <summary>
        /// Creates a task that runs the specified action.
        /// </summary>
        /// <param name="action">The action to execute asynchronously.</param>
        /// <returns>
        /// A task that represents the completion of the action.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="action"/> argument is null.</exception>
        public static Task Run(Action action)
        {
            return Run(action, CancellationToken.None);
        }

        /// <summary>
        /// Creates a task that runs the specified action.
        /// </summary>
        /// <param name="action">The action to execute.</param><param name="cancellationToken">The CancellationToken to use to request cancellation of this task.</param>
        /// <returns>
        /// A task that represents the completion of the action.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="action"/> argument is null.</exception>
        public static Task Run(Action action, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(action, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
        }

        /// <summary>
        /// Creates a task that runs the specified function.
        /// </summary>
        /// <param name="function">The function to execute asynchronously.</param>
        /// <returns>
        /// A task that represents the completion of the action.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function"/> argument is null.</exception>
        public static Task<TResult> Run<TResult>(Func<TResult> function)
        {
            return Run(function, CancellationToken.None);
        }

        /// <summary>
        /// Creates a task that runs the specified function.
        /// </summary>
        /// <param name="function">The action to execute.</param><param name="cancellationToken">The CancellationToken to use to cancel the task.</param>
        /// <returns>
        /// A task that represents the completion of the action.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function"/> argument is null.</exception>
        public static Task<TResult> Run<TResult>(Func<TResult> function, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(function, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
        }

        /// <summary>
        /// Creates a task that runs the specified function.
        /// </summary>
        /// <param name="function">The action to execute asynchronously.</param>
        /// <returns>
        /// A task that represents the completion of the action.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function"/> argument is null.</exception>
        public static Task Run(Func<Task> function)
        {
            return Run(function, CancellationToken.None);
        }

        /// <summary>
        /// Creates a task that runs the specified function.
        /// </summary>
        /// <param name="function">The function to execute.</param><param name="cancellationToken">The CancellationToken to use to request cancellation of this task.</param>
        /// <returns>
        /// A task that represents the completion of the function.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function"/> argument is null.</exception>
        public static Task Run(Func<Task> function, CancellationToken cancellationToken)
        {
            return Run<Task>(function, cancellationToken).Unwrap();
        }

        /// <summary>
        /// Creates a task that runs the specified function.
        /// </summary>
        /// <param name="function">The function to execute asynchronously.</param>
        /// <returns>
        /// A task that represents the completion of the action.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function"/> argument is null.</exception>
        public static Task<TResult> Run<TResult>(Func<Task<TResult>> function)
        {
            return Run(function, CancellationToken.None);
        }

        /// <summary>
        /// Creates a task that runs the specified function.
        /// </summary>
        /// <param name="function">The action to execute.</param><param name="cancellationToken">The CancellationToken to use to cancel the task.</param>
        /// <returns>
        /// A task that represents the completion of the action.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function"/> argument is null.</exception>
        public static Task<TResult> Run<TResult>(Func<Task<TResult>> function, CancellationToken cancellationToken)
        {
            return Run<Task<TResult>>(function, cancellationToken).Unwrap();
        }

        /// <summary>
        /// Starts a Task that will complete after the specified due time.
        /// </summary>
        /// <param name="dueTime">The delay in milliseconds before the returned task completes.</param>
        /// <returns>
        /// The timed Task.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="dueTime"/> argument must be non-negative or -1 and less than or equal to Int32.MaxValue.
        ///             </exception>
        public static Task Delay(int dueTime)
        {
            return Delay(dueTime, CancellationToken.None);
        }

        /// <summary>
        /// Starts a Task that will complete after the specified due time.
        /// </summary>
        /// <param name="dueTime">The delay before the returned task completes.</param>
        /// <returns>
        /// The timed Task.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="dueTime"/> argument must be non-negative or -1 and less than or equal to Int32.MaxValue.
        ///             </exception>
        public static Task Delay(TimeSpan dueTime)
        {
            return Delay(dueTime, CancellationToken.None);
        }

        /// <summary>
        /// Starts a Task that will complete after the specified due time.
        /// </summary>
        /// <param name="dueTime">The delay before the returned task completes.</param><param name="cancellationToken">A CancellationToken that may be used to cancel the task before the due time occurs.</param>
        /// <returns>
        /// The timed Task.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="dueTime"/> argument must be non-negative or -1 and less than or equal to Int32.MaxValue.
        ///             </exception>
        public static Task Delay(TimeSpan dueTime, CancellationToken cancellationToken)
        {
            var timeoutMs = (long)dueTime.TotalMilliseconds;
            if (timeoutMs < Timeout.Infinite || timeoutMs > int.MaxValue)
                throw new ArgumentOutOfRangeException("dueTime", ArgumentOutOfRange_TimeoutNonNegativeOrMinusOne);

            return Delay((int)timeoutMs, cancellationToken);
        }

        /// <summary>
        /// Starts a Task that will complete after the specified due time.
        /// </summary>
        /// <param name="dueTime">The delay in milliseconds before the returned task completes.</param><param name="cancellationToken">A CancellationToken that may be used to cancel the task before the due time occurs.</param>
        /// <returns>
        /// The timed Task.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="dueTime"/> argument must be non-negative or -1 and less than or equal to Int32.MaxValue.
        ///             </exception>
        public static Task Delay(int dueTime, CancellationToken cancellationToken)
        {
            if (dueTime < -1)
                throw new ArgumentOutOfRangeException("dueTime", ArgumentOutOfRange_TimeoutNonNegativeOrMinusOne);
            if (cancellationToken.IsCancellationRequested)
                return s_preCanceledTask;
            if (dueTime == 0)
                return s_preCompletedTask;
            var tcs = new TaskCompletionSource<bool>();
            var ctr = new CancellationTokenRegistration();

            Timer timer = null;
            timer = new Timer(_ =>
            {
                ctr.Dispose();
                timer.Dispose();
                tcs.TrySetResult(true);
            }, null, Timeout.Infinite, Timeout.Infinite);

            if (cancellationToken.CanBeCanceled)
            {
                ctr = cancellationToken.Register(() =>
                {
                    timer.Dispose();
                    tcs.TrySetCanceled();
                });
            }

            timer.Change(dueTime, Timeout.Infinite);
            return tcs.Task;
        }

        /// <summary>
        /// Creates a Task that will complete only when all of the provided collection of Tasks has completed.
        /// </summary>
        /// <param name="tasks">The Tasks to monitor for completion.</param>
        /// <returns>
        /// A Task that represents the completion of all of the provided tasks.
        /// </returns>
        /// 
        /// <remarks>
        /// If any of the provided Tasks faults, the returned Task will also fault, and its Exception will contain information
        ///             about all of the faulted tasks.  If no Tasks fault but one or more Tasks is canceled, the returned
        ///             Task will also be canceled.
        /// 
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks"/> argument is null.</exception><exception cref="T:System.ArgumentException">The <paramref name="tasks"/> argument contains a null reference.</exception>
        public static Task WhenAll(params Task[] tasks)
        {
            return WhenAll((IEnumerable<Task>)tasks);
        }

        /// <summary>
        /// Creates a Task that will complete only when all of the provided collection of Tasks has completed.
        /// </summary>
        /// <param name="tasks">The Tasks to monitor for completion.</param>
        /// <returns>
        /// A Task that represents the completion of all of the provided tasks.
        /// </returns>
        /// 
        /// <remarks>
        /// If any of the provided Tasks faults, the returned Task will also fault, and its Exception will contain information
        ///             about all of the faulted tasks.  If no Tasks fault but one or more Tasks is canceled, the returned
        ///             Task will also be canceled.
        /// 
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks"/> argument is null.</exception><exception cref="T:System.ArgumentException">The <paramref name="tasks"/> argument contains a null reference.</exception>
        public static Task<TResult[]> WhenAll<TResult>(params Task<TResult>[] tasks)
        {
            return WhenAll((IEnumerable<Task<TResult>>)tasks);
        }

        /// <summary>
        /// Creates a Task that will complete only when all of the provided collection of Tasks has completed.
        /// </summary>
        /// <param name="tasks">The Tasks to monitor for completion.</param>
        /// <returns>
        /// A Task that represents the completion of all of the provided tasks.
        /// </returns>
        /// 
        /// <remarks>
        /// If any of the provided Tasks faults, the returned Task will also fault, and its Exception will contain information
        ///             about all of the faulted tasks.  If no Tasks fault but one or more Tasks is canceled, the returned
        ///             Task will also be canceled.
        /// 
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks"/> argument is null.</exception><exception cref="T:System.ArgumentException">The <paramref name="tasks"/> argument contains a null reference.</exception>
        public static Task WhenAll(IEnumerable<Task> tasks)
        {
            return WhenAllCore(tasks, (Action<Task[], TaskCompletionSource<object>>)((completedTasks, tcs) => tcs.TrySetResult(null)));
        }

        /// <summary>
        /// Creates a Task that will complete only when all of the provided collection of Tasks has completed.
        /// </summary>
        /// <param name="tasks">The Tasks to monitor for completion.</param>
        /// <returns>
        /// A Task that represents the completion of all of the provided tasks.
        /// </returns>
        /// 
        /// <remarks>
        /// If any of the provided Tasks faults, the returned Task will also fault, and its Exception will contain information
        ///             about all of the faulted tasks.  If no Tasks fault but one or more Tasks is canceled, the returned
        ///             Task will also be canceled.
        /// 
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks"/> argument is null.</exception><exception cref="T:System.ArgumentException">The <paramref name="tasks"/> argument contains a null reference.</exception>
        public static Task<TResult[]> WhenAll<TResult>(IEnumerable<Task<TResult>> tasks)
        {
            return WhenAllCore<TResult[]>(tasks.Cast<Task>(), (completedTasks, tcs) =>
                                          tcs.TrySetResult(completedTasks
                             .Cast<Task<TResult>>()
                             .Select(t => t.Result)
                             .ToArray()));
        }

        /// <summary>
        /// Creates a Task that will complete only when all of the provided collection of Tasks has completed.
        /// </summary>
        /// <param name="tasks">The Tasks to monitor for completion.</param><param name="setResultAction">A callback invoked when all of the tasks complete successfully in the RanToCompletion state.
        ///             This callback is responsible for storing the results into the TaskCompletionSource.
        ///             </param>
        /// <returns>
        /// A Task that represents the completion of all of the provided tasks.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks"/> argument is null.</exception><exception cref="T:System.ArgumentException">The <paramref name="tasks"/> argument contains a null reference.</exception>
        private static Task<TResult> WhenAllCore<TResult>(IEnumerable<Task> tasks, Action<Task[], TaskCompletionSource<TResult>> setResultAction)
        {
            if (tasks == null)
                throw new ArgumentNullException("tasks");
            var tcs = new TaskCompletionSource<TResult>();
            var taskArray = tasks as Task[] ?? tasks.ToArray();
            if (taskArray.Length == 0)
                setResultAction(taskArray, tcs);
            else
                Task.Factory.ContinueWhenAll(taskArray, completedTasks =>
                {
                    List<Exception> exceptions = null;
                    var canceled = false;
                    foreach (var task in completedTasks)
                    {
                        if (task.IsFaulted)
                            AddPotentiallyUnwrappedExceptions(ref exceptions, task.Exception);
                        else if (task.IsCanceled)
                            canceled = true;
                    }
                    if (exceptions != null && exceptions.Count > 0)
                        tcs.TrySetException(exceptions);
                    else if (canceled)
                        tcs.TrySetCanceled();
                    else
                        setResultAction(completedTasks, tcs);
                }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Current);
            return tcs.Task;
        }

        /// <summary>
        /// Creates a Task that will complete when any of the tasks in the provided collection completes.
        /// </summary>
        /// <param name="tasks">The Tasks to be monitored.</param>
        /// <returns>
        /// A Task that represents the completion of any of the provided Tasks.  The completed Task is this Task's result.
        /// 
        /// </returns>
        /// 
        /// <remarks>
        /// Any Tasks that fault will need to have their exceptions observed elsewhere.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks"/> argument is null.</exception><exception cref="T:System.ArgumentException">The <paramref name="tasks"/> argument contains a null reference.</exception>
        public static Task<Task> WhenAny(params Task[] tasks)
        {
            return WhenAny((IEnumerable<Task>)tasks);
        }

        /// <summary>
        /// Creates a Task that will complete when any of the tasks in the provided collection completes.
        /// </summary>
        /// <param name="tasks">The Tasks to be monitored.</param>
        /// <returns>
        /// A Task that represents the completion of any of the provided Tasks.  The completed Task is this Task's result.
        /// 
        /// </returns>
        /// 
        /// <remarks>
        /// Any Tasks that fault will need to have their exceptions observed elsewhere.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks"/> argument is null.</exception><exception cref="T:System.ArgumentException">The <paramref name="tasks"/> argument contains a null reference.</exception>
        public static Task<Task> WhenAny(IEnumerable<Task> tasks)
        {
            if (tasks == null)
                throw new ArgumentNullException("tasks");
            var tcs = new TaskCompletionSource<Task>();
            Task.Factory.ContinueWhenAny<bool>(tasks as Task[] ?? tasks.ToArray(), tcs.TrySetResult, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Current);
            return tcs.Task;
        }
        /*/// <summary>
        /// Creates a Task that will complete when any of the tasks in the provided collection completes.
        /// </summary>
        /// <param name="tasks">The Tasks to be monitored.</param>
        /// <returns>
        /// A Task that represents the completion of any of the provided Tasks.  The completed Task is this Task's result.
        /// 
        /// </returns>
        /// 
        /// <remarks>
        /// Any Tasks that fault will need to have their exceptions observed elsewhere.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks"/> argument is null.</exception><exception cref="T:System.ArgumentException">The <paramref name="tasks"/> argument contains a null reference.</exception>
        public static Task<Task<TResult>> WhenAny<TResult>(params Task<TResult>[] tasks)
        {
            return WhenAny((IEnumerable<Task<TResult>>)tasks);
        }

        /// <summary>
        /// Creates a Task that will complete when any of the tasks in the provided collection completes.
        /// </summary>
        /// <param name="tasks">The Tasks to be monitored.</param>
        /// <returns>
        /// A Task that represents the completion of any of the provided Tasks.  The completed Task is this Task's result.
        /// 
        /// </returns>
        /// 
        /// <remarks>
        /// Any Tasks that fault will need to have their exceptions observed elsewhere.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks"/> argument is null.</exception><exception cref="T:System.ArgumentException">The <paramref name="tasks"/> argument contains a null reference.</exception>
        public static Task<Task<TResult>> WhenAny<TResult>(IEnumerable<Task<TResult>> tasks)
        {
            if (tasks == null)
                throw new ArgumentNullException("tasks");
            var tcs = new TaskCompletionSource<Task<TResult>>();
            Task.Factory.ContinueWhenAny<TResult, bool>(tasks as Task<TResult>[] ?? tasks.ToArray(), tcs.TrySetResult, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Current);
            return tcs.Task;
        }*/
        public static Task<Task<T>> WhenAny<T>(params Task<T>[] tasks)
        {
            var tcs = new TaskCompletionSource<Task<T>>();
            Action<Task<T>> complete = t =>
            {
                tcs.TrySetResult(t);
            };

            foreach (var task in tasks)
                task.ContinueWith(complete, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        /// <summary>
        /// Creates an already completed <see cref="T:System.Threading.Tasks.Task`1"/> from the specified result.
        /// </summary>
        /// <param name="result">The result from which to create the completed task.</param>
        /// <returns>
        /// The completed task.
        /// </returns>
        public static Task<TResult> FromResult<TResult>(TResult result)
        {
            var completionSource = new TaskCompletionSource<TResult>(result);
            completionSource.TrySetResult(result);
            return completionSource.Task;
        }
        /*/// <summary>
        /// Creates an awaitable that asynchronously yields back to the current context when awaited.
        /// </summary>
        /// 
        /// <returns>
        /// A context that, when awaited, will asynchronously transition back into the current context.
        ///             If SynchronizationContext.Current is non-null, that is treated as the current context.
        ///             Otherwise, TaskScheduler.Current is treated as the current context.
        /// 
        /// </returns>
        public static YieldAwaitable Yield()
        {
            return new YieldAwaitable();
        }*/
        /// <summary>
        /// Adds the target exception to the list, initializing the list if it's null.
        /// </summary>
        /// <param name="targetList">The list to which to add the exception and initialize if the list is null.</param><param name="exception">The exception to add, and unwrap if it's an aggregate.</param>
        private static void AddPotentiallyUnwrappedExceptions(ref List<Exception> targetList, Exception exception)
        {
            var aggregateException = exception as AggregateException;
            if (targetList == null)
                targetList = new List<Exception>();
            if (aggregateException != null)
                targetList.Add(aggregateException.InnerExceptions.Count == 1 ? exception.InnerException : exception);
            else
                targetList.Add(exception);
        }
    }

    public class TaskAwaiter : TaskAwaiter<bool>
    {
        public TaskAwaiter(Task task)
            : base(task)
        {
        }

        new public void GetResult()
        {
            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                throw ex.InnerExceptions[0];
            }
        }
    }

    public class TaskAwaiter<T> : INotifyCompletion
    {
        protected readonly Task task;
        readonly ISoftSynchronizationContext capturedContext;
        readonly TaskScheduler capturedScheduler;

        public TaskAwaiter(Task<T> task)
            : this((Task)task)
        {
        }

        internal TaskAwaiter(Task task)
        {
            this.task = task;
            this.capturedContext = TaskEx.CurrentContext;
            this.capturedScheduler = TaskScheduler.Current;

            if (capturedContext == null)
                throw new InvalidOperationException("TaskAwaiter.CurrentContext must be set before await.");
        }

        public bool IsCompleted
        {
            get { return task.IsCompleted; }
        }

        public void OnCompleted(Action continuation)
        {
            this.task.ContinueWith(delegate
            {
                capturedContext.Post(continuation);
            }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, this.capturedScheduler);
        }

        public T GetResult()
        {
            try
            {
                return ((Task<T>)task).Result;
            }
            catch (AggregateException ex)
            {
                throw ex.InnerExceptions[0];
            }
        }
    }
}

namespace System.Runtime.CompilerServices
{
    public interface INotifyCompletion
    {
        void OnCompleted(Action continuation);
    }

    public interface ICriticalNotifyCompletion : INotifyCompletion
    {
        [SecurityCritical]
        void UnsafeOnCompleted(Action continuation);
    }

    public interface IAsyncStateMachine
    {
        void MoveNext();

        void SetStateMachine(IAsyncStateMachine stateMachine);
    }

    public struct AsyncVoidMethodBuilder
    {
        public static AsyncVoidMethodBuilder Create()
        {
            return new AsyncVoidMethodBuilder();
        }

        public void SetException(Exception exception)
        {
            throw exception;
        }

        public void SetResult()
        {
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            // Should not get called as we don't implement the optimization that this method is used for.
            throw new NotImplementedException();
        }

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }
    }

    public struct AsyncTaskMethodBuilder
    {
        TaskCompletionSource<object> tcs;

        public Task Task { get { return tcs.Task; } }

        public static AsyncTaskMethodBuilder Create()
        {
            AsyncTaskMethodBuilder b;
            b.tcs = new TaskCompletionSource<object>();
            return b;
        }

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            // Should not get called as we don't implement the optimization that this method is used for.
            throw new NotImplementedException();
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        public void SetResult()
        {
            tcs.SetResult(null);
        }

        public void SetException(Exception exception)
        {
            tcs.SetException(exception);
        }
    }

    public struct AsyncTaskMethodBuilder<T>
    {
        TaskCompletionSource<T> tcs;

        public Task<T> Task { get { return tcs.Task; } }

        public static AsyncTaskMethodBuilder<T> Create()
        {
            AsyncTaskMethodBuilder<T> b;
            b.tcs = new TaskCompletionSource<T>();
            return b;
        }

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            // Should not get called as we don't implement the optimization that this method is used for.
            throw new NotImplementedException();
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            AwaitOnCompleted(ref awaiter, ref stateMachine);
        }

        public void SetResult(T result)
        {
            tcs.SetResult(result);
        }

        public void SetException(Exception exception)
        {
            tcs.SetException(exception);
        }
    }
}

#endif
