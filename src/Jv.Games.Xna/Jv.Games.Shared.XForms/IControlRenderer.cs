namespace Jv.Games.Xna.XForms
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Xamarin.Forms;

    public interface IControlRenderer : IRegisterable
    {
        Element Model { get; set; }
        IControlRenderer Parent { get; set; }

        Xamarin.Forms.Size MeasuredSize { get; }
        Xamarin.Forms.Rectangle RenderArea { get; }

        void Initialize(Game game);

        void Measure(Xamarin.Forms.Size availableSize);
        void Arrange(Xamarin.Forms.Rectangle givenArea);
        void InvalidateMeasure();
        void InvalidateArrange();

        void Draw(SpriteBatch spriteBatch, GameTime gameTime);
        void Update(GameTime gameTime);
    }
}
