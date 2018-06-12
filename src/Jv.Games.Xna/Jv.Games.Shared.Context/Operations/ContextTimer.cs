namespace Jv.Games.Xna.Context
{
    using System;
    using System.Threading;
    using Microsoft.Xna.Framework;

    public class ContextTimer : GameOperation<TimeSpan>
    {
        #region Attributes

        public TimeSpan Duration;

        #endregion

        #region Properties

        public TimeSpan Time { get; private set; }

        public TimeSpan RemainingTime => Duration - Time;

        #endregion

        #region Constructors

        public ContextTimer(TimeSpan duration)
        {
            Duration = duration;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the timer CurrentDuration.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        /// <returns>True if the timer is complete.</returns>
        public override void Continue(GameTime gameTime)
        {
            if (Status.IsCompleted)
                return;

            Time += gameTime.ElapsedGameTime;
            if (Time >= Duration)
                Status.SetResult(Time);
        }

        public void Cancel() => Status.Cancel();

        #endregion
    }

    public static class ContextTimerExtensions
    {
        /// <summary>
        /// Creates a task that will complete after a time delay.
        /// </summary>
        /// <param name = "context">The context to run the operation.</param>
        /// <param name="dueTime">The time span to wait before completing the returned task.</param>
        /// <param name="cancellationToken">The cancellation token that will be checked prior to completing the returned task.</param>
        public static ContextOperation<TimeSpan> Delay(this IContext context, TimeSpan dueTime, CancellationToken cancellationToken = default(CancellationToken))
        {
            var timer = new ContextTimer(dueTime);

            if (cancellationToken != default(CancellationToken))
                cancellationToken.Register(timer.Cancel);

            return context.Run(timer);
        }
    }
}
