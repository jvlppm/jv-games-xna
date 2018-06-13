using System;

namespace Jv.Games.Xna.Base
{
    public struct Disposable : IDisposable
    {
        #region Attributes
        bool _needsDisposing;
        readonly Action _onDispose;
        #endregion

        public readonly static Disposable Empty = default(Disposable);

        public static IDisposable Create(Action onDispose)
        {
            return new Disposable(onDispose);
        }

        #region Constructors
        private Disposable(Action onDispose)
        {
            _needsDisposing = true;
            _onDispose = onDispose;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (_needsDisposing)
            {
                _needsDisposing = false;
                _onDispose();
            }
        }
        #endregion
    }
}
