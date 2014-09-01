[assembly: Jv.Games.Xna.XForms.ExportRenderer(
    typeof(Xamarin.Forms.Element),
    typeof(Jv.Games.Xna.XForms.Renderers.ElementRenderer<Xamarin.Forms.Element>))]
namespace Jv.Games.Xna.XForms.Renderers
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text;
    using Xamarin.Forms;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;

    public class ElementRenderer<TModel> : BindableObject, IControlRenderer
        where TModel : Element
    {
        #region Bindable Properties
        static BindableProperty ModelProperty = BindableProperty.Create<ElementRenderer<TModel>, TModel>(p => p.Model, null, propertyChanged: ModelPropertyChanged);
        public TModel Model
        {
            get { return (TModel)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }
        static void ModelPropertyChanged(BindableObject bindable, TModel oldValue, TModel newValue)
        {
            var rend = bindable as ElementRenderer<TModel>;
            if (rend == null)
                return;

            var oldModel = oldValue as TModel;
            if (oldModel != null)
                rend.UnloadModel(oldModel);

            var newModel = newValue as TModel;
            if (newModel != null)
                rend.LoadModel(newModel);
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

        #region IControlRenderer
        Element IControlRenderer.Model
        {
            get { return Model; }
            set { Model = (TModel)value; }
        }

        public virtual void Initialize(Microsoft.Xna.Framework.Game game)
        {
        }

        public virtual void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Microsoft.Xna.Framework.GameTime gameTime)
        {
        }

        public virtual void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
        }

        void IControlRenderer.Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, GameTime gameTime)
        {
            BeginDraw(spriteBatch);
            Draw(spriteBatch, gameTime);
            EndDraw(spriteBatch);
        }

        protected virtual void BeginDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null);
        }

        protected virtual void EndDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            spriteBatch.End();
        }
        #endregion
    }
}
