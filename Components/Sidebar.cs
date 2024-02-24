using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ingenia.Data;

namespace Ingenia.Interface
{
    static class Sidebar
    {
        // Texture
        static Texture2D Graphic = GameData.GetTexture("SideBar.png"),
            Graphic2 = GameData.GetTexture("HelpBar.png");
        // Font
        static SpriteFont Font = Game.Fonts["NormalFont"];

        // Draw sidebar
        public static void Draw(SpriteBatch spriteBatch, int x = 0, bool right = false)
        {
            // Set position
            Vector2 position = new Vector2(x, 0);

            // Draw until screen is filled
            for (int i = 0; (int)position.Y + (Graphic.Height * i) < Screen.Height; i++)
            {
                if (right)
                    spriteBatch.Draw(Graphic, new Vector2(position.X, position.Y + Graphic.Height * i),
                        null, Color.White * .75f, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0f);
                else
                    spriteBatch.Draw(Graphic, new Vector2(position.X, position.Y + Graphic.Height * i), Color.White *.75f);
            }
        }

        // Draw help
        public static void DrawHelp(string text, SpriteBatch spriteBatch, int y = 32)
        {
            // Set position
            Vector2 position = new Vector2(0, y), size;

            // Draw until screen is filled
            for (int i = 0; (int)position.X + (Graphic2.Width * i) < Screen.Width; i++)
                spriteBatch.Draw(Graphic2, new Vector2(position.X + Graphic2.Width * i, position.Y), Color.White * .75f);
            
            // Get text size
            size = Font.MeasureString(text);

            // Draw text
            Tools.DrawShadowText(text, new Vector2(Screen.Width / 2 - size.X / 2, position.Y + Graphic2.Height / 2 - size.Y / 2),
                Color.White, Color.Black, Font, spriteBatch);
        }
    }
}
