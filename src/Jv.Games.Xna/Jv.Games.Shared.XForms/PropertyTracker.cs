﻿namespace Jv.Games.Xna.XForms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xamarin.Forms;

    public delegate bool PropertyHandler(BindableProperty property);

    public class PropertyTracker
    {
        BindableObject _target;
        readonly Dictionary<string, List<BindableProperty>> PropertiesByName;
        readonly Dictionary<BindableProperty, Stack<PropertyHandler>> PropertyHandlers;

        public PropertyTracker()
        {
            PropertiesByName = new Dictionary<string, List<BindableProperty>>();
            PropertyHandlers = new Dictionary<BindableProperty, Stack<PropertyHandler>>();
        }

        public void SetTarget(BindableObject obj)
        {
            if (_target == obj)
                return;

            if (_target != null)
                _target.PropertyChanged -= Target_PropertyChanged;

            _target = obj;
            if (_target != null)
            {
                _target.PropertyChanged += Target_PropertyChanged;
                foreach (var prop in PropertyHandlers)
                    NotifyPropertyChanged(prop.Key);
            }
        }

        public void AddHandler(BindableProperty property, PropertyHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException();

            List<BindableProperty> propStore;
            if (!PropertiesByName.TryGetValue(property.PropertyName, out propStore))
            {
                propStore = new List<BindableProperty> { property };
                PropertiesByName.Add(property.PropertyName, propStore);
            }
            else
            {
                if (!propStore.Contains(property))
                    propStore.Add(property);
            }

            Stack<PropertyHandler> handlStore;
            if(!PropertyHandlers.TryGetValue(property, out handlStore))
            {
                handlStore = new Stack<PropertyHandler>();
                PropertyHandlers.Add(property, handlStore);
            }

            handlStore.Push(handler);

            if (_target != null)
                handler(property);
        }

        void Target_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            List<BindableProperty> possibleProperties;
            if (!PropertiesByName.TryGetValue(e.PropertyName, out possibleProperties))
                return;

            foreach (var prop in possibleProperties)
                NotifyPropertyChanged(prop);
        }

        void NotifyPropertyChanged(BindableProperty property)
        {
            Stack<PropertyHandler> handlers;
            if (!PropertyHandlers.TryGetValue(property, out handlers))
                return;

            handlers.FirstOrDefault(h => h(property));
        }
    }
}