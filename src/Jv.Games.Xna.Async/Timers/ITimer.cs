using System.Threading.Tasks;

namespace Jv.Games.Xna.Async.Timers
{
    public interface ITimer<T>
        where T : GameLoopEventArgs
    {
        bool Tick(T args);
    }
}
