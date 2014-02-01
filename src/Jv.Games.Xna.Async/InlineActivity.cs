using Microsoft.Xna.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async
{
    public class InlineActivity : ActivityBase
    {
        Action<InlineActivity, GameTime> _update;
        Action<InlineActivity, GameTime> _draw;

        public InlineActivity(Game game,
            Action<InlineActivity, GameTime> update = null,
            Action<InlineActivity, GameTime> draw = null)
            : base(game)
        {
            _update = update;
            _draw = draw;
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
