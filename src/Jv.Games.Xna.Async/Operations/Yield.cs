using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async.Operations
{
    public class Yield : IAsyncOperation
    {
        public bool Continue(GameTime gameTime)
        {
            return false;
        }
    }

    public static class YieldExtensions
    {
        public static Task<GameTime> Yield(this AsyncContext context)
        {
            return context.Run(new Yield());
        }
    }
}
