[assembly: Jv.Games.Xna.XForms.ExportRenderer(
    typeof(Xamarin.Forms.Cell),
    typeof(Jv.Games.Xna.XForms.Renderers.CellRenderer))]
namespace Jv.Games.Xna.XForms.Renderers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Xamarin.Forms;

    public class CellRenderer : ICellRenderer
    {
        #region Static
        static BindableProperty RendererProperty = BindableProperty.CreateAttached("Renderer", typeof(ICellRenderer), typeof(CellRenderer), null);

        public static ICellRenderer GetRenderer(BindableObject obj)
        {
            return (ICellRenderer)obj.GetValue(RendererProperty);
        }
        public static void SetRenderer(Element obj, ICellRenderer renderer)
        {
            obj.SetValue(RendererProperty, renderer);
        }

        public static ICellRenderer Create(Cell element)
        {
            if (!Forms.IsInitialized)
                throw new InvalidOperationException("Xamarin.Forms not initialized");

            if (element == null)
                throw new NotImplementedException();

            var renderer = Registrar.Registered.GetHandler<ICellRenderer>(element.GetType());
            if (renderer != null)
            {
                SetRenderer(element, renderer);
                renderer.Model = element;
            }
            return renderer;
        }
        #endregion

        Cell _model;

        protected readonly PropertyTracker PropertyTracker;

        public CellRenderer()
        {
            PropertyTracker = new PropertyTracker();
        }

        public Cell Model
        {
            get { return _model; }
            set
            {
                if (_model == value)
                    return;

                if (_model != null)
                    OnModelUnload(_model);

                _model = value;

                if (_model != null)
                    OnModelLoad(_model);

                PropertyTracker.SetTarget(value);
            }
        }

        public virtual VisualElement CreateVisual(object item)
        {
            return new Label { Text = (item ?? "").ToString(), Parent = Model };
        }

        protected virtual void OnModelUnload(Cell model)
        {
        }

        protected virtual void OnModelLoad(Cell model)
        {
        }
    }
}
