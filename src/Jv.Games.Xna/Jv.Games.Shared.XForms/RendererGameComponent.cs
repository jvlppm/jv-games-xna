namespace Jv.Games.Xna.XForms
{
    using System;
    using Xamarin.Forms;
    using Game = Microsoft.Xna.Framework.Game;
    using GameTime = Microsoft.Xna.Framework.GameTime;

    public class RendererGameComponent : Microsoft.Xna.Framework.DrawableGameComponent, IPlatform
    {
        Page _page;
        ElementView _view;
        object _bindingContext;

        public Xamarin.Forms.Rectangle Area
        {
            get { return Page.Bounds; }
            set { Page.Layout(value); }
        }

        public RendererGameComponent(Microsoft.Xna.Framework.Game game)
            : base(game)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            if (_view != null)
                _view.Draw(gameTime);
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (_view != null)
                _view.Update(gameTime);
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
            _view = new ElementView(Game, newRoot);
            Area = new Rectangle(Forms.Game.GraphicsDevice.Viewport.X, Forms.Game.GraphicsDevice.Viewport.Y, Forms.Game.GraphicsDevice.Viewport.Width, Forms.Game.GraphicsDevice.Viewport.Height);
        }
    }
}
