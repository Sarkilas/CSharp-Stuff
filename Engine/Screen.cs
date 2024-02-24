using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ingenia
{
    public static class Screen
    {
        // Screen sizes
        public static int Width = 800;
        public static int Height = 600;

        // Camera position
        public static Vector2 Camera = Vector2.Zero;

        /// <summary>
        /// Checks if a rectangle is on the screen bounds.
        /// </summary>
        public static bool OnScreen(Rectangle rect)
        {
            return Bounds.Intersects(rect);
        }

        /// <summary>
        /// Returns the offset based on how much larger the screen is comapred to 800x600.
        /// </summary>
        public static Vector2 Offset
        {
            get
            {
                return new Vector2((Screen.Width - 800) / 2, (Screen.Height - 600) / 2);
            }
        }

        // Bounds
        public static Rectangle Bounds
        {
            get { return new Rectangle(0, 0, Width, Height); }
        }
    }
}
