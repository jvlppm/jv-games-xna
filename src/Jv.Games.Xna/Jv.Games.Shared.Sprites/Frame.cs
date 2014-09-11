namespace Jv.Games.Xna.Sprites
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    /// <summary>
    /// Represents a single image inside a sprite sheet.
    /// </summary>
    public class Frame
    {
        /// <summary>
        /// The origin of the sprite.
        /// </summary>
        public Vector2 Origin;

        /// <summary>
        /// Texture image used to render this frame.
        /// </summary>
        public readonly Texture2D Texture;

        /// <summary>
        /// The frame's position inside the sprite sheet.
        /// </summary>
        public readonly Rectangle Rectangle;

        /// <summary>
        /// The speed factor in which the annimation will be played.
        /// </summary>
        public float DurationWeight;

        /// <summary>
        /// The Width of this frame.
        /// </summary>
        public int Width { get { return Rectangle.Width; } }

        /// <summary>
        /// The Height of this frame.
        /// </summary>
        public int Height { get { return Rectangle.Height; } }

        /// <summary>
        /// Initializes a new <see cref="Jv.Games.Xna.Sprites.Frame"/> class.
        /// </summary>
        /// <param name="texture">Texture image used to render this frame.</param>
        /// <param name="rectangle">Region of the Frame inside the texture.</param>
        /// <param name="origin">/// The origin of the sprite. Specify (0,0) for the upper-left corner.</param>
        /// <param name="durationWeight">Frame duration weight.</param>
        public Frame(Texture2D texture, Rectangle rectangle, Vector2 origin = default(Vector2), float durationWeight = 1)
        {
            if (rectangle.Width <= 0 || rectangle.Height <= 0)
                throw new ArgumentException("Invalid frame rectangle", "rectangle");

            if (durationWeight < 0)
                throw new ArgumentOutOfRangeException("durationWeight", "Duration weight cannot be negative");

            Texture = texture;
            Rectangle = rectangle;
            Origin = origin;
            DurationWeight = durationWeight;
        }
    }
}

