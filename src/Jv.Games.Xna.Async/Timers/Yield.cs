using System.Threading.Tasks;

namespace Jv.Games.Xna.Async.Timers
{
    public class Yield<T> : IGameLoopAction<T>
        where T : GameLoopEventArgs
    {
        public bool Step(T args)
        {
            return false;
        }
    }

    public static class YieldExtensions
    {
        public static Task<T> Yield<T>(this SyncContext<T> context)
            where T : GameLoopEventArgs
        {
            return context.RunTimer(new Yield<T>());
        }
    }
}
