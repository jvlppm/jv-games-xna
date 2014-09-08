[assembly: Jv.Games.Xna.XForms.ExportRenderer(typeof(Xamarin.Forms.Image), typeof(Jv.Games.Xna.XForms.Renderers.ImageRenderer))]
namespace Jv.Games.Xna.XForms.Renderers
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    class ImageRenderer : ViewRenderer
    {
        public new Image Model { get { return (Image)base.Model; } }

        CancellationTokenSource _imageLoadCancellation;
        Texture2D _image;
        Microsoft.Xna.Framework.Rectangle ImageArea;

        public override void Initialize(Microsoft.Xna.Framework.Game game)
        {
            base.Initialize(game);
            HandleProperty(Image.SourceProperty, HandleImageProperty);
        }

        public override void Arrange(Xamarin.Forms.Rectangle finalRect)
        {
            base.Arrange(finalRect);
            switch (Model.Aspect)
            {
                case Aspect.Fill:
                    ImageArea = RenderArea;
                    break;
                case Aspect.AspectFit:
                    var scaleFit = Math.Min(ArrangedArea.Width / (float)_image.Width, ArrangedArea.Height / (float)_image.Height);
                    ImageArea = new Microsoft.Xna.Framework.Rectangle(0, 0, (int)(_image.Width * scaleFit), (int)(_image.Height * scaleFit));
                    break;
                case Aspect.AspectFill:
                    var scaleFill = Math.Max(ArrangedArea.Width / (float)_image.Width, ArrangedArea.Height / (float)_image.Height);
                    ImageArea = new Microsoft.Xna.Framework.Rectangle(0, 0, (int)(_image.Width * scaleFill), (int)(_image.Height * scaleFill));
                    break;
            }
        }

        protected virtual bool HandleImageProperty(BindableProperty prop)
        {
            if (_imageLoadCancellation != null)
                _imageLoadCancellation.Cancel();
            _imageLoadCancellation = new CancellationTokenSource();

            Task<Stream> getStream = null;

            var fileSource = Model.Source as FileImageSource;

            if (fileSource != null)
            {
                try
                {
                    _image = Game.Content.Load<Texture2D>(fileSource.File);
                    InvalidateMeasure();
                    _imageLoadCancellation = null;
                    return true;
                }
                catch
                {
#if !PORTABLE
                    if (File.Exists(fileSource.File))
                        getStream = Task.FromResult((Stream)File.OpenRead(fileSource.File));
                    else
#endif
                    {
                        _image = null;
                        InvalidateMeasure();
                        return true;
                    }
                }
            }

            var streamSource = Model.Source as StreamImageSource;
            var uriSource = Model.Source as UriImageSource;
            if (streamSource != null)
                getStream = streamSource.Stream(_imageLoadCancellation.Token);
            else if (uriSource != null)
                getStream = uriSource.GetStreamAsync(_imageLoadCancellation.Token);

            if (getStream != null)
            {
                Model.IsLoading = true;
                getStream.ContinueWith(t =>
                {
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(delegate
                    {
                        if (t.Result == null)
                            _image = null;
                        else
                            _image = Texture2D.FromStream(Game.GraphicsDevice, t.Result);
                        InvalidateMeasure();
                        Model.IsLoading = false;
                        _imageLoadCancellation = null;
                    });
                });
            }

            return true;
        }

        protected override Size MeasureContentOverride(Size availableSize)
        {
            if (_image == null)
                return base.MeasureContentOverride(availableSize);
            return new Size(_image.Width, _image.Height);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (_image == null)
                return;
            spriteBatch.Draw(_image, ImageArea, Microsoft.Xna.Framework.Color.White);
        }
    }
}
