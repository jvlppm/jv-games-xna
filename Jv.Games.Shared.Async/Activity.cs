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

        public ActivityBase(Game game)
            : base(game)
        {
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

        protected ContextTaskAwaitable<TResult> Run<TResult>(ActivityBase activity, Func<Task<TResult>> runActivity)
        {
            if (runActivity == null)
                throw new ArgumentNullException("runActivity");

            if (SubActivity != null)
                throw new InvalidOperationException("Activity is already running another sub-activity");

            var capturedContext = Context.Current;
            if (capturedContext == null)
                throw new InvalidOperationException("Run must be called from inside a context.");

            using (UpdateContext.Activate())
                Deactivating();

            SubActivity = activity;
            ((IGameComponent)activity).Initialize();

            using (activity.UpdateContext.Activate())
            {
                var tcs = new TaskCompletionSource<TResult>();

                Task<TResult> runTask;
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

                return tcs.Task.On(capturedContext);
            }
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

        protected SpriteBatch SpriteBatch { get; private set; }

        protected ContentManager Content { get { return Game.Content; } }
        protected GraphicsDevice GraphicsDevice { get { return Game.GraphicsDevice; } }
        protected Viewport Viewport { get { return Game.GraphicsDevice.Viewport; } }

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
        internal protected virtual Task<T> RunActivity()
        {
            return ActivityCompletion.Task;
        }

        protected void Exit(T result)
        {
            ActivityCompletion.TrySetResult(result);
        }
        #endregion

        #region Public Methods
        public ContextTaskAwaitable<TResult> Run<TResult>(Activity<TResult> subActivity)
        {
            return Run(subActivity, subActivity.RunActivity);
        }

        public ContextTaskAwaitable<TResult> RunNew<TActivity, TResult>(params object[] args)
            where TActivity : Activity<TResult>
        {
            Type[] argTypes;
            object[] ctorArgs;
            GetConstructorInfo(args, out argTypes, out ctorArgs);

            var ctor = typeof(TActivity).GetConstructor(argTypes);
            var act = (TActivity)ctor.Invoke(ctorArgs);
            return Run(act);
        }
        #endregion

        #region Private Methods
        void GetConstructorInfo(object[] args, out Type[] argTypes, out object[] ctorArgs)
        {
            argTypes = new[] { Game.GetType() }.Concat(args.Select(a => a == null ? typeof(object) : a.GetType())).ToArray();
            ctorArgs = new[] { Game }.Concat(args).ToArray();
        }
        #endregion
    }
}
