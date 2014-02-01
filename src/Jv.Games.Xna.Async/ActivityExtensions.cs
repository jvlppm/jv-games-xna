using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async
{
    public static class ActivityExtensions
    {
        public static bool RenderParent(this IActivityStackItem activity)
        {
            return activity.IsTransparent && (activity.SubActivity == null || activity.SubActivity.RenderParent());
        }

        private static async Task<TResult> RunComponent<T, TResult>(Game game, T component, Func<T, Task<TResult>> asyncMethod)
            where T : IGameComponent
        {
            game.Components.Add(component);
            var result = await asyncMethod(component);
            game.Components.Remove(component);
            return result;
        }

        public static Task RunComponent<T>(Game game, T component, Func<T, Task> asyncMethod)
            where T : AsyncGameComponent
        {
            using(component.UpdateContext.Activate())
                return RunComponent(game, component, c => asyncMethod(c).ContinueWith(t => true));
        }

        public static Task Play(this Game game, Func<ActivityHost, Task> asyncMethod)
        {
            var act = new ActivityHost(game);
            return RunComponent<ActivityHost>(game, act, asyncMethod).ContinueWith(t =>
            {
                game.Exit();
            }, TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
