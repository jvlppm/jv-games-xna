﻿namespace Jv.Games.Xna.XForms.Renderers
{
    using Xamarin.Forms;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;

    public class RendererBase : BindableObject, IControlRenderer
    {
        #region Static

        static BindableProperty ModelProperty = BindableProperty.Create<RendererBase, Element>(p => p.Model, null, propertyChanged: ModelPropertyChanged);

        static void ModelPropertyChanged(BindableObject bindable, Element oldValue, Element newValue)
        {
            var rend = bindable as RendererBase;
            if (rend != null)
                rend.OnModelChanged(oldValue, newValue);
        }

        #endregion

        #region Attributes
        Size? _lastAvailableSize;
        Xamarin.Forms.Rectangle? _lastArrangeArea;
        protected Game Game { get; private set; }
        #endregion

        #region Properties
        public Element Model
        {
            get { return (Element)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        public IControlRenderer Parent { get; set; }

        public Size MeasuredSize { get; private set; }
        public Xamarin.Forms.Rectangle RenderArea { get; private set; }
        #endregion

        #region Methods
        protected virtual void OnModelChanged(Element oldValue, Element newValue)
        {
        }

        public virtual void Initialize(Game game)
        {
            Game = game;
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Measure(Size availableSize)
        {
            if (_lastAvailableSize != availableSize)
            {
                MeasuredSize = MeasureOverride(availableSize);
                _lastAvailableSize = availableSize;
            }
        }

        protected virtual Size MeasureOverride(Size availableSize)
        {
            return Size.Zero;
        }

        public virtual void Arrange(Xamarin.Forms.Rectangle finalRect)
        {
            if(_lastArrangeArea != finalRect)
            {
                RenderArea = ArrangeOverride(finalRect);
                _lastArrangeArea = finalRect;
            }
        }

        protected virtual Xamarin.Forms.Rectangle ArrangeOverride(Xamarin.Forms.Rectangle finalRect)
        {
            return Xamarin.Forms.Rectangle.Zero;
        }

        public virtual void InvalidateMeasure()
        {
            _lastAvailableSize = null;
            MeasuredSize = Size.Zero;
            InvalidateArrange();
        }

        public virtual void InvalidateArrange()
        {
            _lastArrangeArea = null;
            RenderArea = Xamarin.Forms.Rectangle.Zero;
        }

        void IControlRenderer.Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            BeginDraw(spriteBatch);
            Draw(spriteBatch, gameTime);
            EndDraw(spriteBatch);
        }

        protected virtual void BeginDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null);
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
        }

        protected virtual void EndDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
        }
        #endregion
    }
}

