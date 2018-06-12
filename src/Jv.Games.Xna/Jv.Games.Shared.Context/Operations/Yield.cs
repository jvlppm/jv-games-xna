namespace Jv.Games.Xna.Context
{
    using System;
    using Microsoft.Xna.Framework;

    public class Yield : GameOperation<TimeSpan>
    {
        public override void Continue(GameTime gameTime)
        {
            if (Status.IsCompleted)
                return;

            Status.SetResult(gameTime.ElapsedGameTime);
        }
    }

    public static class YieldExtensions
    {
        public static ContextOperation<TimeSpan> Yield(this IContext context)
        {
            return context.Run(new Yield());
        }
    }
}
