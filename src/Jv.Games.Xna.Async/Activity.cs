using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async
{
    public abstract class Activity<T> : AsyncGameComponent
    {
        protected readonly TaskCompletionSource<T> ActivityCompletion;
        public virtual Task<T> GetTask()
        {
            return ActivityCompletion.Task;
        }

        public Activity(Game game)
            : base(game)
        {
            ActivityCompletion = new TaskCompletionSource<T>();
        }

        override protected abstract void Draw(GameTime gameTime);

        override protected abstract void Update(GameTime gameTime);

        protected void Exit(T result)
        {
            ActivityCompletion.SetResult(result);
        }
    }
}
