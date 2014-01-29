using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Games.Xna.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var game = new MainGame())
                game.Run();
        }
    }
}
