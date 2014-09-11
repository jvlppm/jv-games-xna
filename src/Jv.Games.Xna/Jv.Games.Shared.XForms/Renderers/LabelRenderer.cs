[assembly: Jv.Games.Xna.XForms.ExportRenderer(
    typeof(Xamarin.Forms.Label),
    typeof(Jv.Games.Xna.XForms.Renderers.LabelRenderer))]
namespace Jv.Games.Xna.XForms.Renderers
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using SpriteFont = Microsoft.Xna.Framework.Graphics.SpriteFont;
    using Color = Microsoft.Xna.Framework.Color;
    using Vector2 = Microsoft.Xna.Framework.Vector2;

    public class LabelRenderer : VisualElementRenderer<Label>
    {
        public static SpriteFont DefaultFont;
        public static Color DefaultTextColor = Color.Black;

        SpriteFont _font;
        Color _textColor;
        Vector2 _textOffset;
        SizeRequest _measuredSize;

        public LabelRenderer()
        {
            PropertyTracker.AddHandler(Label.TextColorProperty, Handle_TextColor);
            PropertyTracker.AddHandler(Label.FontProperty, Handle_Font);

            PropertyTracker.AddHandler(Label.TextProperty, Handle_ArrangeProperty);
            PropertyTracker.AddHandler(Label.XAlignProperty, Handle_ArrangeProperty);
            PropertyTracker.AddHandler(Label.YAlignProperty, Handle_ArrangeProperty);
        }

        public override SizeRequest Measure(Size availableSize)
        {
            var font = _font ?? DefaultFont;

            if (font == null || Model.Text == null)
                return base.Measure(availableSize);

            var textMeasure = font.MeasureString(Model.Text);
            _measuredSize = new SizeRequest(new Size(textMeasure.X, textMeasure.Y), default(Size));
            return _measuredSize;
        }

        protected override void LocalDraw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            var font = _font ?? DefaultFont;
            if (font == null || Model.Text == null)
                return;

            SpriteBatch.DrawString(font, Model.Text, _textOffset, _textColor);
        }

        protected override void Arrange()
        {
            UpdateAlignment();
            base.Arrange();
        }

        void UpdateAlignment()
        {
            _textOffset = new Microsoft.Xna.Framework.Vector2(
                (float)GetAlignOffset(Model.XAlign, (float)_measuredSize.Request.Width, (float)Model.Bounds.Width),
                (float)GetAlignOffset(Model.YAlign, (float)_measuredSize.Request.Height, (float)Model.Bounds.Height));
        }

        #region Property Handlers
        void Handle_TextColor(BindableProperty property)
        {
            if (Model.TextColor == default(Xamarin.Forms.Color))
                _textColor = LabelRenderer.DefaultTextColor;
            else
                _textColor = Model.TextColor.ToXnaColor();

            UpdateAlignment();
        }

        void Handle_Font(BindableProperty property)
        {
            if (Model.Font.FontFamily != null)
                _font = Forms.Game.Content.Load<SpriteFont>(Model.Font.FontFamily);
            else
                _font = null;

            UpdateAlignment();
        }

        void Handle_ArrangeProperty(BindableProperty property)
        {
            UpdateAlignment();
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
        #endregion
    }
}
