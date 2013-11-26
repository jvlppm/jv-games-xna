﻿using Jv.Games.Xna.Async.Timers;
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
        readonly List<KeyValuePair<IGameLoopAction<T>, TaskCompletionSource<T>>> _timers;
        readonly Queue<Action> _asyncQueue;
        #endregion

        #region Constructors
        public SyncContext()
        {
            _timers = new List<KeyValuePair<IGameLoopAction<T>, TaskCompletionSource<T>>>();
            _asyncQueue = new Queue<Action>();
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

                UpdateTimers(args);

                if (PostLoop != null)
                    PostLoop(this, args);

                while (_asyncQueue.Count > 0)
                    _asyncQueue.Dequeue()();
            }
        }

        public Task<T> RunTimer(IGameLoopAction<T> timer, CancellationToken cancellationToken = default(CancellationToken))
        {
            var kv = new KeyValuePair<IGameLoopAction<T>, TaskCompletionSource<T>>(timer, new TaskCompletionSource<T>());

            if (cancellationToken != CancellationToken.None)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.Register(delegate
                    {
                        kv.Value.TrySetCanceled();
                        _timers.Remove(kv);
                    });
                }
                else
                {
                    kv.Value.SetCanceled();
                    return kv.Value.Task;
                }
            }

            _timers.Add(kv);

            return kv.Value.Task;
        }
        #endregion

        #region Private Methods
        void UpdateTimers(T args)
        {
            foreach (var timer in _timers.Where(t => !t.Key.Step(args)).ToList())
            {
                _timers.Remove(timer);
                timer.Value.TrySetResult(args);
            }
        }
        #endregion
    }
}
