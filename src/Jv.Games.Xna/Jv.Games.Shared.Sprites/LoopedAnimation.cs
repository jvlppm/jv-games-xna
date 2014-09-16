namespace Jv.Games.Xna.Sprites
{
    using Microsoft.Xna.Framework;
    using System;

    /// <summary>
    /// Animation that repeats when played.
    /// </summary>
    public class LoopedAnimation : Animation
    {
        int _currentLoopCount;

        /// <summary>
        /// How many times the animation should repeat, before completing.
        /// </summary>
        public int? LoopCount { get; private set; }
        /// <summary>
        /// Index of the first frame to be played, after an animation loop completes.
        /// </summary>
        public int ResetToFrame { get; private set; }

        /// <summary>
        /// Creates a new <see cref="Jv.Games.Xna.Sprites.LoopedAnimation"/>.
        /// </summary>
        /// <param name="name">Name of the animation</param>
        /// <param name="frames">Frames to be played sequentially</param>
        /// <param name="duration">How long it should take to play all frames sequentially.</param>
        /// <param name="resetToFrame">Index of the first frame to be played, after an animation loop completes.</param>
        /// <param name="loopCount">How many times the animation should repeat, before completing.</param>
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

        /// <summary>
        /// Updates the animation state.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
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

        /// <summary>
        /// Restart the animation to its initial state.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            _currentLoopCount = 0;
        }
    }
}
