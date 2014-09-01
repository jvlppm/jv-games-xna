namespace Jv.Games.Xna.XForms
{
    using Microsoft.Xna.Framework;

    public static class Extensions
    {
        public static Color ToXnaColor(this Xamarin.Forms.Color color)
        {
            return new Color((float)color.R, (float)color.G, (float)color.B, (float)color.A);
        }
    }
}
