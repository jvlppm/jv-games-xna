using Jv.Games.Xna.Async.Operations;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async
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
                foreach (var timer in _timers.Where(t => !t.Continue(gameTime)).ToList())
                    _timers.Remove(timer);

                Action job;
                while (_jobs.TryDequeue(out job))
                    job();

                Action<GameTime> updateJob;
                while (_updateJobs.TryDequeue(out updateJob))
                    updateJob(gameTime);
            }
        }

        public Task<T> Run<T>(IAsyncOperation<T> operation)
        {
            _timers.Add(operation);
            return operation.Task;
        }

        public Task Run(IAsyncOperation operation)
        {
            _timers.Add(operation);
            return operation.Task;
        }

        public void Post(System.Action<GameTime> action)
        {
            _updateJobs.Enqueue(action);
        }

        public void Post(System.Action action)
        {
            _jobs.Enqueue(action);
        }
        #endregion
    }
}
