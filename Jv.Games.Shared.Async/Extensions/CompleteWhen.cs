using Jv.Games.Xna.Async;
using Microsoft.Xna.Framework;
using System;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async
{
    class CompleteWhenOperation : IAsyncOperation
    {
        readonly TaskCompletionSource<bool> _tcs;
        readonly Func<GameTime, bool> _checkCompletion;

        public CompleteWhenOperation(Func<GameTime, bool> checkCompletion)
        {
            if (checkCompletion == null)
                throw new ArgumentNullException("checkCompletion");

            _checkCompletion = checkCompletion;
            _tcs = new TaskCompletionSource<bool>();
        }

        public void Cancel() { _tcs.TrySetCanceled(); }

        public Task Task { get { return _tcs.Task; } }

        public bool Continue(GameTime gameTime)
        {
            if (_tcs.Task.IsCompleted)
                return false;

            if (_checkCompletion(gameTime))
            {
                _tcs.TrySetResult(true);
                return false;
            }

            return true;
        }
    }

    static class CompleteWhenExtensions
    {
        public static ContextTaskAwaitable CompleteWhen(this AsyncContext context, Func<GameTime, bool> checkCompletion)
        {
            return context.Run(new CompleteWhenOperation(checkCompletion));
        }
    }
}

