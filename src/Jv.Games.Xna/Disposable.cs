using System;

namespace Jv.Games.Xna
{
    public class Disposable : IDisposable
    {
        #region Attributes
        bool _disposed;
        Action _onDispose;
        #endregion

        public static IDisposable Create(Action onDispose)
        {
            return new Disposable(onDispose);
        }

        #region Constructors
        private Disposable(Action onDispose)
        {
            _onDispose = onDispose;
        }
        #endregion

        #region IDisposable
        ~Disposable()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _disposed = true;
                    _onDispose();
                }
            }
        }
        #endregion
    }
}
