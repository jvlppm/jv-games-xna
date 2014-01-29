using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async
{
    public interface IActivity : IDrawable, IUpdateable
    {
        IActivity SubActivity { get; }
        bool IsTransparent { get; }
    }

    public abstract class Activity<T> : AsyncGameComponent, IActivity
    {
        protected readonly TaskCompletionSource<T> ActivityCompletion;

        public IActivity SubActivity { get; private set; }
        public bool IsTransparent { get; protected set; }

        public Activity(Game game)
            : base(game)
        {
            ActivityCompletion = new TaskCompletionSource<T>();
        }

        #region Game Loop
        override protected abstract void Draw(GameTime gameTime);

        override protected abstract void Update(GameTime gameTime);

        void IDrawable.Draw(GameTime gameTime)
        {
            if (SubActivity == null || SubActivity.RenderParent())
            {
                DrawContext.Send(Draw, gameTime);
                DrawContext.Update(gameTime);
            }

            if (SubActivity != null)
            {
                SubActivity.Draw(gameTime);
                if (SubActivity == null)
                    DrawContext.Update(gameTime);
            }
        }

        void IUpdateable.Update(GameTime gameTime)
        {
            if (SubActivity == null || SubActivity.RenderParent())
            {
                UpdateContext.Send(Update, gameTime);
                UpdateContext.Update(gameTime);
            }
            if (SubActivity != null)
            {
                SubActivity.Update(gameTime);
                if (SubActivity == null)
                    UpdateContext.Update(gameTime);
            }
        }
        #endregion

        protected Task<TActivity> Run<TActivity>(Activity<TActivity> level)
        {
            using (UpdateContext.Activate())
                Deactivating();

            SubActivity = level;

            using (level.UpdateContext.Activate())
            {
                var tcs = new TaskCompletionSource<TActivity>();

                Task<TActivity> runTask;
                bool started = false, activated = false;

                try
                {
                    level.Starting();
                    started = true;
                    level.Activating();
                    activated = true;
                    runTask = level.RunActivity();
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                    runTask = tcs.Task;
                }

                runTask.ContinueWith(t =>
                {
                    List<Exception> errors = new List<Exception>();
                    if (t.IsFaulted)
                        errors.AddRange(t.Exception.InnerExceptions);

                    using (level.UpdateContext.Activate())
                    {
                        if (activated)
                        {
                            try { level.Deactivating(); }
                            catch (Exception ex) { errors.Add(ex); }
                        }

                        if (started)
                        {
                            try { level.Completing(); }
                            catch (Exception ex) { errors.Add(ex); }
                        }
                    }

                    SubActivity = null;

                    using (UpdateContext.Activate())
                        Activating();

                    if (errors.Any())
                        tcs.TrySetException(new AggregateException(errors));
                    else
                        tcs.TrySetResult(t.Result);
                }, TaskContinuationOptions.ExecuteSynchronously);
                return tcs.Task;
            }
        }

        #region Life Cycle
        internal protected virtual Task<T> RunActivity()
        {
            return ActivityCompletion.Task;
        }
        protected void Exit(T result)
        {
            ActivityCompletion.SetResult(result);
        }
        internal protected virtual void Deactivating() { }
        internal protected virtual void Starting() { }
        internal protected virtual void Activating() { }
        internal protected virtual void Completing() { }
        #endregion
    }
}
