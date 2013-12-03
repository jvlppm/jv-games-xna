
using Microsoft.Xna.Framework;
using System;

namespace Jv.Games.Xna.Async
{
    public class ContextLoopEventArgs<T> : GameLoopEventArgs
        where T : GameLoopEventArgs
    {
        public ContextLoopEventArgs(SyncContext<T> context, GameTime gameTime)
            : base(gameTime)
        {
            Context = context;
        }

        public SyncContext<T> Context { get; private set; }
    }

    public delegate void ContextLoopEventHandler<T>(object sender, ContextLoopEventArgs<T> e) where T : GameLoopEventArgs;
}
