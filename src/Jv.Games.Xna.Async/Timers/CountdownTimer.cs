using Microsoft.Xna.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async.Timers
{
    public class CountdownTimer
    {
        #region Attributes
        public TimeSpan CurrentDuration;
        public readonly TimeSpan Duration;
        #endregion

        #region Constructors
        public CountdownTimer(TimeSpan duration)
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
        public virtual bool Tick(GameTime gameTime)
        {
            if (CurrentDuration >= Duration)
                return false;

            CurrentDuration += gameTime.ElapsedGameTime;
            return CurrentDuration >= Duration;
        }
        #endregion
    }

    public class CountdownTimer<T> : IGameLoopAction<T>
        where T : GameLoopEventArgs
    {
        public readonly CountdownTimer Timer;

        public CountdownTimer(TimeSpan duration)
        {
            Timer = new CountdownTimer(duration);
        }

        public bool Step(T args)
        {
            return Timer.Tick(args.GameTime);
        }
    }

    public static class CountdownTimerExtensions
    {
        /// <summary>
        /// Creates a task that will complete after a time delay.
        /// </summary>
        /// <param name="dueTime">The time span to wait before completing the returned task.</param>
        /// <param name="cancellationToken">The cancellation token that will be checked prior to completing the returned task.</param>
        public static Task<T> Delay<T>(SyncContext<T> context, TimeSpan dueTime, CancellationToken cancellationToken = default(CancellationToken))
            where T : GameLoopEventArgs
        {
            var timer = new CountdownTimer<T>(dueTime);
            return context.RunTimer(timer, cancellationToken);
        }
    }
}
