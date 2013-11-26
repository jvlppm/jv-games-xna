using System.Threading.Tasks;

namespace Jv.Games.Xna.Async.Timers
{
    public class YieldTimer<T> : ITimer<T>
        where T : GameLoopEventArgs
    {
        public bool Tick(T args)
        {
            return false;
        }
    }
}
