
using Microsoft.Xna.Framework;
using System;

namespace Jv.Games.Xna.Async
{
    public class GameLoopEventArgs : EventArgs
    {
        public GameLoopEventArgs(GameTime gameTime)
        {
            GameTime = gameTime;
        }

        public GameTime GameTime { get; private set; }
    }

    public delegate void GameLoopEventHandler(object sender, GameLoopEventArgs e);
    public delegate void GameLoopEventHandler<in T>(object sender, T e) where T : GameLoopEventArgs;
}
