namespace Jv.Games.Xna.Sprites
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Linq;

    /// <summary>
    /// Sprite animation.
    /// </summary>
    public class Animation
    {
        #region Attributes
        TimeSpan _frameSpentTime;
        TimeSpan _currentFrameDuration;
        int _currentFrameIndex;
        Frame _currentFrame;
        #endregion

        #region Properties
        /// <summary>
        /// Name of this animation.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Array of sequential image frames to be played.
        /// </summary>
        public Frame[] Frames { get; private set; }
        /// <summary>
        /// How long it takes to play all frames sequentially.
        /// </summary>
        public TimeSpan Duration { get; private set; }
        /// <summary>
        /// <c>True</c> if the animation reached its end.
        /// </summary>
        public virtual bool IsFinished
        {
            get { return _currentFrameIndex >= Frames.Length - 1 && _frameSpentTime > _currentFrameDuration; }
        }

        /// <summary>
        /// Frame used during Draw.
        /// When the animation completes, it will refer to the last played frame.
        /// </summary>
        public Frame CurrentFrame { get { return _currentFrame; } }
        /// <summary>
        /// Index of the current frame, inside the Frames array.
        /// </summary>
        public int CurrentFrameIndex { get { return _currentFrameIndex; } }
        /// <summary>
        /// How long this frame is being shown.
        /// </summary>
        public TimeSpan FrameSpentTime { get { return _frameSpentTime; } }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new playable animation.
        /// </summary>
        /// <param name="name">Name of the animation</param>
        /// <param name="frames">Frames to be played sequentially</param>
        /// <param name="duration">How long it should take to play all frames sequentially.</param>
        public Animation(string name, Frame[] frames, TimeSpan duration)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Name cannot be null or empty", "name");
            if (frames == null)
                throw new ArgumentNullException("frames");
            if (frames.Length <= 0)
                throw new ArgumentException("Frames array cannot be empty", "frames");
            if (duration <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("duration", "Invalid duration");

            Name = name;
            Frames = frames;
            Duration = duration;
            Reset();
        }
        #endregion

        #region Game Loop
        /// <summary>
        /// Updates the animation state.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        public virtual void Update(GameTime gameTime)
        {
            _frameSpentTime += gameTime.ElapsedGameTime;
            if (_frameSpentTime > _currentFrameDuration)
            {
                var nextFrame = _currentFrameIndex + 1;
                if (nextFrame < Frames.Length)
                    JumpToFrame(nextFrame);
            }
        }

        /// <summary>
        /// Draw the current animation frame into the specified sprite batch.
        /// <c>spritebatch.Begin()</c> and <c>spritebatch.End()</c> should be called
        /// before and after this method call respectivelly.
        /// </summary>
        /// <param name="spriteBatch"><see cref="Microsoft.Xna.Framework.Graphics.SpriteBatch"/> used to draw this animation.</param>
        /// <param name="position">Location to draw this animation.</param>
        /// <param name="color">The color channel modulation to use. Use <c>Color.White</c> for full color with no tinting.</param>
        /// <param name="effect">Effect to apply prior to rendering.</param>
        /// <param name="rotation">The angle, in radians, to rotate the sprite around the origin.</param>
        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, SpriteEffects effect, float rotation)
        {
            spriteBatch.Draw(
                texture: _currentFrame.Texture,
                sourceRectangle: _currentFrame.Rectangle,
                origin: _currentFrame.Origin,
                position: position,
                rotation: rotation,
                color: color,
                effect: effect);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Restart the animation to its initial state.
        /// </summary>
        public virtual void Reset()
        {
            JumpToFrame(0);
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Forces the animation to play from beginning of the specified frame index.
        /// </summary>
        /// <param name="frameIndex">Frame index to be played.</param>
        protected void JumpToFrame(int frameIndex)
        {
            if (frameIndex < 0)
                throw new ArgumentOutOfRangeException("frameIndex", "CurrentFrameIndex cannot be negative");
            if (frameIndex >= Frames.Length)
                throw new ArgumentOutOfRangeException("frameIndex", "CurrentFrameIndex cannot point outside the Frames array");

            _currentFrameIndex = frameIndex;
            _currentFrame = Frames[frameIndex];
            _currentFrameDuration = TimeSpan.FromSeconds((Duration.TotalSeconds / Frames.Sum(f => f.DurationWeight)) * _currentFrame.DurationWeight);
            _frameSpentTime = TimeSpan.Zero;
        }
        #endregion
    }
}
