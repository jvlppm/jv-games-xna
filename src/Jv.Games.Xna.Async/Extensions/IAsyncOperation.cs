using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async
{
    public interface IOperation
    {
        /// <summary>
        /// Updates the operation.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        /// <returns>True if the operation can continue (its not complete).</returns>
        bool Continue(GameTime gameTime);
    }

    public interface IAsyncOperation : IOperation
    {
        /// <summary>
        /// A task to represent the async operation.
        /// </summary>
        Task Task { get; }
        void Cancel();
    }

    public interface IAsyncOperation<T> : IAsyncOperation
    {
        /// <summary>
        /// A task to represent the async operation.
        /// </summary>
        new Task<T> Task { get; }
    }
}
