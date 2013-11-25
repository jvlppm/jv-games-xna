using System.Threading.Tasks;

namespace Jv.Games.Xna.Async.Timers
{
    public class YieldTimer<T> : ITimer<T>
        where T : GameLoopEventArgs
    {
        TaskCompletionSource<T> _tcs;

        public YieldTimer()
        {
            _tcs = new TaskCompletionSource<T>();
        }

        public bool Tick(T args)
        {
            _tcs.TrySetResult(args);
            return false;
        }

        public System.Threading.Tasks.Task<T> Task
        {
            get { return _tcs.Task; }
        }

        public void Cancel()
        {
            _tcs.TrySetCanceled();
        }
    }
}
