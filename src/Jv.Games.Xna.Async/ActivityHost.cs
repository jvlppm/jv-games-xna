﻿using Microsoft.Xna.Framework;
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

        new public Task Run<T>(params object[] args)
            where T : Activity
        {
            return base.Run<T>(args);
        }

        new public Task<TResult> Run<T, TResult>(params object[] args)
            where T : Activity<TResult>
        {
            return base.Run<T, TResult>(args);
        }
    }

    public class ActivityHost : ActivityHost<Game>
    {
        public ActivityHost(Game game) : base(game) { }
    }
}
