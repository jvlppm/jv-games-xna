namespace Jv.Games.Xna.Sprites
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Linq;

    /// <summary>
    /// Sprite sheet image formed by a grid of animation frames.
    /// </summary>
    public class GridSpriteSheet : SpriteSheet
    {
        /// <summary>
        /// Number of image frames per row.
        /// </summary>
        public readonly int Columns;
        /// <summary>
        /// Number of image frames per column.
        /// </summary>
        public readonly int Rows;
        /// <summary>
        /// The size in pixels of each image frame.
        /// </summary>
        public readonly Point FrameSize;

        /// <summary>
        /// Creates a new <see cref="Jv.Games.Xna.Sprites.GridSpriteSheet"/> grid.
        /// </summary>
        /// <param name="texture">Texture image containing the animations.</param>
        /// <param name="columns">The number of columns this sprite sheet contains.</param>
        /// <param name="rows">The number of rows this sprite sheet contains.</param>
        public GridSpriteSheet(Texture2D texture, int columns, int rows)
            : base(texture)
        {
            if (columns <= 0)
                throw new ArgumentOutOfRangeException("columns");
            if (rows <= 0)
                throw new ArgumentOutOfRangeException("rows");

            if (texture.Width % columns != 0 || texture.Height % rows != 0)
                throw new ArgumentException("Texture size does not match rows / columns");

            Columns = columns;
            Rows = rows;
            FrameSize = new Point(texture.Width / columns, texture.Height / rows);
        }

        /// <summary>
        /// Creates a new <see cref="Jv.Games.Xna.Sprites.GridSpriteSheet"/>
        /// </summary>
        /// <param name="texture">Texture image containing the animations.</param>
        /// <param name="frameSize">The size in pixels of each image frame.</param>
        public GridSpriteSheet(Texture2D texture, Point frameSize)
            : base(texture)
        {
            if (frameSize.X <= 0 || frameSize.Y <= 0)
                throw new ArgumentOutOfRangeException("frameSize", "Frame size cannot be negative");
            if (texture.Width % frameSize.X != 0 || texture.Height % frameSize.Y != 0)
                throw new ArgumentException("Texture size does not match rows / columns");

            Columns = texture.Width / frameSize.X;
            Rows = texture.Height / frameSize.Y;
            FrameSize = frameSize;
        }

        /// <summary>
        /// Extracts a <see cref="Jv.Games.Xna.Sprites.Animation"/> with the specified frames.
        /// </summary>
        /// <param name="name">Name of the extracted animation.</param>
        /// <param name="frameIndexes">
        /// Zero based index of the frame, inside the sprite sheet grid.
        /// </param>
        /// <param name="frameDuration">How long each frame is displayed, before switching to the next frame.</param>
        /// <param name="repeat"><c>True</c> if the animation should repeat after the last frame finishes.</param>
        /// <returns>The extracted animation.</returns>
        public Animation GetAnimation(string name, int[] frameIndexes, TimeSpan frameDuration, bool repeat = true)
        {
            if (frameIndexes.Length <= 0 || frameIndexes.Any(index => index < 0 || index >= Columns * Rows))
                throw new ArgumentOutOfRangeException("frameIndexes");

            if (frameDuration <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("frameDuration");

            var frames = frameIndexes.Select(GetFrame).ToArray();
            var duration = TimeSpan.FromSeconds(frameDuration.TotalSeconds * frames.Length);
            if (repeat)
                return new LoopedAnimation(name, frames, duration, 0, null);
            return new Animation(name, frames, duration);
        }

        /// <summary>
        /// Extracts a <see cref="Jv.Games.Xna.Sprites.Animation"/> from a specified animation row.
        /// </summary>
        /// <param name="name">Name of the extracted animation.</param>
        /// <param name="line">Zero based index of the animation line.</param>
        /// <param name="count">How many sequential frames should be picked.</param>
        /// <param name="frameDuration">How long each frame is displayed, before switching to the next frame.</param>
        /// <param name="repeat"><c>True</c> if the animation should repeat after the last frame finishes.</param>
        /// <param name="skipFrames">How many frames should be ignored from the line, before starting to pick frames.</param>
        /// <returns>The extracted animation.</returns>
        public Animation GetAnimation(string name, int line, int count, TimeSpan frameDuration, bool repeat = true, int skipFrames = 0)
        {
            if (line < 0 || line > Rows)
                throw new ArgumentOutOfRangeException("line");

            if (count <= 0 || count > Columns)
                throw new ArgumentOutOfRangeException("count");

            if (skipFrames < 0)
                throw new ArgumentOutOfRangeException("skipFrames");

            var startIndex = (Texture.Width / FrameSize.X) * line + skipFrames;
            var indexes = Enumerable.Range(startIndex, count - skipFrames).ToArray();

            return GetAnimation(name, indexes, frameDuration, repeat);
        }

        /// <summary>
        /// Extracts a single frame from the sprite sheet.
        /// </summary>
        /// <param name="index">Zero based index of the frame, inside the sprite sheet grid.</param>
        /// <returns>The extracted frame.</returns>
        public Frame GetFrame(int index)
        {
            if (index < 0 || index >= Columns * Rows)
                throw new ArgumentOutOfRangeException("index");

            return new Frame (
                Texture,
                rectangle: new Rectangle(
                    (index % Columns) * FrameSize.X,
                    (index / Columns) * FrameSize.Y,
                    FrameSize.X,
                    FrameSize.Y)
				) {
				RenderScale = RenderScale
			};
        }
    }
}
