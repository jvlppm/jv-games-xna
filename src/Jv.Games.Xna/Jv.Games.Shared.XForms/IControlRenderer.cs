namespace Jv.Games.Xna.XForms
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Xamarin.Forms;

    public interface IControlRenderer : IRegisterable
    {
        Element Model { get; set; }
        Vector2 DesiredSize { get; }

        void Initialize(Game game);

        void Measure(Vector2 availableSize);
        void Draw(SpriteBatch spriteBatch, GameTime gameTime, Microsoft.Xna.Framework.Rectangle area);
        void Update(GameTime gameTime);
    }
}
