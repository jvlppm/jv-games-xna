
using Microsoft.Xna.Framework;
using System;

namespace Jv.Games.Xna.Async
{
    public class TimerContextLoopEventArgs : GameLoopEventArgs
    {
        public TimerContextLoopEventArgs(TimerContext context, GameTime gameTime)
            : base(gameTime)
        {
            Context = context;
        }

        public TimerContext Context { get; private set; }
    }

    public delegate void TimerContextLoopEventHandler<T>(object sender, TimerContextLoopEventArgs e);
}
