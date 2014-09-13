namespace Jv.Games.Xna.XForms
{
    using System;
    using Xamarin.Forms;
    using GameTime = Microsoft.Xna.Framework.GameTime;

    public class UIGameComponent : Microsoft.Xna.Framework.DrawableGameComponent, IPlatform
    {
        Page _page;
        IRenderer _renderer;
        object _bindingContext;

        public Xamarin.Forms.Rectangle? Area { get; set; }

        public UIGameComponent()
            : base(Forms.Game)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            if (_renderer != null)
            {
                _page.Layout(Area ?? Forms.Game.GraphicsDevice.Viewport.Bounds.ToXFormsRectangle());
                _renderer.Draw(gameTime);
            }
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (_renderer != null)
                _renderer.Update(gameTime);
            base.Update(gameTime);
        }

        public object BindingContext
        {
            get { return _bindingContext; }
            set
            {
                if (_bindingContext == value)
                    return;
                _bindingContext = value;
                if (BindingContextChanged != null)
                    BindingContextChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler BindingContextChanged;

        public IPlatformEngine Engine
        {
            get { return Forms.PlatformEngine; }
        }

        public Page Page { get { return _page; } }

        public void SetPage(Page newRoot)
        {
            _page = newRoot;
            _page.Platform = this;
            _renderer = RendererFactory.Create(newRoot);
        }
    }
}
