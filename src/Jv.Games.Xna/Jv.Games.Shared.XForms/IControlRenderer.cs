namespace Jv.Games.Xna.XForms
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Xamarin.Forms;

    public interface IControlRenderer : IRegisterable
    {
        Element Model { get; set; }

        void Draw(SpriteBatch spriteBatch, GameTime gameTime);
        void Update(GameTime gameTime);
    }
}
