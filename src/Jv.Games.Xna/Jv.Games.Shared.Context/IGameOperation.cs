namespace Jv.Games.Xna.Context
{
    using System;
    using Microsoft.Xna.Framework;
    using System.Runtime.CompilerServices;

    public interface IGameOperation
    {
        IOperationStatus Status { get; }

        /// <summary>
        /// Updates the operation.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        /// <returns>True if the operation can continue (its not complete).</returns>
        void Continue(GameTime gameTime);
    }

    public interface IGameOperation<out T> : IGameOperation
    {
        new IOperationStatus<T> Status { get; }
    }

    public interface IOperationStatus
    {
        /// <summary>
        /// Gets whether this IGameOperation has completed.
        /// </summary>
        /// <value><c>true</c> if this operation is completed; otherwise, <c>false</c>.</value>
        bool IsCompleted { get; }

        /// <summary>
        /// Gets whether the IGameOperation completed due to an exception.
        /// </summary>
        /// <value><c>true</c> if this instance is completed in a faulted state; otherwise, <c>false</c>.</value>
        bool IsFaulted { get; }

        /// <summary>
        /// Gets whether this IGameOperation instance has completed execution due to being canceled.
        /// </summary>
        /// <value><c>true</c> if this instance is completed in a canceled state; otherwise, <c>false</c>.</value>
        bool IsCanceled { get; }

        /// <summary>
        /// Gets the state of this operation.
        /// </summary>
        /// <value>The state.</value>
        OperationState State { get; }

        /// <summary>
        /// Gets the error of the operation, if it is in a faulted state.
        /// </summary>
        /// <value>The resulting error.</value>
        Exception Error { get; }

        /// <summary>
        /// Throws the resulting exception if the operation failed.
        /// </summary>
        void GetResult();

        void OnCompleted(Action continuation);
    }

    public interface IOperationStatus<out T> : IOperationStatus
    {
        /// <summary>
        /// Throws the resulting exception if the operation failed.
        /// </summary>
        /// <returns>The result of the operation.</returns>
        new T GetResult();
    }
}

