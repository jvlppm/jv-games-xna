using Microsoft.Xna.Framework;
using System;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async
{
    public class InlineActivity<T> : Activity<T>
    {
        Action<InlineActivity<T>, GameTime> _update;
        Action<InlineActivity<T>, GameTime> _draw;
        Func<InlineActivity<T>, Task<T>> _runActivity;

        public InlineActivity(Game game,
            Func<InlineActivity<T>, Task<T>> runActivity,
            Action<InlineActivity<T>, GameTime> update = null,
            Action<InlineActivity<T>, GameTime> draw = null)
            : base(game)
        {
            if (runActivity == null)
                throw new ArgumentNullException("runActivity");
            _runActivity = runActivity;
            _update = update;
            _draw = draw;
        }

        internal protected override Task<T> RunActivity()
        {
            return _runActivity(this);
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
}
