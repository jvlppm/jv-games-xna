namespace Jv.Games.Xna.XForms
{
    using Jv.Games.Xna.Context;
    using Jv.Games.Xna.Context.Operations;
    using Microsoft.Xna.Framework;
    using System;
    using Xamarin.Forms;

    sealed class PlatformTimer : RepeatingTimer, ITimer
    {
        readonly Context _context;
        bool _attached;

        public static PlatformTimer StartNew(Context context, Func<bool> callback, TimeSpan dueTime, TimeSpan period)
        {
            var timer = new PlatformTimer(context, callback, dueTime, period);
            if (dueTime >= TimeSpan.Zero)
            {
                context.Run(timer);
                timer._attached = true;
            }
            return timer;
        }

        public static PlatformTimer StartNew(Context context, Action<object> callback, object state, TimeSpan dueTime, TimeSpan period)
        {
            var timer = new PlatformTimer(context, callback, state, dueTime, period);
            if (dueTime >= TimeSpan.Zero)
            {
                context.Run(timer);
                timer._attached = true;
            }
            return timer;
        }

        public PlatformTimer(Context context, Action<object> callback)
            : base(TimeSpan.Zero, TimeSpan.Zero)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            _context = context;
            Ellapsed += (s, e) => callback(this);
        }

        PlatformTimer(Context context, Func<bool> callback, TimeSpan dueTime, TimeSpan period)
            : base(dueTime, period)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            _context = context;
            Ellapsed += delegate
            {
                if (!callback())
                {
                    _context.Remove(this);
                    _attached = false;
                }
            };
        }

        PlatformTimer(Context context, Action<object> callback, object state, TimeSpan dueTime, TimeSpan period)
            : base(dueTime, period)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            _context = context;
            Ellapsed += (s, e) => callback(state);
        }

        public override bool Continue(GameTime gameTime)
        {
            if (DueTime < TimeSpan.Zero)
            {
                _attached = false;
                return false;
            }

            return base.Continue(gameTime);
        }

        public override void Change(TimeSpan dueTime, TimeSpan period)
        {
            base.Change(dueTime, period);

            if (!_attached && dueTime >= TimeSpan.Zero)
                _context.Run(this);
        }

        public void Change(uint dueTime, uint period)
        {
            Change(TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(period));
        }

        public void Change(long dueTime, long period)
        {
            Change(TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(period));
        }

        public void Change(int dueTime, int period)
        {
            Change(TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(period));
        }
    }
}
