using System.Collections.Generic;
using System;

namespace Jv.Games.Xna.Base
{
    public struct ResourceLock
    {
        HashSet<string> Locks;

        public bool IsLocked => Locks != null && Locks.Count > 0;

        public IDisposable Lock(string reason)
        {
            if (Locks == null) {
                Locks = new HashSet<string>();
            }

            if (!Locks.Add(reason)) {
                throw new InvalidOperationException();
            }
            var locks = Locks;
            return Disposable.Create(() => locks.Remove(reason));
        }
    }
}