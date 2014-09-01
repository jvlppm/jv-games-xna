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

        protected virtual void OnModelChanged(Element oldValue, Element newValue)
        {
        }

        #region IControlRenderer

        public Element Model
        {
            get { return (Element)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        public Vector2 DesiredSize { get; protected set; }

        public virtual void Initialize(Game game)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Measure(Vector2 availableSize)
        {
            DesiredSize = Vector2.Zero;
        }

        void IControlRenderer.Draw(SpriteBatch spriteBatch, GameTime gameTime, Microsoft.Xna.Framework.Rectangle area)
        {
            BeginDraw(spriteBatch);
            Draw(spriteBatch, gameTime, area);
            EndDraw(spriteBatch);
        }

        protected virtual void BeginDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null);
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime, Microsoft.Xna.Framework.Rectangle area)
        {
        }

        protected virtual void EndDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
        }

        #endregion
    }
}

