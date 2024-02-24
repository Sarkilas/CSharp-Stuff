using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ingenia.Interface
{
    /// <summary>
    /// Holds image scroller data and location for drawing.
    /// </summary>
    public class ImageScroller : Component
    {
        /// <summary>
        /// The image to draw.
        /// </summary>
        Texture2D Image;

        /// <summary>
        /// Scroll values.
        /// </summary>
        float ScrollX = 0f,
            ScrollY = 0f;

        /// <summary>
        /// The location of this scroller.
        /// </summary>
        Vector2 Location;

        /// <summary>
        /// The opacity.
        /// </summary>
        float Opacity = 1f;

        public ImageScroller(Texture2D image, Vector2 location, float scrollx, float scrolly, float opacity)
        {
            Image = image;
            Bounds = new Rectangle((int)location.X, (int)location.Y,
                Image.Width, Image.Height);
            Location = location;
            ScrollX = scrollx;
            ScrollY = scrolly;
            Opacity = opacity;
        }

        public override void Update(GameTime gameTime, Vector2 relative)
        {
            Location += new Vector2(ScrollX, ScrollY);

            if (Location.X >= Image.Width ||
                Location.X <= -Image.Width)
                Location.X = 0;

            if (Location.Y <= -Image.Height ||
                Location.Y >= Image.Height)
                Location.Y = 0;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 relative)
        {
            spriteBatch.Draw(Image, Location, Color.White * Opacity);

            if (Location.X > 0)
            {
                float x = Location.X;
                while (x > 0)
                {
                    x -= Image.Width;
                    spriteBatch.Draw(Image, new Vector2(x, Location.Y), Color.White * Opacity);
                }
            }

            if(Location.X + Image.Width < Screen.Width)
            {
                float x = Location.X;
                while (x < Screen.Width)
                {
                    x += Image.Width;
                    spriteBatch.Draw(Image, new Vector2(x, Location.Y), Color.White * Opacity);
                }
            }

            if (Location.Y > 0)
            {
                float y = Location.Y;
                while (y > 0)
                {
                    y -= Image.Height;
                    spriteBatch.Draw(Image, new Vector2(Location.X, y), Color.White * Opacity);
                }
            }

            if (Location.Y + Image.Height < Screen.Height)
            {
                float y = Location.Y;
                while (y < Screen.Height)
                {
                    y += Image.Height;
                    spriteBatch.Draw(Image, new Vector2(Location.X, y), Color.White * Opacity);
                }
            }
        }
    }
}
