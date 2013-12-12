using Microsoft.Xna.Framework;

namespace Jv.Games.Xna.Async.Operations
{
    public interface IAsyncOperation
    {
        bool Continue(GameTime gameTime);
    }
}
