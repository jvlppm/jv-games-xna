namespace Jv.Games.Xna.Context.Operations
{
    using Microsoft.Xna.Framework;
    using System;

    public class RepeatingTimer : GameOperation
    {
        public TimeSpan DueTime, Period;

        public event EventHandler Ellapsed = delegate { };

        public RepeatingTimer(TimeSpan dueTime, TimeSpan period)
        {
            DueTime = dueTime;
            Period = period;
        }

        public override bool Continue(GameTime gameTime)
        {
            DueTime -= gameTime.ElapsedGameTime;
            if (DueTime <= TimeSpan.Zero)
            {
                DueTime = Period;
                Ellapsed(this, EventArgs.Empty);
            }

            return true;
        }

        public virtual void Change(TimeSpan dueTime, TimeSpan period)
        {
            Period = period;
            DueTime = dueTime;
        }
    }
}
