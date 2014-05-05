using Jv.Games.Xna.Async.Core;
using Microsoft.Xna.Framework;
using System;

namespace Jv.Games.Xna.Async
{
    public class AsyncGameComponent : IGameComponent, IDrawable, IUpdateable
    {
        #region Attributes
        public readonly AsyncContext DrawContext, UpdateContext;

        int _drawOrder, _updateOrder;
        bool _visible, _enabled;
        bool _initialized;
        #endregion

        #region Properties
        public int DrawOrder
        {
            get { return _drawOrder; }
            set
            {
                if (_drawOrder == value)
                    return;

                _drawOrder = value;

                if (DrawOrderChanged != null)
                    DrawOrderChanged(this, EventArgs.Empty);
            }
        }

        public int UpdateOrder
        {
            get { return _updateOrder; }
            set
            {
                if (_updateOrder == value)
                    return;

                _updateOrder = value;

                if (UpdateOrderChanged != null)
                    UpdateOrderChanged(this, EventArgs.Empty);
            }
        }

        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible == value)
                    return;

                _visible = value;

                if (VisibleChanged != null)
                    VisibleChanged(this, EventArgs.Empty);
            }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled == value)
                    return;

                _enabled = value;

                if (EnabledChanged != null)
                    EnabledChanged(this, EventArgs.Empty);
            }
        }

        protected Game Game { get; private set; }
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

            DrawContext = new AsyncContext();
            UpdateContext = new AsyncContext();

            Game = game;
        }

        protected virtual void Initialize() { }

        protected virtual void Draw(GameTime gameTime) { }

        protected virtual void Update(GameTime gameTime) { }

        void IGameComponent.Initialize()
        {
            if (!_initialized)
            {
                _initialized = true;
                UpdateContext.Send(Initialize);
            }
        }

        void IDrawable.Draw(GameTime gameTime)
        {
            DrawContext.Send(Draw, gameTime);
            DrawContext.Update(gameTime);
        }

        void IUpdateable.Update(GameTime gameTime)
        {
            UpdateContext.Send(Update, gameTime);
            UpdateContext.Update(gameTime);
        }
    }
}
