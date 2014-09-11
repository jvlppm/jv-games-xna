namespace Jv.Games.Xna.Sprites
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Animated game Sprite.
    /// </summary>
    public class Sprite : ICollection<Animation>
    {
        Dictionary<string, Animation> _animations;

        /// <summary>
        /// The color to tint the sprite. Use <c>Color.White</c> for full color with no tinting.
        /// Default is <c>Color.White</c>.
        /// </summary>
        public Color Color;
        /// <summary>
        /// The location (in screen coordinates) to draw the sprite.
        /// </summary>
        public Vector2 Position;
        /// <summary>
        /// Effects to apply when drawing the sprite.
        /// </summary>
        public SpriteEffects Effect;
        /// <summary>
        /// Specifies the angle (in radians) to rotate the sprite about its center.
        /// </summary>
        public float Rotation;

        /// <summary>
        /// Animation used to draw the sprite.
        /// </summary>
        public Animation CurrentAnimation { get; private set; }

        /// <summary>
        /// Indicates if the current animation is finished, or no animation is playing.
        /// </summary>
        public bool AnimationIsFinished { get { return CurrentAnimation == null || CurrentAnimation.IsFinished; } }

        /// <summary>
        /// Creates a new sprite, containing no animations.
        /// </summary>
        public Sprite()
        {
            _animations = new Dictionary<string, Animation>();
            Color = Color.White;
        }

        #region Game Loop
        /// <summary>
        /// Updates the current animation state.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (CurrentAnimation == null)
                return;
            CurrentAnimation.Update(gameTime);
        }

        /// <summary>
        /// Draws the current animation frame at the current <c>Position</c>.
        /// </summary>
        /// <param name="spriteBatch">
        /// Spritebatch used to draw the sprite.
        /// <c>spritebatch.Begin()</c> and <c>spritebatch.End()</c> should be called before and after this method call.
        /// </param>
        /// <param name="gameTime">Current game time.</param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (CurrentAnimation == null)
                return;
            CurrentAnimation.Draw(spriteBatch, Position, Color, Effect, Rotation);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Changes the currently playing animation.
        /// The selected animation will be played from beggining.
        /// If the animation is already playing nothing will be done.
        /// </summary>
        /// <param name="name">Name of the animation to be played.</param>
        public void PlayAnimation(string name)
        {
            if (!_animations.ContainsKey(name))
                throw new ArgumentException("Invalid animation name", "name");

            if (CurrentAnimation != null && CurrentAnimation.Name == name)
                return;

            CurrentAnimation = _animations[name];
            CurrentAnimation.Reset();
        }

        /// <summary>
        /// Changes the currently playing animation.
        /// The selected animation will be played from beggining.
        /// If the animation is already playing nothing will be done.
        /// </summary>
        /// <param name="animation">The animation to be played.</param>
        public void PlayAnimation(Animation animation)
        {
            if (animation == null)
                throw new ArgumentNullException("animation");

            if (CurrentAnimation == animation)
                return;

            CurrentAnimation = animation;
            CurrentAnimation.Reset();
        }
        #endregion

        #region region ICollection<Animation>
        /// <summary>
        /// Add an animation to this sprite.
        /// </summary>
        /// <param name="item"><c>Jv.Games.Xna.Sprites.Animation</c> to be added.</param>
        public void Add(Animation item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (_animations.ContainsKey(item.Name))
                throw new InvalidOperationException("Animation name must be unique");

            _animations.Add(item.Name, item);
            if (CurrentAnimation == null)
                CurrentAnimation = item;
        }

        /// <summary>
        /// Removes all registered animation.
        /// </summary>
        public void Clear()
        {
            _animations.Clear();
        }

        /// <summary>
        /// Test if an animation is registered in this sprite.
        /// </summary>
        /// <param name="item">Animation to test.</param>
        /// <returns><c>True</c> is this sprite contains the animation item.</returns>
        public bool Contains(Animation item)
        {
            return _animations.ContainsValue(item);
        }

        void ICollection<Animation>.CopyTo(Animation[] array, int arrayIndex)
        {
            _animations.Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns the number of animations in this <see cref="Jv.Games.Xna.Sprites.Sprite"/>.
        /// </summary>
        public int Count
        {
            get { return _animations.Count; }
        }

        bool ICollection<Animation>.IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes a single animation from the <see cref="Jv.Games.Xna.Sprites.Sprite"/>.
        /// </summary>
        /// <param name="item">Animation to be removed.</param>
        /// <returns><c>True</c> if the animation was removed.</returns>
        public bool Remove(Animation item)
        {
            if (!_animations.ContainsValue(item))
                return false;

            return _animations.Remove(item.Name);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the registered animations.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Animation> GetEnumerator()
        {
            return _animations.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _animations.Values.GetEnumerator();
        }
        #endregion
    }
}

