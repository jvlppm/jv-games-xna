namespace Jv.Games.Xna.Async
{
    using Microsoft.Xna.Framework;
    using System;

    public interface IOperation
    {
        /// <summary>
        /// Updates the operation.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        /// <returns>True if the operation can continue (its not complete).</returns>
        bool Continue(GameTime gameTime);
        void Cancel();
    }

    public interface IAsyncOperation : IOperation
    {
        bool IsCompleted { get; }
        bool IsFaulted { get; }
        bool IsCanceled { get; }

        Exception Error { get; }

        void OnCompleted(Action continuation);
        void GetResult();
    }

    public interface IAsyncOperation<out T> : IAsyncOperation
    {
        new T GetResult();
    }
}
