namespace Jv.Games.Xna.XForms
{
    using System;
    using Xamarin.Forms;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class ExportImageSourceHandlerAttribute : HandlerAttribute
    {
        public ExportImageSourceHandlerAttribute(Type imageSourceType, Type imageSourceHandlerType)
            : base(imageSourceType, imageSourceHandlerType)
        {
        }
    }
}
