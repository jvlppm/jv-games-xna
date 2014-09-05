[assembly:Jv.Games.Xna.XForms.ExportRenderer(typeof(Sample.XForms.ImageButton), typeof(Sample.XForms.Renderers.ImageButtonRenderer))]
namespace Sample.XForms.Renderers
{
    using Jv.Games.Xna.XForms.Renderers;
    using Xamarin.Forms;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class ImageButtonRenderer : ViewRenderer
    {
        Texture2D _image;

        public new ImageButton Model { get { return (ImageButton)base.Model; } }

        public override void Initialize(Game game)
        {
            base.Initialize(game);
            HandleProperty(ImageButton.ImageProperty, ImageChanged);
        }

        bool ImageChanged(BindableProperty prop)
        {
            if (Model.Image == null)
                _image = null;
            else
                _image = Game.Content.Load<Texture2D>(Model.Image);
            return true;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_image == null)
                return base.MeasureOverride(availableSize);

            return new Size(_image.Width, _image.Height);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(_image, Vector2.Zero, Microsoft.Xna.Framework.Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            var mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
            if (mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed
                && RenderArea.Contains(new Xamarin.Forms.Point(mouse.X, mouse.Y)))
                Model.FireClicked();

            base.Update(gameTime);
        }
    }
}

