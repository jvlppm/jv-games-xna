[assembly: Jv.Games.Xna.XForms.ExportRenderer(
    typeof(Xamarin.Forms.Image),
    typeof(Jv.Games.Xna.XForms.Renderers.ImageRenderer))]
namespace Jv.Games.Xna.XForms.Renderers
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using Texture2D=Microsoft.Xna.Framework.Graphics.Texture2D;

    public class ImageRenderer : VisualElementRenderer<Image>
    {
        CancellationTokenSource _imageLoadCancellation;
        Texture2D _image;

        public ImageRenderer()
        {
            PropertyTracker.AddHandler(Image.SourceProperty, Handle_Source);
        }

        public override SizeRequest Measure(Size availableSize)
        {
            if (_image == null)
                return default(SizeRequest);

            if(double.IsPositiveInfinity(availableSize.Width))
                availableSize.Width = _image.Width;
            if (double.IsPositiveInfinity(availableSize.Height))
                availableSize.Height = _image.Height;

            switch (Model.Aspect)
            {
                case Aspect.Fill:
                    return new SizeRequest(availableSize, default(Size));
                case Aspect.AspectFit:
                    var scaleFit = Math.Min(availableSize.Width / (float)_image.Width, availableSize.Height / (float)_image.Height);
                    return new SizeRequest(new Size(_image.Width * scaleFit, _image.Height * scaleFit), default(Size));
                case Aspect.AspectFill:
                    var scaleFill = Math.Max(availableSize.Width / (float)_image.Width, availableSize.Height / (float)_image.Height);
                    return new SizeRequest(new Size(_image.Width * scaleFill, _image.Height * scaleFill), default(Size));
            }

            throw new NotImplementedException();
        }

        protected override void LocalDraw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if(_image == null)
                return;
            SpriteBatch.Draw(_image, Microsoft.Xna.Framework.Vector2.Zero, Microsoft.Xna.Framework.Color.White);
        }

        #region Property Handlers
        protected virtual bool Handle_Source(BindableProperty prop)
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
                    _image = Forms.Game.Content.Load<Texture2D>(fileSource.File);
                    //InvalidateMeasure();
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
                        //InvalidateMeasure();
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
                            _image = Texture2D.FromStream(Forms.Game.GraphicsDevice, t.Result);
                        //InvalidateMeasure();
                        Model.IsLoading = false;
                        _imageLoadCancellation = null;
                    });
                });
            }

            return true;
        }
        #endregion
    }
}
