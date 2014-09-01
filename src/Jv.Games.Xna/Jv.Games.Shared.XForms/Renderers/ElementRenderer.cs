[assembly: Jv.Games.Xna.XForms.ExportRenderer(
    typeof(Xamarin.Forms.Element),
    typeof(Jv.Games.Xna.XForms.Renderers.ElementRenderer<Xamarin.Forms.Element>))]
namespace Jv.Games.Xna.XForms.Renderers
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Xamarin.Forms;

    public class ElementRenderer<TModel> : RendererBase
        where TModel : Element
    {
        #region Properties
        public new TModel Model
        {
            get { return (TModel)base.Model; }
            set { base.Model = value; }
        }
        #endregion

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
                UnloadModel((TModel)oldValue);

            if (newValue != null)
                LoadModel((TModel)newValue);
        }

        protected virtual void LoadModel(TModel model)
        {
            model.PropertyChanged += OnPropertyChanged;
            foreach (var prop in Handlers)
                prop.Value.FirstOrDefault(handle => handle(prop.Key));
        }

        protected virtual void UnloadModel(TModel model)
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
            foreach (var property in HandledProperties[e.PropertyName])
                Handlers[property].FirstOrDefault(handle => handle(property));
        }
        #endregion
    }
}
