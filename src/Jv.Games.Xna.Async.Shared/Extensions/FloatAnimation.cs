using Jv.Games.Xna.Async.Core;
using Microsoft.Xna.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using XNATweener;

namespace Jv.Games.Xna.Async
{
    public class FloatAnimation : IAsyncOperation<TimeSpan>
    {
        #region Attributes
        TaskCompletionSource<TimeSpan> _taskCompletion;

        public readonly TimeSpan Duration;
        public readonly Action<float> ValueStep;
        public readonly float StartValue;
        public readonly float EndValue;
        public readonly TweeningFunction EasingFunction;
        #endregion

        #region Properties
        public Task<TimeSpan> Task { get { return _taskCompletion.Task; } }
        Task IAsyncOperation.Task { get { return _taskCompletion.Task; } }

        public TimeSpan CurrentDuration { get; private set; }
        #endregion

        #region Constructors
        public FloatAnimation(TimeSpan duration, float startValue, float endValue, Action<float> valueStep, TweeningFunction easingFunction = null)
        {
            if (duration <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("duration", "Duration must be greater than zero");

            if (valueStep == null)
                throw new ArgumentNullException("valueStep");

            _taskCompletion = new TaskCompletionSource<TimeSpan>();

            Duration = duration;

            StartValue = startValue;
            EndValue = endValue;
            ValueStep = valueStep;
            EasingFunction = easingFunction;

            NotifyValue();
        }
        #endregion

        #region Public Methods
        public bool Continue(GameTime gameTime)
        {
            if (Task.IsCompleted)
                return false;

            CurrentDuration += gameTime.ElapsedGameTime;
            NotifyValue();

            if (CurrentDuration < Duration)
                return true;

            _taskCompletion.SetResult(CurrentDuration);
            return false;
        }

        public void Cancel()
        {
            _taskCompletion.TrySetCanceled();
        }
        #endregion

        #region Private Methods
        void NotifyValue()
        {
            ValueStep(GetValue());
        }

        float GetValue()
        {
            float curDuration = MathHelper.Clamp((float)CurrentDuration.TotalMilliseconds, 0, (float)Duration.TotalMilliseconds);

            if (EasingFunction != null)
                return EasingFunction(curDuration, StartValue, EndValue - StartValue, (float)Duration.TotalMilliseconds);

            var curValue = curDuration / (float)Duration.TotalMilliseconds;
            return MathHelper.Lerp(StartValue, EndValue, MathHelper.Clamp(curValue, 0, 1));
        }
        #endregion
    }
}
