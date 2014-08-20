using Microsoft.Xna.Framework;

namespace Jv.Games.Xna.Async
{
    public class Yield : AsyncOperation<GameTime>
    {
        public override bool Continue(GameTime gameTime)
        {
            SetResult(gameTime);
            return false;
        }
    }

    public static class YieldExtensions
    {
        public static ContextOperationAwaitable<GameTime> Yield(this AsyncContext context)
        {
            return context.Run(new Yield());
        }
    }
}
