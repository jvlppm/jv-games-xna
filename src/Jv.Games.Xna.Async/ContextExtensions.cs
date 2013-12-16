using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async
{
    public static class ContextExtensions
    {
        public static IDisposable Activate(this ISoftSynchronizationContext context)
        {
            var oldContext = TaskAwaiter.CurrentContext;
            TaskAwaiter.CurrentContext = context;
            return Disposable.Create(() => TaskAwaiter.CurrentContext = oldContext);
        }
    }
}
