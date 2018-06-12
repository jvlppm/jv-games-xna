namespace Jv.Games.Xna.Async
{
    using System;
    using Microsoft.Xna.Framework;
    using Jv.Games.Xna.Context;

    public class AsyncGameComponent : IGameComponent, IDrawable, IUpdateable
    {
        #region Attributes

        public readonly Context DrawContext, UpdateContext;

        int _drawOrder, _updateOrder;
        bool _visible, _enabled;
        bool _initialized;

        #endregion

        #region Properties

        public int DrawOrder
        {
            get => _drawOrder;
            set
            {
                if (_drawOrder == value)
                    return;
                _drawOrder = value;
                DrawOrderChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int UpdateOrder
        {
            get => _updateOrder;
            set
            {
                if (_updateOrder == value)
                    return;
                _updateOrder = value;
                UpdateOrderChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool Visible
        {
            get => _visible;
            set
            {
                if (_visible == value)
                    return;
                _visible = value;
                VisibleChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled == value)
                    return;
                _enabled = value;
                EnabledChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        protected Game Game { get; }

        #endregion

        #region Events

        public event EventHandler<EventArgs> DrawOrderChanged;

        public event EventHandler<EventArgs> VisibleChanged;

        public event EventHandler<EventArgs> EnabledChanged;

        public event EventHandler<EventArgs> UpdateOrderChanged;

        #endregion

        public AsyncGameComponent(Game game)
        {
            _visible = true;
            _enabled = true;

            DrawContext = new Context();
            UpdateContext = new Context();

            Game = game;
        }

        protected virtual void Initialize()
        {
        }

        protected virtual void Draw(GameTime gameTime)
        {
        }

        protected virtual void Update(GameTime gameTime)
        {
        }

        void IGameComponent.Initialize()
        {
            if (!_initialized)
            {
                _initialized = true;
                Initialize();
            }
        }

        void IDrawable.Draw(GameTime gameTime)
        {
            Draw(gameTime);
            DrawContext.Update(gameTime);
        }

        void IUpdateable.Update(GameTime gameTime)
        {
            Update(gameTime);
            UpdateContext.Update(gameTime);
        }
    }
}
