using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async
{
    public class ActivityHost<T> : ActivityBase
        where T : Game
    {
        new public T Game { get { return (T)base.Game; } }

        public ActivityHost(Game game)
            : base(game)
        {
        }

        [SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods")]
        new public Task Run(Activity activity)
        {
            return base.Run(activity);
        }

        [SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods")]
        new public Task<TActivity> Run<TActivity>(Activity<TActivity> activity)
        {
            return base.Run(activity);
        }

        new public ContextTaskAwaitable Run<TActivity>(params object[] args)
            where TActivity : Activity
        {
            return base.Run<TActivity>(args).On(UpdateContext);
        }

        new public ContextTaskAwaitable<TResult> Run<TActivity, TResult>(params object[] args)
            where TActivity : Activity<TResult>
        {
            return base.Run<TActivity, TResult>(args).On(UpdateContext);
        }
    }

    public class ActivityHost : ActivityHost<Game>
    {
        public ActivityHost(Game game) : base(game) { }
    }
}
