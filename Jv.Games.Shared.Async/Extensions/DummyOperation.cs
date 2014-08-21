using Microsoft.Xna.Framework;
using System;

namespace Jv.Games.Xna.Async
{
    public class DummyOperation : AsyncOperation
    {
        public override bool Continue(GameTime gameTime)
        {
            return !IsCompleted;
        }

        new public void SetCompleted()
        {
            base.SetCompleted();
        }

        new public void SetError(Exception ex)
        {
            base.SetError(ex);
        }
    }

    public class DummyOperation<T> : AsyncOperation<T>
    {
        public override bool Continue(GameTime gameTime)
        {
            return !IsCompleted;
        }

        new public void SetResult(T result)
        {
            base.SetResult(result);
        }

        new public void SetError(Exception ex)
        {
            base.SetError(ex);
        }
    }
}
