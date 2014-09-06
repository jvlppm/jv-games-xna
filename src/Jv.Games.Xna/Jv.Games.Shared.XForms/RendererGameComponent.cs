namespace Jv.Games.Xna.XForms
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class RendererGameComponent : DrawableGameComponent
    {
        public Xamarin.Forms.Rectangle? Area;
        readonly SpriteBatch _spriteBatch;
        public readonly IControlRenderer Renderer;

        public RendererGameComponent(Game game, IControlRenderer renderer)
            : base(game)
        {
            Renderer = renderer;
            _spriteBatch = new SpriteBatch(game.GraphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            var area = Area ?? new Xamarin.Forms.Rectangle(
                Game.GraphicsDevice.Viewport.X,
                Game.GraphicsDevice.Viewport.Y,
                Game.GraphicsDevice.Viewport.Width,
                Game.GraphicsDevice.Viewport.Height);

            Renderer.Measure(area.Size);
            Renderer.Arrange(area);
            Renderer.Draw(_spriteBatch, gameTime);

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
