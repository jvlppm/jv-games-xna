namespace Jv.Games.Xna.Sprites
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    public class Animation
    {
        TimeSpan _frameSpentTime;
        TimeSpan _currentFrameDuration;
        int _currentFrameIndex;
        Frame _currentFrame;

        public string Name { get; private set; }
        public Frame[] Frames { get; private set; }
        public TimeSpan Duration { get; private set; }
        public virtual bool IsFinished
        {
            get { return _currentFrameIndex >= Frames.Length; }
        }

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
        }

        #region Game Loop
        public virtual void Update(GameTime gameTime)
        {
            if (_currentFrameIndex >= Frames.Length)
                return;

            if (_currentFrame == null)
            {
                JumpToFrame(0);
                return;
            }

            _frameSpentTime += gameTime.ElapsedGameTime;
            if (_frameSpentTime > _currentFrameDuration)
            {
                _currentFrameIndex++;
                if (_currentFrameIndex < Frames.Length)
                    JumpToFrame(_currentFrameIndex);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, SpriteEffects effect, float rotation)
        {
            if (_currentFrame == null)
                return;

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

        public virtual void Reset()
        {
            JumpToFrame(0);
        }

        protected void JumpToFrame(int frameIndex)
        {
            if (frameIndex >= Frames.Length)
            {
                frameIndex = Frames.Length - 1;
                if (frameIndex < 0)
                {
                    _currentFrameIndex = -1;
                    _currentFrame = null;
                    return;
                }
            }

            _currentFrameIndex = frameIndex;
            _currentFrame = Frames[frameIndex];
            _currentFrameDuration = TimeSpan.FromSeconds(Duration.TotalSeconds / Frames.Length * _currentFrame.DurationWeight);
            _frameSpentTime = TimeSpan.Zero;
        }
    }
}
