using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async.Timers
{
    public class Yield : ITimer
    {
        public bool Tick(GameTime gameTime)
        {
            return false;
        }
    }

    public static class YieldExtensions
    {
        public static Task<GameTime> Yield(this TimerContext context)
        {
            return context.Run(new Yield());
        }
    }
}
