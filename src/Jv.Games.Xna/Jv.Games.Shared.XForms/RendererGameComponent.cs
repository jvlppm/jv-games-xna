namespace Jv.Games.Xna.XForms
{
    using Microsoft.Xna.Framework;
    using System;

    public class RendererGameComponent : DrawableGameComponent, Xamarin.Forms.IPlatform
    {
        ElementView _view;
        public Xamarin.Forms.Page PageRoot { get; private set; }

        public RendererGameComponent(Game game)
            : base(game)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            if(_view != null)
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
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler BindingContextChanged;

        public Xamarin.Forms.IPlatformEngine Engine
        {
            get { return Forms.PlatformEngine; }
        }

        public Xamarin.Forms.Page Page
        {
            get { return PageRoot; }
        }

        public void SetPage(Xamarin.Forms.Page newRoot)
        {
            _view = new ElementView(Game, newRoot);
            PageRoot = newRoot;
            PageRoot.Platform = this;
            newRoot.Layout(new Xamarin.Forms.Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height));
        }
    }
}
