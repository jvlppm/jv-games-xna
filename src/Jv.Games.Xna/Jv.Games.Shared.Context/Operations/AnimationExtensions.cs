namespace Jv.Games.Xna.Context
{
    using System;
    using System.Threading;
    using Jv.Games.Xna.Base;
    using Microsoft.Xna.Framework;

    public static class AnimationExtensions
    {
        public static ContextOperation<TimeSpan> Animate(this IContext context, TimeSpan duration, float startValue, float endValue, Action<float> valueStep, CancellationToken cancellationToken = default(CancellationToken)
			#if !DISABLE_TWEENER
            , XNATweener.TweeningFunction easingFunction = null
			#endif
        )
        {
            var info = new FloatAnimation(duration, startValue, endValue, valueStep
				#if !DISABLE_TWEENER
				, easingFunction
				#endif
                       );

            if (cancellationToken != default(CancellationToken))
                cancellationToken.Register(info.Cancel);

            return context.Run(info);
        }

        public static ContextOperation<TimeSpan> Animate(this IContext context, TimeSpan duration, Vector2 start, Vector2 end, Action<Vector2> step, CancellationToken cancellationToken = default(CancellationToken)
#if !DISABLE_TWEENER
            , XNATweener.TweeningFunction easingFunction = null
#endif
        )
        {
            if (step == null)
                throw new ArgumentNullException("colorStep");

            var info = new FloatAnimation(duration, 0, 1, value => step(new Vector2(
                x: MathHelper.Lerp(start.X, end.X, value),
                y: MathHelper.Lerp(start.Y, end.Y, value)))
#if !DISABLE_TWEENER
                , easingFunction
#endif
                       );

            if (cancellationToken != default(CancellationToken))
                cancellationToken.Register(info.Cancel);

            return context.Run(info);
        }

        public static ContextOperation<TimeSpan> Animate(this IContext context, TimeSpan duration, Reference<Color> color, Color endColor, CancellationToken cancellationToken = default(CancellationToken)
			#if !DISABLE_TWEENER
            , XNATweener.TweeningFunction easingFunction = null
			#endif
        )
        {
            if (color == null)
                throw new ArgumentNullException("color");

            return Animate(context, duration, color.Value, endColor, c =>
            {
                color.Value = c;
            }, cancellationToken
				#if !DISABLE_TWEENER
				, easingFunction
				#endif
            );
        }

        public static ContextOperation<TimeSpan> Animate(this IContext context, TimeSpan duration, Color startColor, Color endColor, Action<Color> colorStep, CancellationToken cancellationToken = default(CancellationToken)
			#if !DISABLE_TWEENER
            , XNATweener.TweeningFunction easingFunction = null
			#endif
        )
        {
            if (colorStep == null)
                throw new ArgumentNullException("colorStep");

            var info = new FloatAnimation(duration, 0, 1, value =>
            colorStep(Color.Lerp(startColor, endColor, value))
				#if !DISABLE_TWEENER
				, easingFunction
				#endif
                       );

            if (cancellationToken != default(CancellationToken))
                cancellationToken.Register(info.Cancel);

            return context.Run(info);
        }
    }
}
