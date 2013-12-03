using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async.Timers
{
    public class Yield : ITimedOperation
    {
        public bool Tick(GameTime gameTime)
        {
            return false;
        }
    }

    public static class YieldExtensions
    {
        public static Task<GameTime> Yield(this SyncContext context)
        {
            return context.RunTimer(new Yield());
        }
    }
}
