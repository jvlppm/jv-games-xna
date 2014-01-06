using Microsoft.Xna.Framework;
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

        public static void Send(this AsyncContext context, System.Action action)
        {
            using (context.Activate())
                action();
        }

        public static void Send(this AsyncContext context, System.Action<AsyncContext> action)
        {
            using (context.Activate())
                action(context);
        }

        public static void Send(this AsyncContext context, System.Action<GameTime> action, GameTime gameTime)
        {
            using (context.Activate())
                action(gameTime);
        }
    }
}
