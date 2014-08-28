namespace Jv.Games.Xna.Context
{
    using Microsoft.Xna.Framework;
    using System;

    public class DummyOperation : GameOperation
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

    public class DummyOperation<T> : GameOperation<T>
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
