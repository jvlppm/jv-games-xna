using Jv.Games.Xna.Async.Core;
using Microsoft.Xna.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using XNATweener;

namespace Jv.Games.Xna.Async
{
    public static class AnimationExtensions
    {
        public static ContextTaskAwaitable<TimeSpan> Animate(this AsyncContext context, TimeSpan duration, float startValue, float endValue, Action<float> valueStep, CancellationToken cancellationToken = default(CancellationToken), TweeningFunction easingFunction = null)
        {
            var info = new FloatAnimation(duration, startValue, endValue, valueStep, easingFunction);

            if (cancellationToken != default(CancellationToken))
                cancellationToken.Register(info.Cancel);

            return context.Run(info);
        }

        public static ContextTaskAwaitable<TimeSpan> Animate(this AsyncContext context, TimeSpan duration, Reference<Color> color, Color endColor, CancellationToken cancellationToken = default(CancellationToken), TweeningFunction easingFunction = null)
        {
            if (color == null)
                throw new ArgumentNullException("color");

            return Animate(context, duration, color.Value, endColor, c =>
            {
                color.Value = c;
            }, cancellationToken, easingFunction);
        }

        public static ContextTaskAwaitable<TimeSpan> Animate(this AsyncContext context, TimeSpan duration, Color startColor, Color endColor, Action<Color> colorStep, CancellationToken cancellationToken = default(CancellationToken), TweeningFunction easingFunction = null)
        {
            if (colorStep == null)
                throw new ArgumentNullException("colorStep");

            var info = new FloatAnimation(duration, 0, 1, value =>
            {
                colorStep(Color.Lerp(startColor, endColor, value));
            }, easingFunction);

            if (cancellationToken != default(CancellationToken))
                cancellationToken.Register(info.Cancel);

            return context.Run(info);
        }
    }
}
