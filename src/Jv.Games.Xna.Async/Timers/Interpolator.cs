using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNATweener;

namespace Jv.Games.Xna.Async.Timers
{
    public class Interpolator<T> : ITimer<T>
        where T : GameLoopEventArgs
    {
        TaskCompletionSource<T> Completion;
        float CurrentDuration;
        float Duration;
        Action<float> ValueStep;
        float StartValue;
        float EndValue;
        TweeningFunction EasingFunction;

        public Task<T> Task { get { return Completion.Task; } }

        public Interpolator(TimeSpan duration, float startValue, float endValue, Action<float> valueStep, TweeningFunction easingFunction = null)
        {
            if (duration <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("duration", "Duration must be greater than zero");

            if (valueStep == null)
                throw new ArgumentNullException("valueStep");

            Completion = new TaskCompletionSource<T>();
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

        public bool Tick(T args)
        {
            if (Completion.Task.IsCompleted)
                return false;

            Duration += (float)args.GameTime.ElapsedGameTime.TotalMilliseconds;

            if (CurrentDuration < Duration)
            {
                NotifyValue();
                return true;
            }

            CurrentDuration = Duration;
            NotifyValue();
            Completion.TrySetResult(args);
            return false;
        }

        public void Cancel()
        {
            Completion.TrySetCanceled();
        }

        float GetValue()
        {
            if (EasingFunction != null)
                return EasingFunction(CurrentDuration, StartValue, EndValue - StartValue, Duration);

            var curValue = CurrentDuration / Duration;
            return MathHelper.Lerp(StartValue, EndValue, MathHelper.Clamp(curValue, 0, 1));
        }
    }
}
