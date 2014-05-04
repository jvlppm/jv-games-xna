using System;

namespace Jv.Games.Xna
{
    public struct Disposable : IDisposable
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
            _disposed = false;
            _onDispose = onDispose;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _onDispose();
            }
        }
        #endregion
    }
}
