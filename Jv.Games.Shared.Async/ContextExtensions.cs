using System;
using Jv.Games.Xna.Context;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async
{
    public static class ContextExtensions
    {
        public static ContextTask Wait(this IContext context, Task task)
        {
            return new ContextTask(task, context);
        }

        public static ContextTask<T> Wait<T>(this IContext context, Task<T> task)
        {
            return new ContextTask<T>(task, context);
        }

        public static ContextTask<Task<T>> WhenAny<T>(this IContext context, params Task<T>[] tasks)
        {
            #if NET_40
            return context.Wait(AsyncBridge.WhenAny(tasks));
            #else
            return context.Wait(Task.WhenAny(tasks));
            #endif
        }

        public static ContextTask<Task> WhenAny(this IContext context, params Task[] tasks)
        {
            #if NET_40
            return context.Wait(AsyncBridge.WhenAny(tasks));
            #else
            return context.Wait(Task.WhenAny(tasks));
            #endif
        }

        public static ContextTask<T[]> WhenAll<T>(this IContext context, params Task<T>[] tasks)
        {
            #if NET_40
            return context.Wait(AsyncBridge.WhenAll(tasks));
            #else
            return context.Wait(Task.WhenAll(tasks));
            #endif
        }

        public static ContextTask WhenAll(this IContext context, params Task[] tasks)
        {
            #if NET_40
            return context.Wait(AsyncBridge.WhenAll(tasks));
            #else
            return context.Wait(Task.WhenAll(tasks));
            #endif
        }
    }
}

