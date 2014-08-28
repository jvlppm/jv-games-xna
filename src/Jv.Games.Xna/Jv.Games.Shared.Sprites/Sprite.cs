namespace Jv.Games.Xna.Sprites
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;

    public class Sprite : ICollection<Animation>
    {
        Dictionary<string, Animation> _animations;

        public Color Color;
        public Vector2 Position;
        public SpriteEffects Effect;
        public float Rotation;

        public Animation CurrentAnimation { get; private set; }
        public bool AnimationIsFinished { get { return CurrentAnimation == null || CurrentAnimation.IsFinished; } }

        public Sprite()
        {
            _animations = new Dictionary<string, Animation>();
            Color = Color.White;
        }

        #region Game Loop
        public void Update(GameTime gameTime)
        {
            if (CurrentAnimation == null)
                return;
            CurrentAnimation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (CurrentAnimation == null)
                return;
            CurrentAnimation.Draw(spriteBatch, Position, Color, Effect, Rotation);
        }
        #endregion

        #region Public Methods
        public void PlayAnimation(string name)
        {
            if (!_animations.ContainsKey(name))
                throw new ArgumentException("Invalid animation name", "name");
            CurrentAnimation = _animations[name];
            CurrentAnimation.Reset();
        }

        public void PlayAnimation(Animation animation)
        {
            if (animation == null)
                throw new ArgumentNullException("animation");

            CurrentAnimation = animation;
            CurrentAnimation.Reset();
        }
        #endregion

        #region region ICollection<Animation>
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

        public void Clear()
        {
            _animations.Clear();
        }

        public bool Contains(Animation item)
        {
            return _animations.ContainsValue(item);
        }

        public void CopyTo(Animation[] array, int arrayIndex)
        {
            _animations.Values.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _animations.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Animation item)
        {
            if (!_animations.ContainsValue(item))
                return false;

            return _animations.Remove(item.Name);
        }

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

