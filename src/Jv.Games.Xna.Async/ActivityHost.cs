using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Async
{
    public class ActivityHost : ActivityBase
    {
        public ActivityHost(Game game)
            : base(game)
        {
        }

        [SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods")]
        new public Task Run(Activity activity)
        {
            return base.Run(activity);
        }

        [SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods")]
        new public Task<TActivity> Run<TActivity>(Activity<TActivity> activity)
        {
            return base.Run(activity);
        }
    }
}
