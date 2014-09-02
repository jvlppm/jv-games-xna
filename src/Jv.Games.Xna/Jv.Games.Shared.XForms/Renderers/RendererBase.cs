namespace Jv.Games.Xna.XForms.Renderers
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
        Vector2? _lastAvailableSize;
        #endregion

        #region Properties
        public Element Model
        {
            get { return (Element)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        public Vector2 DesiredSize { get; private set; }
        #endregion

        #region Methods
        protected virtual void OnModelChanged(Element oldValue, Element newValue)
        {
        }

        public virtual void Initialize(Game game)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public void Measure(Vector2 availableSize)
        {
            if (_lastAvailableSize != availableSize)
            {
                DesiredSize = MeasureOverride(availableSize);
                _lastAvailableSize = availableSize;
            }
        }

        protected virtual Vector2 MeasureOverride(Vector2 availableSize)
        {
            return Vector2.Zero;
        }

        void IControlRenderer.Draw(SpriteBatch spriteBatch, GameTime gameTime, Microsoft.Xna.Framework.Rectangle area)
        {
            BeginDraw(spriteBatch, area);
            Draw(spriteBatch, gameTime, area);
            EndDraw(spriteBatch, area);
        }

        protected virtual void BeginDraw(SpriteBatch spriteBatch, Microsoft.Xna.Framework.Rectangle area)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null);
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime, Microsoft.Xna.Framework.Rectangle area)
        {
        }

        protected virtual void EndDraw(SpriteBatch spriteBatch, Microsoft.Xna.Framework.Rectangle area)
        {
            spriteBatch.End();
        }
        #endregion
    }
}

