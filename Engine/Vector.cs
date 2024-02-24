using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ingenia
{
    /// <summary>
    /// Contains integer X and Y values. Opposed to float values in Vector2.
    /// </summary>
    [Serializable]
    public class Vector
    {
        // Coordinates
        public int X, Y;

        // Constructor
        public Vector(int x, int y) { X = x; Y = y; }
    }
}
