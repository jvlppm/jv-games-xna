namespace Jv.Games.Xna.Context
{
    using System;

    public interface IContext
    {
        void Post(Action action);

        ContextOperation Run(IGameOperation operation);

        ContextOperation<T> Run<T>(IGameOperation<T> operation);
    }
}

