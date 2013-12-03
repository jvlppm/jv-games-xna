
using Microsoft.Xna.Framework;
using System;

namespace Jv.Games.Xna.Async
{
    public class ContextLoopEventArgs : GameLoopEventArgs
    {
        public ContextLoopEventArgs(SyncContext context, GameTime gameTime)
            : base(gameTime)
        {
            Context = context;
        }

        public SyncContext Context { get; private set; }
    }

    public delegate void ContextLoopEventHandler<T>(object sender, ContextLoopEventArgs e);
}
