/*[assembly: Jv.Games.Xna.XForms.ExportRenderer(
    typeof(Xamarin.Forms.Element),
    typeof(Jv.Games.Xna.XForms.Renderers.ElementRenderer))]
namespace Jv.Games.Xna.XForms.Renderers
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Xamarin.Forms;

    public class ElementRenderer : RendererBase
    {
        #region Handle Properties
        MultiValueDictionary<BindableProperty, Func<BindableProperty, bool>> Handlers;
        MultiValueDictionary<string, BindableProperty> HandledProperties;
        #endregion

        #region Constructors
        public ElementRenderer()
        {
            Handlers = StackDictionary<BindableProperty, Func<BindableProperty, bool>>.Create();
            HandledProperties = new MultiValueDictionary<string, BindableProperty>();
        }
        #endregion

        #region Methods
        protected override void OnModelChanged(Element oldValue, Element newValue)
        {
            base.OnModelChanged(oldValue, newValue);

            if (oldValue != null)
                UnloadModel(oldValue);

            if (newValue != null)
                LoadModel(newValue);
        }

        protected virtual void LoadModel(Element model)
        {
            model.PropertyChanged += OnPropertyChanged;
            foreach (var prop in Handlers)
                prop.Value.FirstOrDefault(handle => handle(prop.Key));
        }

        protected virtual void UnloadModel(Element model)
        {
            model.PropertyChanged -= OnPropertyChanged;
        }

        protected void HandleProperty(BindableProperty property, Func<BindableProperty, bool> handler)
        {
            HandledProperties.Add(property.PropertyName, property);
            Handlers.Add(property, p => true);
            Handlers.Add(property, handler);

            if (Model != null)
                handler(property);
        }

        protected virtual void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            IReadOnlyCollection<BindableProperty> handlers;
            if (HandledProperties.TryGetValue(e.PropertyName, out handlers))
                foreach (var property in handlers)
                    Handlers[property].FirstOrDefault(handle => handle(property));
        }
        #endregion
    }
}
*/