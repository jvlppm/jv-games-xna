namespace Jv.Games.Xna.Context
{
    using Microsoft.Xna.Framework;
    using System;

    public class RepeatingTimer : GameOperation, IDisposable
    {
        public event EventHandler Elapsed;

        public TimeSpan DueTime { get; private set; }
        public TimeSpan Period { get; private set; }

        public RepeatingTimer(TimeSpan dueTime, TimeSpan period)
        {
            DueTime = dueTime;
            Period = period;
        }

        public RepeatingTimer(TimeSpan period, bool startImmediately = true)
        {
            DueTime = startImmediately? TimeSpan.Zero : period;
            Period = period;
        }

        public override void Continue(GameTime gameTime)
        {
            if (Status.IsCompleted)
                return;

            DueTime -= gameTime.ElapsedGameTime;
            if (DueTime <= TimeSpan.Zero)
            {
                DueTime = Period;
                Elapsed(this, EventArgs.Empty);
            }
        }

        public void Change(TimeSpan dueTime, TimeSpan period)
        {
            if (Status.IsCompleted)
                throw new ObjectDisposedException(GetType().FullName);

            Period = period;
            DueTime = dueTime;
        }

        public void Dispose()
        {
            Status.SetCompleted();
        }
    }

    public static class RepeatingTimerExtensions
    {
        public static RepeatingTimer StartTimer(this IContext context, TimeSpan period, bool startImmediatelly = true)
        {
            var timer = new RepeatingTimer(period, startImmediatelly);
            context.Run(timer);
            return timer;
        }
    }
}
