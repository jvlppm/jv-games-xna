[assembly: Jv.Games.Xna.XForms.ExportRenderer(
    typeof(Xamarin.Forms.Label),
    typeof(Jv.Games.Xna.XForms.Renderers.LabelRenderer))]
namespace Jv.Games.Xna.XForms.Renderers
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Xamarin.Forms;
    using Color = Microsoft.Xna.Framework.Color;
    using Rectangle = Xamarin.Forms.Rectangle;

    public class LabelRenderer : ViewRenderer
    {
        public new Label Model { get { return (Label)base.Model; } }

        public static SpriteFont DefaultFont;
        public static Color DefaultTextColor = Color.Black;

        ContentManager _content;
        SpriteFont _font;
        Color TextColor;

        Vector2 _textOffset;

        public override void Initialize(Game game)
        {
            _content = game.Content;
            HandleProperty(Label.FontProperty, HandleFont);
            HandleProperty(Label.TextColorProperty, HandleTextColor);
            HandleProperty(Label.TextProperty, HandleMeasurePropertyChanged);
            HandleProperty(Label.XAlignProperty, HandleArrangePropertyChanged);
            HandleProperty(Label.YAlignProperty, HandleArrangePropertyChanged);
            base.Initialize(game);
        }

        protected virtual bool HandleFont(BindableProperty prop)
        {
            if (Model.Font.FontFamily != null)
                _font = _content.Load<SpriteFont>(Model.Font.FontFamily);
            else
                _font = null;
            InvalidateMeasure();
            return true;
        }

        protected virtual bool HandleTextColor(BindableProperty prop)
        {
            if (Model.TextColor == default(Xamarin.Forms.Color))
                TextColor = LabelRenderer.DefaultTextColor;
            else
                TextColor = Model.TextColor.ToXnaColor();
            return true;
        }

        protected override Size MeasureContentOverride(Size availableSize)
        {
            if (_font == null)
                _font = LabelRenderer.DefaultFont;
            if (_font == null)
                return base.MeasureOverride(availableSize);
            var textMeasure = _font.MeasureString(Model.Text);

            return new Size(textMeasure.X, textMeasure.Y);
        }

        protected override Rectangle ArrangeOverride(Rectangle finalRect)
        {
            var renderArea = base.ArrangeOverride(finalRect);

            _textOffset = new Vector2(
                (float)GetAlignOffset(Model.XAlign, (float)ContentMeasuredSize.Width, (float)renderArea.Width),
                (float)GetAlignOffset(Model.YAlign, (float)ContentMeasuredSize.Height, (float)renderArea.Height));

            return renderArea;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawString(_font, Model.Text, _textOffset, TextColor, 0, Vector2.Zero, 1, 0, 0);
        }

        static float GetAlignOffset(TextAlignment alignment, float textSize, float renderSize)
        {
            switch (alignment)
            {
                case TextAlignment.Start:
                    return 0;
                case TextAlignment.Center:
                    return (renderSize - textSize) * 0.5f;
                case TextAlignment.End:
                    return (renderSize - textSize);
            }
            throw new System.NotImplementedException("Unsupported TextAlignment");
        }
    }
}
