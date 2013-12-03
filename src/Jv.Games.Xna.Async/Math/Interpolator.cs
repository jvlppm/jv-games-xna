using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XNATweener;

namespace Jv.Games.Xna.Async.Math
{
    public class Interpolator : ITimedOperation
    {
        bool _completed;

        float CurrentDuration;
        float Duration;
        Action<float> ValueStep;
        float StartValue;
        float EndValue;
        TweeningFunction EasingFunction;

        public Interpolator(TimeSpan duration, float startValue, float endValue, Action<float> valueStep, TweeningFunction easingFunction = null)
        {
            if (duration <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("duration", "Duration must be greater than zero");

            if (valueStep == null)
                throw new ArgumentNullException("valueStep");

            Duration = (float)duration.TotalMilliseconds;
            ValueStep = valueStep;
            StartValue = startValue;
            EndValue = endValue;
            EasingFunction = easingFunction;

            NotifyValue();
        }

        public void NotifyValue()
        {
            ValueStep(GetValue());
        }

        public bool Tick(GameTime gameTime)
        {
            if (_completed)
                return false;

            Duration += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (CurrentDuration < Duration)
            {
                NotifyValue();
                return true;
            }

            CurrentDuration = Duration;
            NotifyValue();
            _completed = true;
            return false;
        }

        float GetValue()
        {
            if (EasingFunction != null)
                return EasingFunction(CurrentDuration, StartValue, EndValue - StartValue, Duration);

            var curValue = CurrentDuration / Duration;
            return MathHelper.Lerp(StartValue, EndValue, MathHelper.Clamp(curValue, 0, 1));
        }
    }

    public static class InterpolatorExtensions
    {
        public static Task<GameTime> Interpolate(this SyncContext context, TimeSpan duration, float startValue, float endValue, Action<float> valueStep, TweeningFunction easingFunction = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var info = new Interpolator(duration, startValue, endValue, valueStep, easingFunction);
            return context.RunTimer(info, cancellationToken);
        }
    }
}
