using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async.Core
{
    public class AsyncContext : ISoftSynchronizationContext
    {
        #region Attributes
        readonly List<IAsyncOperation> _timers;
        readonly ConcurrentQueue<Action<GameTime>> _updateJobs;
        readonly ConcurrentQueue<Action> _jobs;
        #endregion

        #region Constructors
        public AsyncContext()
        {
            _timers = new List<IAsyncOperation>();
            _jobs = new ConcurrentQueue<Action>();
            _updateJobs = new ConcurrentQueue<Action<GameTime>>();
        }
        #endregion

        #region Public Methods
        public void Update(GameTime gameTime)
        {
            using (this.Activate())
            {
                _timers.RemoveAll(t => !t.Continue(gameTime));

                Action job;
                while (_jobs.TryDequeue(out job))
                    job();

                Action<GameTime> updateJob;
                while (_updateJobs.TryDequeue(out updateJob))
                    updateJob(gameTime);
            }
        }

        public ContextTaskAwaitable<T> Run<T>(IAsyncOperation<T> operation)
        {
            _timers.Add(operation);
            return operation.Task.On(this);
        }

        public ContextTaskAwaitable Run(IAsyncOperation operation)
        {
            _timers.Add(operation);
            return operation.Task.On(this);
        }

        public void Post(System.Action<GameTime> action)
        {
            _updateJobs.Enqueue(action);
        }

        public void Post(System.Action action)
        {
            _jobs.Enqueue(action);
        }

        public void Send(System.Action action)
        {
            using (Activate())
                action();
        }

        public void Send(System.Action<AsyncContext> action)
        {
            using (Activate())
                action(this);
        }

        public void Send(Action<GameTime> action, GameTime gameTime)
        {
            using (Activate())
                action(gameTime);
        }
        #endregion

        #region Private Methods
        internal IDisposable Activate()
        {
            var oldContext = TaskEx.CurrentContext;
            TaskEx.CurrentContext = this;
            return Disposable.Create(() => TaskEx.CurrentContext = oldContext);
        }
        #endregion
    }
}
