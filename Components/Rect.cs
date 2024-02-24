using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ingenia.Data;

namespace Ingenia.Interface
{
    public class Rect : Component
    {
        // Rectangle texture
        static Texture2D Graphic = GameData.GetTexture("Rectangle.png");
        // Color
        Color color;

        // Constructor
        public Rect(int x, int y, int width, int height, Color color)
        {
            Bounds = new Rectangle(x, y, width, height);
            this.color = color;
        }
        public Rect(Rectangle rect, Color color)
        {
            Bounds = rect;
            this.color = color;
        }

        // Update
        public override void Update(GameTime gameTime, Vector2 relative)
        {
        }

        // Draw
        public override void Draw(SpriteBatch spriteBatch, Vector2 relative)
        {
            // Set bounds
            Rectangle newBounds = new Rectangle((int)relative.X + Bounds.X,
                (int)relative.Y + Bounds.Y, Bounds.Width, Bounds.Height);
            // Draw only if enabled
            if (Enabled)
                spriteBatch.Draw(Graphic, newBounds, color);
        }
    }
}
