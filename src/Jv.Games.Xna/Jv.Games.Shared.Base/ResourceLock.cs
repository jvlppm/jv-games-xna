using System.Collections.Generic;
using System;

namespace Jv.Games.Xna.Base
{
    public class ResourceLock
    {
        uint _locks;

        public bool IsLocked => _locks > 0;

        public IDisposable Lock(string reason)
        {
            _locks++;
            return Disposable.Create(() => _locks--);
        }
    }
}