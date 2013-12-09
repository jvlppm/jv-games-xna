using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async
{
    public class TimerContext
    {
        #region Nested
        struct TimerInfo
        {
            public ITimer Operation;
            public TaskCompletionSource<GameTime> Completion;
        }
        #endregion

        #region Attributes
        readonly List<TimerInfo> _timers;
        #endregion

        #region Constructors
        public TimerContext()
        {
            _timers = new List<TimerInfo>();
        }
        #endregion

        #region Public Methods
        public void Update(GameTime gameTime)
        {
            foreach (var timer in _timers.Where(t => !t.Operation.Tick(gameTime)).ToList())
            {
                _timers.Remove(timer);
                timer.Completion.TrySetResult(gameTime);
            }
        }

        public Task<GameTime> RunTimer(ITimer timer, CancellationToken cancellationToken = default(CancellationToken))
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
        #endregion
    }
}
