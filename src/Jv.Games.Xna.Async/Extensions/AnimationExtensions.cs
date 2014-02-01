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
        public static Task<TimeSpan> Animate(this AsyncContext context, TimeSpan duration, float startValue, float endValue, Action<float> valueStep, TweeningFunction easingFunction = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var info = new FloatAnimation(duration, startValue, endValue, valueStep, easingFunction);

            if (cancellationToken != default(CancellationToken))
                cancellationToken.Register(info.Cancel);

            return context.Run(info);
        }

        public static Task<TimeSpan> Animate(this AsyncContext context, TimeSpan duration, Reference<Color> color, Color endColor, TweeningFunction easingFunction = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (color == null)
                throw new ArgumentNullException("color");

            return Animate(context, duration, color.Value, endColor, c =>
            {
                color.Value = c;
            }, easingFunction, cancellationToken);
        }

        public static Task<TimeSpan> Animate(this AsyncContext context, TimeSpan duration, Color startColor, Color endColor, Action<Color> colorStep, TweeningFunction easingFunction = null, CancellationToken cancellationToken = default(CancellationToken))
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
