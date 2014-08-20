using Microsoft.Xna.Framework;
using System;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async
{
    public static class ActivityExtensions
    {
        public static Task<TResult> RunComponent<T, TResult>(this Game game, T component, Func<T, Task<TResult>> asyncMethod)
            where T : AsyncGameComponent
        {
            var oldContext = Context.Current;
            Context.Current = component.UpdateContext;

            try
            {
                return RunComponentSafe(game, component, asyncMethod);
            }
            finally
            {
                Context.Current = oldContext;
            }
        }

        #region Private Methods
        internal static bool AllTransparent(this IActivityStackItem activity)
        {
            return activity.IsTransparent && (activity.SubActivity == null || activity.SubActivity.AllTransparent());
        }
        static async Task<TResult> RunComponentSafe<T, TResult>(Game game, T component, Func<T, Task<TResult>> asyncMethod)
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
