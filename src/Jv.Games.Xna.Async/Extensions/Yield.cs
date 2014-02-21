using Jv.Games.Xna.Async.Core;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async
{
    public class Yield : IAsyncOperation<GameTime>
    {
        #region Attributes
        TaskCompletionSource<GameTime> _taskCompletion;
        #endregion

        #region Properties
        public Task<GameTime> Task { get { return _taskCompletion.Task; } }
        Task IAsyncOperation.Task { get { return _taskCompletion.Task; } }
        #endregion

        public Yield()
        {
            _taskCompletion = new TaskCompletionSource<GameTime>();
        }

        public bool Continue(GameTime gameTime)
        {
            _taskCompletion.TrySetResult(gameTime);
            return false;
        }

        public void Cancel()
        {
            _taskCompletion.SetCanceled();
        }
    }

    public static class YieldExtensions
    {
        public static ContextTaskAwaitable<GameTime> Yield(this AsyncContext context)
        {
            return context.Run(new Yield());
        }
    }
}
