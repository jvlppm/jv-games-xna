namespace Jv.Games.Xna.Sprites
{
    using Microsoft.Xna.Framework;
    using System;

    public class LoopedAnimation : Animation
    {
        int _currentLoopCount;

        public int? LoopCount { get; private set; }
        public int ResetToFrame { get; private set; }

        public LoopedAnimation(string name, Frame[] frames, TimeSpan duration, int resetToFrame, int? loopCount)
            : base(name, frames, duration)
        {
            if (resetToFrame >= frames.Length || resetToFrame < 0)
                throw new ArgumentOutOfRangeException("resetToFrame", "ResetToFrame must point to a valid frame index");
            if (loopCount < 0)
                throw new ArgumentOutOfRangeException("loopCount", "Invalid LoopCount quantity");

            ResetToFrame = resetToFrame;
            LoopCount = loopCount;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!base.IsFinished)
                return;

            if (LoopCount == null || _currentLoopCount < LoopCount)
            {
                _currentLoopCount++;
                Reset();
            }
        }

        public override void Reset()
        {
            base.Reset();
            _currentLoopCount = 0;
        }
    }
}
