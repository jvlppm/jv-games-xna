using Microsoft.Xna.Framework;
using System;
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

    public class CountdownTimer<T> : ITimer<T>
        where T : GameLoopEventArgs
    {
        public readonly CountdownTimer Timer;
        TaskCompletionSource<T> Completion;
        public Task<T> Task { get { return Completion.Task; } }

        public CountdownTimer(TimeSpan duration)
        {
            Timer = new CountdownTimer(duration);
            Completion = new TaskCompletionSource<T>();
        }

        public bool Tick(T args)
        {
            if (Completion.Task.IsCompleted)
                return false;

            if (!Timer.Tick(args.GameTime))
            {
                Completion.TrySetResult(args);
                return false;
            }

            return true;
        }

        public void Cancel()
        {
            Completion.TrySetCanceled();
        }
    }
}
