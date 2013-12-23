using Jv.Games.Xna.Async.Operations;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jv.Games.Xna.Async
{
    public class AsyncGameComponent<T> : IGameComponent, IDrawable, IUpdateable
    {
        #region Attributes
        public readonly AsyncContext DrawContext, UpdateContext;

        int _drawOrder, _updateOrder;
        bool _visible, _enabled;
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
        #endregion

        #region Events
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
        #endregion

        public AsyncGameComponent(Game game)
        {
            DrawContext = new AsyncContext();
            UpdateContext = new AsyncContext();
        }

        public void Initialize()
        {
            UpdateContext.Send((Action<AsyncContext>)Initialize);
        }

        public void Draw(GameTime gameTime)
        {
            DrawContext.Send(Draw, gameTime);
        }

        public void Update(GameTime gameTime)
        {
            UpdateContext.Send(Update, gameTime);
        }

        protected virtual void Initialize(AsyncContext context) { }

        protected virtual void Draw(AsyncContext context, GameTime gameTime) { }

        protected virtual void Update(AsyncContext context, GameTime gameTime) { }
    }
}
