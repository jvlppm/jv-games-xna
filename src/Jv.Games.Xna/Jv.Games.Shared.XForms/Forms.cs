#if !PORTABLE
namespace Jv.Games.Xna.XForms
{
    using Microsoft.Xna.Framework;
    using Xamarin.Forms;

    public static class Forms
    {
        internal static bool IsInitialized;

        public static void Init(Game game)
        {
            if (IsInitialized)
                return;

            PlatformServices platformServices = new PlatformServices(game);

            game.Components.Add(platformServices);
            Device.PlatformServices = platformServices;

            RendererFactory.ScanForRenderers();

            IsInitialized = true;
        }
    }
}
#endif
