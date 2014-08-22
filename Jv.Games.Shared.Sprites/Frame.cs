using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Jv.Games.Xna.Sprites
{
    /// <summary>
    /// Represents a single image inside a sprite sheet.
    /// </summary>
    public class Frame
    {
        /// <summary>
        /// Initializes a new <see cref="Jv.Games.Xna.Sprites.Frame"/> class.
        /// </summary>
        /// <param name="texture">Texture image used to render this frame.</param>
        /// <param name="rectangle">Region of the Frame inside the texture.</param>
        /// <param name="durationWeight">Frame duration weight.</param>
        public Frame(Texture2D texture, Rectangle rectangle, float durationWeight = 1)
        {
            Texture = texture;
            Rectangle = rectangle;
            DurationWeight = durationWeight;
        }

        /// <summary>
        /// Texture image used to render this frame.
        /// </summary>
        public Texture2D Texture { get; private set; }

        /// <summary>
        /// The frame's position inside the sprite sheet.
        /// </summary>
        public Rectangle Rectangle { get; private set; }

        /// <summary>
        /// The speed factor in which the annimation will be played.
        /// </summary>
        public float DurationWeight { get; private set; }

        /// <summary>
        /// The Width of this frame.
        /// </summary>
        public int Width { get { return Rectangle.Width; } }

        /// <summary>
        /// The Height of this frame.
        /// </summary>
        public int Height { get { return Rectangle.Height; } }
    }
}

