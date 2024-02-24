using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ingenia.Engine;

namespace Ingenia.Interface
{
    /// <summary>
    /// Holds image data and location for drawing.
    /// </summary>
    public class ImageComponent : Component
    {
        /// <summary>
        /// The image to draw.
        /// </summary>
        Texture2D Image;
        Animation Animation;

        /// <summary>
        /// The center values.
        /// </summary>
        bool CenterX, CenterY;

        public ImageComponent(Texture2D image, Vector2 location, bool centerx = false, bool centery = false)
        {
            Image = image;
            Bounds = new Rectangle((int)location.X, (int)location.Y,
                Image.Width, Image.Height);
            CenterX = centerx;
            CenterY = centery;
            Animation = null;
        }
        public ImageComponent(Animation animation, Vector2 location, bool centerx = false, bool centery = false)
        {
            Animation = animation;
            Bounds = new Rectangle((int)location.X, (int)location.Y,
                Animation.Width, Animation.Height);
            CenterX = centerx;
            CenterY = centery;
            Image = null;
        }

        public override void Update(GameTime gameTime, Vector2 relative)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 relative)
        {
            if (Image != null)
            {
                Rectangle newBounds = new Rectangle((int)relative.X + Bounds.X,
                    (int)relative.Y + Bounds.Y, Image.Width, Image.Height);
                if (CenterX)
                    newBounds.X -= Image.Width / 2;
                if (CenterY)
                    newBounds.Y -= Image.Height / 2;
                spriteBatch.Draw(Image, newBounds, Color.White);
            }
            else
            {
                Vector2 vector = new Vector2(Bounds.X, Bounds.Y);
                if (CenterX)
                    vector.X -= Animation.Width / 2;
                if (CenterY)
                    vector.Y -= Animation.Height / 2;
                Animation.Draw(spriteBatch, relative + vector);
            }
        }
    }
}
