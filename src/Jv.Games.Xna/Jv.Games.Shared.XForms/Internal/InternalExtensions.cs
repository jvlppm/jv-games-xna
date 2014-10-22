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

        public static Vector3 Transform(this IEnumerable<Matrix> matrices, Vector3 point)
        {
            return matrices.Aggregate(point, (a, b) => Vector3.Transform(a, b));
        }

        public static Vector3 Revert(this IEnumerable<Matrix> matrices, Vector3 point)
        {
            return matrices.Reverse().Aggregate(point, (a, b) => Vector3.Transform(a, Matrix.Invert(b)));
        }
    }
}
