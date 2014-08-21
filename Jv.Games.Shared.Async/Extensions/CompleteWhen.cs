using Jv.Games.Xna.Async;
using Microsoft.Xna.Framework;
using System;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async
{
    public class CompleteWhenOperation : AsyncOperation
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
        public static ContextOperationAwaitable CompleteWhen(this AsyncContext context, Func<GameTime, bool> checkCompletion)
        {
            return context.Run(new CompleteWhenOperation(checkCompletion));
        }
    }
}

