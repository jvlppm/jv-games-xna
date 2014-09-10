namespace Jv.Games.Xna.XForms
{
    using Microsoft.Xna.Framework.Graphics;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    public interface IImageSourceHandler : IRegisterable
    {
        Task<Texture2D> LoadImageAsync(ImageSource imageSource, CancellationToken cancellationToken);
    }
}
