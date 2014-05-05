using Microsoft.Xna.Framework;
using System;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async
{
    public static class ActivityExtensions
    {
        public static Task RunComponent<T>(this Game game, T component, Func<T, Task> asyncMethod)
            where T : AsyncGameComponent
        {
            using(component.UpdateContext.Activate())
                return RunComponent(game, component, c => asyncMethod(c).Select(true));
        }

        public static Task Play(this Game game, Func<ActivityHost, Task> asyncMethod)
        {
            var act = new ActivityHost(game);
            return RunComponent<ActivityHost>(game, act, asyncMethod)
                    .Finally(t => { game.Exit(); });
        }

        public static Task Play<T>(this T game, Func<ActivityHost<T>, Task> asyncMethod)
            where T : Game
        {
            var act = new ActivityHost<T>(game);
            return RunComponent<ActivityHost<T>>(game, act, asyncMethod)
                    .Finally(t => { game.Exit(); });
        }

        #region Private Methods
        internal static bool AllTransparent(this IActivityStackItem activity)
        {
            return activity.IsTransparent && (activity.SubActivity == null || activity.SubActivity.AllTransparent());
        }
        static async Task<TResult> RunComponent<T, TResult>(Game game, T component, Func<T, Task<TResult>> asyncMethod)
            where T : IGameComponent
        {
            game.Components.Add(component);
            var result = await asyncMethod(component);
            game.Components.Remove(component);
            return result;
        }
        #endregion
    }
}
