namespace Jv.Games.Xna.Context
{
    using System;
    using System.Threading;
    using Microsoft.Xna.Framework;

    class KeepAliveOperation : IGameOperation
    {
        public IOperationStatus Status => _status;

        public readonly Action Enabled;
        public readonly Action Disabled;

        readonly IContext _context;
        readonly OperationStatus _status = new OperationStatus();

        public KeepAliveOperation(IContext context, Action onEnable, Action onDisable)
        {
            _context = context;
            Enabled = onEnable;
            Disabled = onDisable;
            _status.SetCompleted();
        }

        int _keepAlive;
        bool _onContext;

        public void Process ()
        {
            if (_status.IsCompleted) {
                _keepAlive = 2;
                _status.Reset();
                if (!_onContext) {
                    _onContext = true;
                    _context.Run(this);
                }
                Enabled?.Invoke();
            }
            else {
                _keepAlive += 1;
            }
        }

        public void Cancel ()
        {
            if (Status.IsCompleted)
                return;

            _keepAlive = 0;
            Disabled?.Invoke();
            _status.SetCompleted();
        }

        void IGameOperation.Continue(GameTime gameTime)
        {
            if (Status.IsCompleted) {
                _onContext = false;
                return;
            }

            if (--_keepAlive <= 0) {
                _status.SetCompleted();
                Disabled?.Invoke();
                _onContext = false;
            }
        }
    }

    public readonly struct OperationController
    {
        readonly Action _process;
        readonly Action _cancel;

        internal OperationController(Action process, Action cancel)
        {
            _process = process;
            _cancel = cancel;
        }

        public void Process() => _process?.Invoke();
        public void Cancel() => _cancel?.Invoke();
    }

    public readonly struct OperationController<T>
    {
        readonly Action<T> _process;
        readonly Action _cancel;

        internal OperationController(Action<T> process, Action cancel)
        {
            _process = process;
            _cancel = cancel;
        }

        public void Process(T value) => _process?.Invoke(value);
        public void Cancel() => _cancel?.Invoke();
    }

    public static class KeepAliveExtensions
    {
        static OperationController CreateUpdateOperation(this IContext context, Action onEnable, Action onDisable)
        {
            var op = new KeepAliveOperation(context, onEnable, onDisable);
            return new OperationController(op.Process, op.Cancel);
        }

        public static OperationController CreateUpdateOperation(this IContext context, Action<CancellationToken> asyncMethod)
        {
            CancellationTokenSource cts = null;
            return context.CreateUpdateOperation(delegate {
                cts = new CancellationTokenSource();
                asyncMethod(cts.Token);
            },
            delegate {
                cts.Cancel();
            });
        }

        public static OperationController<T> CreateUpdateOperation<T>(this IContext context, Action<T> onEnable, Action<T> onDisable)
        {
            T lastT = default(T);
            var op = new KeepAliveOperation(context,
                onEnable: () => onEnable(lastT),
                onDisable: () => onDisable(lastT)
            );
            return new OperationController<T>(t => ProcessValue(t, ref lastT, op), op.Cancel);
        }

        public static OperationController<T> CreateUpdateOperation<T>(this IContext context, Action<T, CancellationToken> asyncMethod)
        {
            T lastT = default(T);
            CancellationTokenSource cts = null;
            var op = new KeepAliveOperation(context,
                onEnable:delegate {
                    cts = new CancellationTokenSource();
                    asyncMethod(lastT, cts.Token);
                },
                onDisable: delegate {
                    cts.Cancel();
                });
            return new OperationController<T>(t => ProcessValue(t, ref lastT, op), op.Cancel);
        }

        static void ProcessValue<T>(T value, ref T previousValue, KeepAliveOperation op)
        {
            if (!object.Equals(value, previousValue))
            {
                if (!op.Status.IsCompleted) {
                    op.Disabled?.Invoke();
                    previousValue = value;
                    op.Enabled?.Invoke();
                }
                else {
                    previousValue = value;
                }
            }
            op.Process();
        }
    }
}
