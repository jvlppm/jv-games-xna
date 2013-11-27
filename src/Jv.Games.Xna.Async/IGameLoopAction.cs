using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async
{
    public interface ITimedOperation
    {
        bool Tick(GameTime gameTime);
    }
}
