namespace Jv.Games.Xna.Async
{
    using Microsoft.Xna.Framework;
    using System;
    using System.Threading.Tasks;

    public static class ActivityExtensions
    {
        public static async Task<TResult> RunComponent<T, TResult>(this Game game, T component, Func<T, Task<TResult>> asyncMethod)
            where T : AsyncGameComponent
        {
            game.Components.Add(component);
            var result = await component.UpdateContext.Wait(asyncMethod(component));
            game.Components.Remove(component);
            return result;
        }

        #region Private Methods
        internal static bool AllTransparent(this IActivityStackItem activity)
        {
            return activity.IsTransparent && (activity.SubActivity == null || activity.SubActivity.AllTransparent());
        }
        #endregion
    }
}
