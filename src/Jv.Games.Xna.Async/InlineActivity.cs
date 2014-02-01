using Microsoft.Xna.Framework;
using System;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async
{
    public interface IInlineActivity
    {
        Task Run(Activity activity);
        Task<T> Run<T>(Activity<T> activity);
    }

    //public class InlineActivityBase : ActivityBase, IInlineActivity
    //{
    //    Action<IInlineActivity, GameTime> _update;
    //    Action<IInlineActivity, GameTime> _draw;

    //    public InlineActivityBase(Game game,
    //        Action<IInlineActivity, GameTime> update = null,
    //        Action<IInlineActivity, GameTime> draw = null)
    //        : base(game)
    //    {
    //        _update = update;
    //        _draw = draw;
    //    }

    //    protected override void Draw(GameTime gameTime)
    //    {
    //        if (_draw != null)
    //            _draw(this, gameTime);
    //    }

    //    protected override void Update(GameTime gameTime)
    //    {
    //        if (_update != null)
    //            _update(this, gameTime);
    //    }
    //}

    public class InlineActivity : ActivityBase, IInlineActivity
    {
        Action<IInlineActivity, GameTime> _update;
        Action<IInlineActivity, GameTime> _draw;

        public InlineActivity(Game game,
            Action<IInlineActivity, GameTime> update = null,
            Action<IInlineActivity, GameTime> draw = null)
            : base(game)
        {
            _update = update;
            _draw = draw;
        }

        new public Task Run(Activity activity)
        {
            return base.Run(activity);
        }

        new public Task<TActivity> Run<TActivity>(Activity<TActivity> activity)
        {
            return base.Run(activity);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (_draw != null)
                _draw(this, gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            if (_update != null)
                _update(this, gameTime);
        }
    }

    //public class InlineActivity<T> : InlineActivity
    //{
    //    public InlineActivity(Game game,
    //        Func<IInlineActivity, Task<T>> runActivity,
    //        Action<IInlineActivity, GameTime> update = null,
    //        Action<IInlineActivity, GameTime> draw = null)
    //        : base(game, runActivity, update, draw)
    //    {
    //    }
    //}
}
