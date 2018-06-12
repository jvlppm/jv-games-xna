namespace Jv.Games.Xna.Context
{
    using Microsoft.Xna.Framework;
    using System;

    public class CompleteWhenOperation : GameOperation
    {
        readonly Func<GameTime, bool> _checkCompletion;

        public CompleteWhenOperation(Func<GameTime, bool> checkCompletion)
        {
            if (checkCompletion == null)
                throw new ArgumentNullException(nameof(checkCompletion));

            _checkCompletion = checkCompletion;
        }

        public override void Continue(GameTime gameTime)
        {
            if (Status.IsCompleted)
                return;

            if (_checkCompletion(gameTime))
                Status.SetCompleted();
        }
    }

    public static class CompleteWhenExtensions
    {
        public static ContextOperation CompleteWhen(this IContext context, Func<GameTime, bool> checkCompletion)
        {
            return context.Run(new CompleteWhenOperation(checkCompletion));
        }
    }
}

