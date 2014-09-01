[assembly: Jv.Games.Xna.XForms.ExportRenderer(
    typeof(Xamarin.Forms.View),
    typeof(Jv.Games.Xna.XForms.Renderers.ViewRenderer))]
namespace Jv.Games.Xna.XForms.Renderers
{
    using Xamarin.Forms;

    public class ViewRenderer : ViewRenderer<View>
    {

    }

    public class ViewRenderer<TModel> : VisualElementRenderer<TModel>
        where TModel : View
    {
    }
}
