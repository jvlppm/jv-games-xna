using Jv.Games.Xna.Async.Timers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XNATweener;

namespace Jv.Games.Xna.Async
{
    public interface ISyncContext
    {
        void Enqueue(Action action);
        void Run(Action action);
    }

    public interface ISyncContext<in T> : ISyncContext
    {
        void RunLoop(T args);
    }

    public class SyncContext<T> : ISyncContext<T>
        where T : GameLoopEventArgs
    {
        #region Events
        public event GameLoopEventHandler<T> BeforeLoop, PostLoop;
        #endregion

        #region Attributes
        readonly List<KeyValuePair<ITimer<T>, TaskCompletionSource<T>>> _timers;
        readonly Queue<Action> _asyncQueue;
        readonly Action<T> _loop;
        #endregion

        #region Constructors
        public SyncContext(Action<T> loop)
        {
            _timers = new List<KeyValuePair<ITimer<T>, TaskCompletionSource<T>>>();
            _asyncQueue = new Queue<Action>();
            _loop = loop;
        }
        #endregion

        #region Public Methods
        public IDisposable Activate()
        {
            var oldCont = TaskAwaiter.CurrentContext;
            TaskAwaiter.CurrentContext = this;

            return Disposable.Create(() =>
            {
                if (TaskAwaiter.CurrentContext != this)
                    throw new InvalidOperationException("Context must be active, in order to be deactivated.");

                TaskAwaiter.CurrentContext = oldCont;
            });
        }

        public void Enqueue(Action action)
        {
            lock (_asyncQueue)
                _asyncQueue.Enqueue(action);
        }

        public void Run(Action action)
        {
            using (Activate())
                action();
        }

        public void RunLoop(T args)
        {
            using (Activate())
            {
                if (BeforeLoop != null)
                    BeforeLoop(this, args);

                if (_loop != null)
                    _loop(args);

                if (PostLoop != null)
                    PostLoop(this, args);

                while (_asyncQueue.Count > 0)
                    _asyncQueue.Dequeue()();
            }
        }

        #region Async
        public Task<T> RunTimer(ITimer<T> timer, CancellationToken cancellationToken = default(CancellationToken))
        {
            var kv = new KeyValuePair<ITimer<T>, TaskCompletionSource<T>>(timer, new TaskCompletionSource<T>());

            if (cancellationToken != CancellationToken.None)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.Register(delegate
                    {
                        kv.Value.TrySetCanceled();
                        _timers.Remove(kv);

                        if (_timers.Count == 0)
                            BeforeLoop -= UpdateTimers;
                    });
                }
                else
                {
                    kv.Value.SetCanceled();
                    return kv.Value.Task;
                }
            }

            if (_timers.Count == 0)
                BeforeLoop += UpdateTimers;

            _timers.Add(kv);

            return kv.Value.Task;
        }

        public Task<T> Yield()
        {
            return RunTimer(new YieldTimer<T>());
        }

        /// <summary>
        /// Creates a task that will complete after a time delay.
        /// </summary>
        /// <param name="dueTime">The time span to wait before completing the returned task.</param>
        /// <param name="cancellationToken">The cancellation token that will be checked prior to completing the returned task.</param>
        public Task<T> Delay(TimeSpan dueTime, CancellationToken cancellationToken = default(CancellationToken))
        {
            var timer = new CountdownTimer<T>(dueTime);
            return RunTimer(timer, cancellationToken);
        }

        public Task Interpolate(TimeSpan duration, float startValue, float endValue, Action<float> valueStep, TweeningFunction easingFunction = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var info = new Interpolator<T>(duration, startValue, endValue, valueStep, easingFunction);
            return RunTimer(info, cancellationToken);
        }
        #endregion
        #endregion

        #region Private Methods
        void UpdateTimers(object sender, T args)
        {
            foreach (var timer in _timers.ToList())
            {
                if (!timer.Key.Tick(args))
                {
                    _timers.Remove(timer);
                    timer.Value.TrySetResult(args);
                }
            }

            if (_timers.Count == 0)
                BeforeLoop -= UpdateTimers;
        }
        #endregion
    }
}
