using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Jv.Games.Xna.XForms
{
    static class InternalExtensions
    {
        public static Matrix Multiply(this IEnumerable<Matrix> matrices)
        {
            return matrices.Aggregate(Matrix.Identity, (a, b) => a * b);
        }

        public static Vector2 Transform(this IEnumerable<Matrix> matrices, Vector2 point)
        {
            return matrices.Aggregate(point, (a, b) => Vector2.Transform(a, b));
        }

        public static Vector2 Revert(this IEnumerable<Matrix> matrices, Vector2 point)
        {
            return matrices.Reverse().Aggregate(point, (a, b) => Vector2.Transform(a, Matrix.Invert(b)));
        }
    }
}
