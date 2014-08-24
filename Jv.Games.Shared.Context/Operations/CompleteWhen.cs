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
                throw new ArgumentNullException("checkCompletion");

            _checkCompletion = checkCompletion;
        }

        public override bool Continue(GameTime gameTime)
        {
            if (IsCompleted)
                return false;

            if (_checkCompletion(gameTime))
            {
                SetCompleted();
                return false;
            }

            return true;
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

