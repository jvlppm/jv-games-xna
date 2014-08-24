namespace Jv.Games.Xna.Sprites
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Linq;

    public class SpriteSheet
    {
        public Texture2D Texture { get; private set; }
        public int Columns { get; private set; }
        public int Rows { get; private set; }
        public Point FrameSize { get; private set; }

        public SpriteSheet(Texture2D texture, int columns, int rows)
        {
            if (texture == null)
                throw new ArgumentNullException("texture");
            if (columns <= 0)
                throw new ArgumentOutOfRangeException("columns");
            if (rows <= 0)
                throw new ArgumentOutOfRangeException("rows");

            if (texture.Width % columns != 0 || texture.Height % rows != 0)
                throw new ArgumentException("Texture size does not match rows / columns");

            Texture = texture;
            Columns = columns;
            Rows = rows;
            FrameSize = new Point(texture.Width / columns, texture.Height / rows);
        }

        public SpriteSheet(Texture2D texture, Point frameSize)
        {
            if (texture == null)
                throw new ArgumentNullException("texture");
            if (frameSize.X <= 0 || frameSize.Y <= 0)
                throw new ArgumentOutOfRangeException("frameSize", "Frame size cannot be negative");

            if (texture.Width % frameSize.X != 0 || texture.Height % frameSize.Y != 0)
                throw new ArgumentException("Texture size does not match rows / columns");

            Texture = texture;
            Columns = texture.Width / frameSize.X;
            Rows = texture.Height / frameSize.Y;
            FrameSize = frameSize;
        }

        public Animation GetAnimation(string name, Rectangle[] frameRects, TimeSpan duration, bool repeat = true)
        {
            var frames = frameRects.Select(r => new Frame(Texture, r)).ToArray();
            if (repeat)
                return new LoopedAnimation(name, frames, duration, 0, null);
            return new Animation(name, frames, duration);
        }

        public Animation GetAnimation(string name, int[] frameIndexes, TimeSpan duration, bool repeat = true)
        {
            var frames = frameIndexes.Select(GetFrame).ToArray();
            if (repeat)
                return new LoopedAnimation(name, frames, duration, 0, null);
            return new Animation(name, frames, duration);
        }

        public Animation GetAnimation(string name, int line, int count, TimeSpan frameDuration, bool repeat = true, int skipFrames = 0)
        {
            if (FrameSize == default(Point))
                throw new InvalidOperationException("No FrameSize was specified");

            var startIndex = (Texture.Width / FrameSize.X) * line + skipFrames;
            var indexes = Enumerable.Range(startIndex, count - skipFrames).ToArray();

            return GetAnimation(name, indexes, frameDuration, repeat);
        }

        public Frame GetFrame(int index)
        {
            if (FrameSize == default(Point))
                throw new InvalidOperationException("No FrameSize was specified");

            return new Frame
            (
                Texture,
                rectangle: new Rectangle(
                    (index % Columns) * FrameSize.X,
                    (index / Columns) * FrameSize.Y,
                    FrameSize.X,
                    FrameSize.Y)
            );
        }
    }
}
