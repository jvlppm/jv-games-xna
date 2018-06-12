namespace Jv.Games.Xna.Context
{
    using Microsoft.Xna.Framework;
    using System;
    using System.Runtime.ExceptionServices;

    public abstract class GameOperation : IGameOperation
    {
        readonly OperationStatus _status = new OperationStatus();

        protected OperationStatus Status => _status;

        IOperationStatus IGameOperation.Status => _status;

        #region IOperation implementation

        public abstract void Continue(GameTime gameTime);

        #endregion
    }

    public abstract class GameOperation<T> : IGameOperation<T>
    {
        readonly OperationStatus<T> _status = new OperationStatus<T>();

        protected OperationStatus<T> Status => _status;

        IOperationStatus IGameOperation.Status => _status;

        IOperationStatus<T> IGameOperation<T>.Status => _status;

        #region IOperation implementation

        public abstract void Continue(GameTime gameTime);


        #endregion
    }
}
