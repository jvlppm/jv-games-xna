namespace Jv.Games.Xna.Context
{
    using Microsoft.Xna.Framework;

    public class Yield : GameOperation<GameTime>
    {
        public override bool Continue(GameTime gameTime)
        {
            SetResult(gameTime);
            return false;
        }
    }

    public static class YieldExtensions
    {
        public static ContextOperation<GameTime> Yield(this IContext context)
        {
            return context.Run(new Yield());
        }
    }
}
