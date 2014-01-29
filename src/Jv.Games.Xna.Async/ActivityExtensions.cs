﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async
{
    public static class ActivityExtensions
    {
        public static bool RenderParent(this IActivity activity)
        {
            return activity.IsTransparent && (activity.SubActivity == null || activity.SubActivity.RenderParent());
        }

        public static async Task<T> RunAsync<T>(this Game game, Activity<T> activity)
        {
            game.Components.Add(activity);
            var result = await activity.RunActivity();
            game.Components.Remove(activity);
            return result;
        }

        public static Task<T> RunAsync<T>(this Game game, Func<InlineActivity<T>, Task<T>> asyncMethod)
        {
            return game.RunAsync(new InlineActivity<T>(game, asyncMethod));
        }
    }
}