namespace Jv.Games.Xna.XForms
{
    using System;
    using Xamarin.Forms;
    using Game = Microsoft.Xna.Framework.Game;
    using GameTime = Microsoft.Xna.Framework.GameTime;

    public class RendererGameComponent : Microsoft.Xna.Framework.DrawableGameComponent, IPlatform
    {
        ElementView _view;
        object _bindingContext;
        public Xamarin.Forms.Page PageRoot { get; private set; }

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

        public Page Page
        {
            get { return PageRoot; }
        }

        public void SetPage(Page newRoot)
        {
            _view = new ElementView(Game, newRoot);
            PageRoot = newRoot;
            PageRoot.Platform = this;
            newRoot.Layout(new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height));
        }
    }
}
