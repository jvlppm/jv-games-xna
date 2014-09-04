﻿namespace Jv.Games.Xna.XForms
{
    using Microsoft.Xna.Framework;
    using Xamarin.Forms;

    public static class Forms
    {
        internal static bool IsInitialized;
        public static Game Game { get; private set; }

        public static void Init(Game game)
        {
#if PORTABLE
            throw new System.NotImplementedException();
#else
            if (IsInitialized)
                return;

            PlatformServices platformServices = new PlatformServices(game);

            game.Components.Add(platformServices);
            Device.PlatformServices = platformServices;

            RendererFactory.ScanForRenderers();

            Game = game;
            IsInitialized = true;
#endif
        }
    }
}