using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async
{
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
        #endregion

        internal Task<TActivity> Run<TActivity>(ActivityBase activity, Func<Task<TActivity>> runActivity)
        {
            using (UpdateContext.Activate())
                Deactivating();

            SubActivity = activity;
            ((IGameComponent)activity).Initialize();

            using (activity.UpdateContext.Activate())
            {
                var tcs = new TaskCompletionSource<TActivity>();

                Task<TActivity> runTask;
                bool started = false, activated = false;

                try
                {
                    activity.Starting();
                    started = true;
                    activity.Activating();
                    activated = true;
                    runTask = runActivity();
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

                    using (activity.UpdateContext.Activate())
                    {
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

        protected Task<TActivity> Run<TActivity>(Activity<TActivity> level)
        {
            return Run(level, level.RunActivity);
        }

        protected Task Run(Activity level)
        {
            return Run(level, () => level.RunActivity().ContinueWith(t => true));
        }

        #region Life Cycle
        protected virtual void Deactivating() { }
        protected virtual void Starting() { }
        protected virtual void Activating() { }
        protected virtual void Completing() { }
        #endregion
    }

    public abstract class Activity : ActivityBase
    {
        protected readonly TaskCompletionSource<bool> ActivityCompletion;

        public Activity(Game game)
            : base(game)
        {
            ActivityCompletion = new TaskCompletionSource<bool>();
        }

        #region Game Loop
        protected abstract override void Update(GameTime gameTime);
        protected abstract override void Draw(GameTime gameTime);
        #endregion

        #region Life Cycle
        internal protected virtual Task RunActivity()
        {
            return ActivityCompletion.Task;
        }
        protected void Exit()
        {
            ActivityCompletion.SetResult(true);
        }
        #endregion
    }

    public abstract class Activity<T> : ActivityBase
    {
        protected readonly TaskCompletionSource<T> ActivityCompletion;

        public Activity(Game game)
            : base(game)
        {
            ActivityCompletion = new TaskCompletionSource<T>();
        }

        #region Game Loop
        protected abstract override void Update(GameTime gameTime);
        protected abstract override void Draw(GameTime gameTime);
        #endregion

        #region Life Cycle
        internal protected virtual Task<T> RunActivity()
        {
            return ActivityCompletion.Task;
        }
        protected void Exit(T result)
        {
            ActivityCompletion.SetResult(result);
        }
        #endregion
    }
}
