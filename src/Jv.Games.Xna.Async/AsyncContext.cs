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
        #region Nested
        struct TimerInfo
        {
            public IAsyncOperation Operation;
            public TaskCompletionSource<GameTime> Completion;
        }
        #endregion

        #region Attributes
        readonly List<TimerInfo> _timers;
        readonly ConcurrentQueue<Action> _jobs;
        #endregion

        #region Constructors
        public AsyncContext()
        {
            _timers = new List<TimerInfo>();
            _jobs = new ConcurrentQueue<Action>();
        }
        #endregion

        #region Public Methods
        public void Update(GameTime gameTime)
        {
            using (this.Activate())
            {
                foreach (var timer in _timers.Where(t => !t.Operation.Continue(gameTime)).ToList())
                {
                    _timers.Remove(timer);
                    timer.Completion.TrySetResult(gameTime);
                }

                Action job;
                while (_jobs.TryDequeue(out job))
                    job();
            }
        }

        public Task<GameTime> Run(IAsyncOperation timer, CancellationToken cancellationToken = default(CancellationToken))
        {
            var info = new TimerInfo { Completion = new TaskCompletionSource<GameTime>(), Operation = timer };

            if (cancellationToken != CancellationToken.None)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.Register(delegate
                    {
                        info.Completion.TrySetCanceled();
                        _timers.Remove(info);
                    });
                }
                else
                {
                    info.Completion.SetCanceled();
                    return info.Completion.Task;
                }
            }

            _timers.Add(info);

            return info.Completion.Task;
        }

        public void Post(System.Action action)
        {
            _jobs.Enqueue(action);
        }
        #endregion
    }
}
