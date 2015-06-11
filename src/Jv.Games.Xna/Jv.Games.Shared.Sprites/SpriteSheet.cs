namespace Jv.Games.Xna.Sprites
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Linq;

    /// <summary>
    /// Sprite sheet image.
    /// </summary>
    public class SpriteSheet
    {
        /// <summary>
        /// Sprite sheet image.
        /// </summary>
        public readonly Texture2D Texture;

        /// <summary>
        /// Creates a new <see cref="Jv.Games.Xna.Sprites.SpriteSheet"/>.
        /// </summary>
        /// <param name="texture">Texture image containing the animation frames.</param>
        public SpriteSheet(Texture2D texture)
        {
            if (texture == null)
                throw new ArgumentNullException("texture");

            Texture = texture;
        }

        /// <summary>
        /// Extracts a <see cref="Jv.Games.Xna.Sprites.Animation"/> with the specified frame locations.
        /// </summary>
        /// <param name="name">Name of the extracted animation.</param>
        /// <param name="frameRects">Location coordinates of each frame in the animation.</param>
        /// <param name="frameDuration">How long each frame is displayed, before switching to the next frame.</param>
        /// <param name="repeat"><c>True</c> if the animation should repeat after the last frame finishes.</param>
        /// <returns>The extracted animation.</returns>
        public Animation GetAnimation(string name, Rectangle[] frameRects, TimeSpan frameDuration, bool repeat = true)
        {
            if (frameRects.Length <= 0 ||
                frameRects.Any(r => r.Width <= 0 || r.Height <= 0) ||
                frameRects.Any(r => r.X + r.Width > Texture.Width || r.Y + r.Height > Texture.Height))
                throw new ArgumentOutOfRangeException("frameRects");

            if (frameDuration <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("frameDuration");

            var frames = frameRects.Select(r => new Frame(Texture, r)).ToArray();
            var duration = TimeSpan.FromSeconds(frameDuration.TotalSeconds * frames.Length);
            if (repeat)
                return new LoopedAnimation(name, frames, duration, 0, null);
            return new Animation(name, frames, duration);
        }
    }
}
