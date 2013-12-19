using Microsoft.Xna.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async.Operations
{
    public class CountdownTimer : IAsyncOperation<TimeSpan>
    {
        #region Attributes
        TaskCompletionSource<TimeSpan> _taskCompletion;

        public readonly TimeSpan Duration;
        #endregion

        #region Properties
        public Task<TimeSpan> Task { get { return _taskCompletion.Task; } }
        Task IAsyncOperation.Task { get { return _taskCompletion.Task; } }

        public TimeSpan CurrentDuration { get; private set; }
        #endregion

        #region Constructors
        public CountdownTimer(TimeSpan duration)
        {
            _taskCompletion = new TaskCompletionSource<TimeSpan>();
            Duration = duration;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Updates the timer CurrentDuration.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        /// <returns>True if the timer is complete.</returns>
        public virtual bool Continue(GameTime gameTime)
        {
            if (Task.IsCompleted)
                return false;

            CurrentDuration += gameTime.ElapsedGameTime;
            if (CurrentDuration >= Duration)
            {
                _taskCompletion.SetResult(CurrentDuration);
                return false;
            }
            return true;
        }

        public void Cancel()
        {
            _taskCompletion.SetCanceled();
        }
        #endregion
    }

    public static class CountdownTimerExtensions
    {
        /// <summary>
        /// Creates a task that will complete after a time delay.
        /// </summary>
        /// <param name = "context">The context to run the operation.</param>
        /// <param name="dueTime">The time span to wait before completing the returned task.</param>
        /// <param name="cancellationToken">The cancellation token that will be checked prior to completing the returned task.</param>
        public static Task<TimeSpan> Delay(this AsyncContext context, TimeSpan dueTime, CancellationToken cancellationToken = default(CancellationToken))
        {
            var timer = new CountdownTimer(dueTime);

            if(cancellationToken != default(CancellationToken))
                cancellationToken.Register(timer.Cancel);

            return context.Run(timer);
        }
    }
}
