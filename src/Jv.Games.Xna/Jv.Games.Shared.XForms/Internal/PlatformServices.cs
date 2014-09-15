﻿namespace Jv.Games.Xna.XForms
{
    using Jv.Games.Xna.Context;
    using Microsoft.Xna.Framework;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;
#if !PORTABLE
    using System.IO.IsolatedStorage;
#endif

    class PlatformServices : GameComponent, IPlatformServices
    {
        readonly Context UpdateContext;
        readonly HttpClient HttpClient;
        readonly SynchronizationContext MainThreadContext;

        public PlatformServices(Game game)
            : base(game)
        {
            UpdateContext = new Context();
            HttpClient = new HttpClient();
            MainThreadContext = SynchronizationContext.Current;
        }

        public override void Update(GameTime gameTime)
        {
            UpdateContext.Update(gameTime);
            base.Update(gameTime);
        }

        public void BeginInvokeOnMainThread(Action action)
        {
            UpdateContext.Post(action);
        }

        public ITimer CreateTimer(Action<object> callback, object state, uint dueTime, uint period)
        {
            return PlatformTimer.StartNew(UpdateContext, callback, state,
                TimeSpan.FromMilliseconds(dueTime),
                TimeSpan.FromMilliseconds(period));
        }

        public ITimer CreateTimer(Action<object> callback, object state, TimeSpan dueTime, TimeSpan period)
        {
            return PlatformTimer.StartNew(UpdateContext, callback, state, dueTime, period);
        }

        public ITimer CreateTimer(Action<object> callback, object state, long dueTime, long period)
        {
            return PlatformTimer.StartNew(UpdateContext, callback, state,
                TimeSpan.FromMilliseconds(dueTime),
                TimeSpan.FromMilliseconds(period));
        }

        public ITimer CreateTimer(Action<object> callback, object state, int dueTime, int period)
        {
            return PlatformTimer.StartNew(UpdateContext, callback, state,
                TimeSpan.FromMilliseconds(dueTime),
                TimeSpan.FromMilliseconds(period));
        }

        public ITimer CreateTimer(Action<object> callback)
        {
            return new PlatformTimer(UpdateContext, callback);
        }

        public void StartTimer(TimeSpan interval, Func<bool> callback)
        {
            PlatformTimer.StartNew(UpdateContext, callback, interval, interval);
        }

        public async Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
        {
            var webCancellation = new CancellationTokenSource();
            var getWebResponse = HttpClient.GetAsync(uri, webCancellation.Token);

            if (!uri.IsAbsoluteUri)
            {
                try
                {
                    var stream = await Task.Factory.StartNew(() =>
                        TitleContainer.OpenStream(uri.ToString()), cancellationToken);
                    webCancellation.Cancel();
                    return stream;
                }
                catch { }
            }

            cancellationToken.Register(webCancellation.Cancel);

            var response = await getWebResponse;
            return await response.Content.ReadAsStreamAsync();
        }

        public bool IsInvokeRequired
        {
            get { return MainThreadContext == SynchronizationContext.Current; }
        }

        public System.Reflection.Assembly[] GetAssemblies()
        {
#if PORTABLE
            throw new NotImplementedException();
#else
            return AppDomain.CurrentDomain.GetAssemblies();
#endif
        }

        public void OpenUriAction(Uri uri)
        {
#if PORTABLE
            throw new NotImplementedException();
#else
            Process.Start(uri.ToString());
#endif
        }

        public Xamarin.Forms.IIsolatedStorageFile GetUserStoreForApplication()
        {
#if PORTABLE
            throw new NotImplementedException();
#else
            var scope = IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain;
            var isolatedStorage = System.IO.IsolatedStorage.IsolatedStorageFile.GetStore(scope, null, null);
            return new IsolatedStorageFile(isolatedStorage);
#endif
        }
    }
}
