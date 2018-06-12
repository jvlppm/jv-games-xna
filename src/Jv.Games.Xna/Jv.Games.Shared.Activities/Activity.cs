namespace Jv.Games.Xna.Async
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IActivityStackItem : IDrawable, IUpdateable
    {
        IActivityStackItem SubActivity { get; }
        bool IsTransparent { get; }
    }

    public class ActivityBase : AsyncGameComponent, IActivityStackItem
    {
        public IActivityStackItem SubActivity { get; private set; }
        public bool IsTransparent { get; protected set; }

        public ActivityBase(Game game)
            : base(game)
        {
        }

        #region Game Loop
        void IUpdateable.Update(GameTime gameTime)
        {
            UpdateActivity(gameTime);
        }

        protected virtual void UpdateActivity(GameTime gameTime)
        {
            if (SubActivity == null || SubActivity.AllTransparent())
            {
                try
                {
                    Update(gameTime);
                }
                finally
                {
                    UpdateContext.Update(gameTime);
                }
            }
            if (SubActivity != null)
            {
                SubActivity.Update(gameTime);
                if (SubActivity == null)
                    UpdateContext.Update(gameTime);
            }
        }

        void IDrawable.Draw(GameTime gameTime)
        {
            DrawActivity(gameTime);
        }

        protected virtual void DrawActivity(GameTime gameTime)
        {
            if (SubActivity == null || SubActivity.AllTransparent())
            {
                try
                {
                    Draw(gameTime);
                }
                finally
                {
                    DrawContext.Update(gameTime);
                }
            }

            if (SubActivity != null)
            {
                SubActivity.Draw(gameTime);
                if (SubActivity == null)
                    DrawContext.Update(gameTime);
            }
        }
        #endregion

        protected Task<TResult> Run<TResult>(ActivityBase activity, Func<Task<TResult>> activityRun)
        {
            if (activityRun == null)
                throw new ArgumentNullException("activityRun");

            if (SubActivity != null)
                throw new InvalidOperationException("Activity is already running another sub-activity");

            bool deactivated = false;
            try
            {
                SubActivity = activity;

                Deactivating();
                deactivated = true;

                ((IGameComponent)activity).Initialize();
            }
            catch (Exception ex)
            {
                try
                {
                    if (deactivated)
                        Activating();
                }
                catch (Exception reactivateEx)
                {
                    throw new AggregateException(ex, reactivateEx);
                }

                SubActivity = null;
                throw;
            }

            var tcs = new TaskCompletionSource<TResult>();

            Task<TResult> runTask;
            bool started = false, activated = false;

            try
            {
                activity.Starting();
                started = true;
                activity.Activating();
                activated = true;
                runTask = activityRun();
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

                if (activated)
                {
                    try { activity.Deactivating(); }
                    catch (Exception ex) { errors.Add(ex); }
                }

                if (started)
                {
                    try { activity.Completing(); }
                    catch (Exception ex) { errors.Add(ex); }
                }

                SubActivity = null;

                Activating();

                if (errors.Any())
                    tcs.TrySetException(new AggregateException(errors).Flatten());
                else
                    tcs.TrySetResult(t.Result);
            }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        #region Life Cycle
        protected virtual void Deactivating() { }
        protected virtual void Starting() { }
        protected virtual void Activating() { }
        protected virtual void Completing() { }
        #endregion
    }

    public class Activity<T> : ActivityBase
    {
        protected readonly TaskCompletionSource<T> ActivityCompletion;
        protected readonly CancellationToken CancelOnExit;

        protected SpriteBatch SpriteBatch { get; }

        protected ContentManager Content => Game.Content;
        protected GraphicsDevice GraphicsDevice => Game.GraphicsDevice;
        protected Viewport Viewport => Game.GraphicsDevice.Viewport;

        protected Activity(Game game)
            : base(game)
        {
            ActivityCompletion = new TaskCompletionSource<T>();
            var cts = new CancellationTokenSource();
            ActivityCompletion.Task.ContinueWith(t => cts.Cancel());
            CancelOnExit = cts.Token;
            SpriteBatch = new SpriteBatch(game.GraphicsDevice);
        }

        #region Life Cycle
        protected virtual Task<T> Run()
        {
            return ActivityCompletion.Task;
        }

        protected void Exit(T result)
        {
            ActivityCompletion.TrySetResult(result);
        }
        #endregion

        #region Public Methods
        public ContextTask<TResult> Run<TResult>(Activity<TResult> subActivity)
        {
            return UpdateContext.Wait(Run(subActivity, subActivity.Run));
        }
        #endregion
    }
}
