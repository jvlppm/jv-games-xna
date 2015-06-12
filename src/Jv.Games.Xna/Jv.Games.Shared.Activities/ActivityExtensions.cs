namespace Jv.Games.Xna.Async
{
    using Microsoft.Xna.Framework;
    using System;
    using System.Threading.Tasks;

    public delegate Task<bool> GameWorkflow(RootActivity rootActivity);
    public sealed class RootActivity : Activity<bool>
    {
        public RootActivity(Game game) : base(game)
        {

        }

        public new ContextTask<T> Run<T>(Activity<T> activity)
        {
            return base.Run(activity);
        }
    }

    public static class ActivityExtensions
    {
        public static async Task<bool> Run(this Game game, GameWorkflow workflow)
        {
            var res = await game.RunComponent(new RootActivity(game), t => workflow(t));
            if (res)
            {
                game.Exit();
            }
            return res;
        }

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
