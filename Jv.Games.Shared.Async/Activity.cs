using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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

        protected SpriteBatch SpriteBatch { get; private set; }

        protected ContentManager Content { get { return Game.Content; } }
        protected GraphicsDevice GraphicsDevice { get { return Game.GraphicsDevice; } }
        protected Viewport Viewport { get { return Game.GraphicsDevice.Viewport; } }

        public ActivityBase(Game game)
            : base(game)
        {
            SpriteBatch = new SpriteBatch(game.GraphicsDevice);
        }

        #region Game Loop
        void IUpdateable.Update(GameTime gameTime)
        {
            if (SubActivity == null || SubActivity.AllTransparent())
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
            if (SubActivity == null || SubActivity.AllTransparent())
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

        protected Task Run(Activity activity)
        {
            return Run(activity, () => activity.RunActivity().Select(true));
        }

        protected Task Run<T>(params object[] args)
            where T : Activity
        {
            Type[] argTypes;
            object[] ctorArgs;
            GetConstructorInfo(args, out argTypes, out ctorArgs);

            var ctor = typeof(T).GetConstructor(argTypes);
            var act = (T)ctor.Invoke(ctorArgs);
            return Run(act);
        }

        protected Task<TResult> Run<T, TResult>(params object[] args)
            where T : Activity<TResult>
        {
            Type[] argTypes;
            object[] ctorArgs;
            GetConstructorInfo(args, out argTypes, out ctorArgs);

            var ctor = typeof(T).GetConstructor(argTypes);
            var act = (T)ctor.Invoke(ctorArgs);
            return Run(act);
        }

        #region Life Cycle
        protected virtual void Deactivating() { }
        protected virtual void Starting() { }
        protected virtual void Activating() { }
        protected virtual void Completing() { }
        #endregion

        #region Private Methods
        void GetConstructorInfo(object[] args, out Type[] argTypes, out object[] ctorArgs)
        {
            argTypes = new[] { Game.GetType() }.Concat(args.Select(a => a == null ? typeof(object) : a.GetType())).ToArray();
            ctorArgs = new[] { Game }.Concat(args).ToArray();
        }
        #endregion
    }

    public abstract class Activity : ActivityBase
    {
        protected readonly TaskCompletionSource<bool> ActivityCompletion;
        protected readonly CancellationToken CancelOnExit;

        protected Activity(Game game)
            : base(game)
        {
            ActivityCompletion = new TaskCompletionSource<bool>();
            var cts = new CancellationTokenSource();
            ActivityCompletion.Task.ContinueWith(t => cts.Cancel());
            CancelOnExit = cts.Token;
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
            ActivityCompletion.TrySetResult(true);
        }
        #endregion
    }

    public abstract class Activity<T> : ActivityBase
    {
        protected readonly TaskCompletionSource<T> ActivityCompletion;
        protected readonly CancellationToken CancelOnExit;

        protected Activity(Game game)
            : base(game)
        {
            ActivityCompletion = new TaskCompletionSource<T>();
            var cts = new CancellationTokenSource();
            ActivityCompletion.Task.ContinueWith(t => cts.Cancel());
            CancelOnExit = cts.Token;
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
            ActivityCompletion.TrySetResult(result);
        }
        #endregion
    }
}
