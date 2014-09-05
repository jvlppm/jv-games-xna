namespace Jv.Games.Xna.XForms
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Xamarin.Forms;
    using Rectangle=Xamarin.Forms.Rectangle;

    public interface IControlRenderer : IRegisterable
    {
        Element Model { get; set; }
        IControlRenderer Parent { get; set; }

        Size MeasuredSize { get; }
        Rectangle RenderArea { get; }

        void Initialize(Game game);

        void Measure(Size availableSize);
        void Arrange(Rectangle givenArea);
        void InvalidateMeasure();
        void InvalidateArrange();

        void Draw(SpriteBatch spriteBatch, GameTime gameTime);
        void Update(GameTime gameTime);
    }
}
