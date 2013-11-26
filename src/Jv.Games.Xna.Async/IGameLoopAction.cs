using System.Threading.Tasks;

namespace Jv.Games.Xna.Async
{
    public interface IGameLoopAction<T>
        where T : GameLoopEventArgs
    {
        bool Step(T args);
    }
}
