using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ingenia.Data;
using Ingenia.Engine;
using Ingenia.Interface;

namespace Ingenia
{
    public static class Tools
    {
        /// <summary>
        /// Color flasher variable. Used in DrawBorderText for dynamic appearance.
        /// </summary>
        static int Flash = -40;
        static bool FlashOut = false;
        static TimeSpan FlashInterval = new TimeSpan(0, 0, 0, 0, 10);
        static DateTime FlashTime = DateTime.Now;

        public static void DrawItemTooltip(Base item)
        {
            
        }

        /// <summary>
        /// Draws text with a border. Default 1 pixel border.
        /// </summary>
        /// <param name="text">The text to draw.</param>
        /// <param name="position">The position where the text will be drawn.</param>
        /// <param name="color">The color of the text itself.</param>
        /// <param name="bordercolor">The color of the border.</param>
        /// <param name="font">The SpriteFont to use when drawing.</param>
        /// <param name="spriteBatch">The necessary SpriteBatch.</param>
        /// <param name="size">The border size. Default 1 pixel.</param>
        public static void DrawBorderText(string text, Vector2 position, Color color,
            Color bordercolor, SpriteFont font, SpriteBatch spriteBatch, int size = 1, bool noflash = false)
        {
            if (text == null) return;
            // Draw all border texts
            List<int> mx = new List<int>();
            List<int> my = new List<int>();
            for (int i = 1; i <= size; i++) { mx.AddRange(new int[] { -(i), i }); my.AddRange(new int[] { -(i), i }); }
            foreach (int i in mx)
            {
                foreach (int a in my)
                {
                    spriteBatch.DrawString(font, text, new Vector2(position.X + i, position.Y + a), bordercolor);
                }
            }
            // Control flash changes
            if (FlashTime + FlashInterval <= DateTime.Now)
            {
                FlashTime = DateTime.Now;
                if (!FlashOut)
                {
                    Flash += 2;
                    if (Flash == 40) FlashOut = true;
                }
                if (FlashOut)
                {
                    Flash -= 2;
                    if (Flash == -40) FlashOut = false;
                }
            }
            // Create new color
            if(!color.Equals(Color.LightGray) && !noflash)
                color = new Color(color.R - Flash, color.G - Flash, color.B - Flash);
            // Draw main text
            spriteBatch.DrawString(font, text, position, color);
        }

        public static void DrawShadowText(string text, Vector2 position, Color color,
            Color shadowcolor, SpriteFont font, SpriteBatch spriteBatch, int size = 1, bool noflash = false)
        {
            if (text == null) return;
            // Draw shadow
            spriteBatch.DrawString(font, text, new Vector2(position.X + size, position.Y + size), shadowcolor);
            // Control flash changes
            if (FlashTime + FlashInterval <= DateTime.Now)
            {
                FlashTime = DateTime.Now;
                if (!FlashOut)
                {
                    Flash += 2;
                    if (Flash == 40) FlashOut = true;
                }
                if (FlashOut)
                {
                    Flash -= 2;
                    if (Flash == -40) FlashOut = false;
                }
            }
            // Create new color
            if (!color.Equals(Color.LightGray) && !noflash)
                color = new Color(color.R - Flash, color.G - Flash, color.B - Flash);
            // Draw main text
            spriteBatch.DrawString(font, text, position, color);
        }

        public static string GetName<T>(T item) where T : class
        {
            var properties = typeof(T).GetProperties();
            if (properties.Length < 1) return null;
            return properties[0].Name;
        }

        public static TKey FindKeyByValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value)
        {
            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
                if (value.Equals(pair.Value)) return pair.Key;

            return default(TKey);
        }

        static StringBuilder builder = new StringBuilder(" ");
        public static char[] NewLine = { '\r', '\n' };
        static Vector2 MeasureCharacter(this SpriteFont font, char character)
        {
            builder[0] = character;
            return font.MeasureString(builder);
        }

        public static void WordWrap(StringBuilder original, StringBuilder target, SpriteFont font, Rectangle bounds, float scale)
        {
            int lastWhiteSpace = 0;
            float currentLength = 0;
            float lengthSinceLastWhiteSpace = 0;
            float characterWidth = 0;
            for (int i = 0; i < original.Length; i++)
            {
                //get the character 
                char character = original[i];
                //measure the length of the current line 
                characterWidth = font.MeasureCharacter(character).X * scale;
                currentLength += characterWidth;
                //find the length since last white space 
                lengthSinceLastWhiteSpace += characterWidth;
                //are we at a new line? 
                if ((character != '\r') && (character != '\n'))
                {
                    //time for a new line? 
                    if (currentLength > bounds.Width)
                    {
                        //if so are we at white space? 
                        if (char.IsWhiteSpace(character))
                        {
                            //if so insert newline here 
                            target.Insert(i, NewLine);
                            //reset lengths 
                            currentLength = 0;
                            lengthSinceLastWhiteSpace = 0;
                            // return to the top of the loop as to not append white space 
                            continue;
                        }
                        else
                        {
                            //not at white space so we insert a new line at the previous recorded white space 
                            target.Insert(lastWhiteSpace, NewLine);
                            //remove the white space 
                            target.Remove(lastWhiteSpace + NewLine.Length, 1);
                            //make sure the the characters at the line break are accounted for 
                            currentLength = lengthSinceLastWhiteSpace;
                            lengthSinceLastWhiteSpace = 0;
                        }
                    }
                    else
                    {
                        //not time for a line break? are we at white space? 
                        if (char.IsWhiteSpace(character))
                        {
                            //record it's location 
                            lastWhiteSpace = target.Length;
                            lengthSinceLastWhiteSpace = 0;
                        }
                    }
                }
                else
                {
                    lengthSinceLastWhiteSpace = 0;
                    currentLength = 0;
                }
                //always append  
                target.Append(character);
            }
        }

        public static int StringLines(string s)
        {
            int count = 1;
            int start = 0;
            while ((start = s.IndexOf('\n', start)) != -1)
            {
                count++;
                start++;
            }
            return count;
        }

        /// <summary>
        /// Fades the screen to the certain percentage value represented by a float number.
        /// 100% fade makes the screen completely black.
        /// </summary>
        /// <param name="alpha">The alpha float value.</param>
        public static void Fade(float alpha)
        {
            // Load the graphic on demand
            Texture2D pixel = GameData.GetTexture("White.png");

            // Draw it based on the alpha value
            Game.spriteBatch.Draw(pixel, Screen.Bounds, Color.Black * alpha);
        }

        public static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public static bool Between(this float self, float a, float b)
        {
            return (self > a && self < b);
        }

        /*public static bool PlayerOnScreen(NetworkPlayer player)
        {
            // Find camera span
            int topX = (int)Screen.Camera.X - 100,
                topY = (int)Screen.Camera.Y - 100;

            // Check positioning
            if (player.Position.X >= topX && player.Position.X <= topX + Screen.Width + 200 &&
                player.Position.Y >= topY && player.Position.Y <= topY + Screen.Height + 200)
                return true;

            // Return false
            return false;
        }
        public static bool PlayerOnScreen(Character player)
        {
            // Find camera span
            int topX = (int)Screen.Camera.X - 100,
                topY = (int)Screen.Camera.Y - 100;

            // Check positioning
            if (player.Position.X >= topX && player.Position.X <= topX + Screen.Width + 200 &&
                player.Position.Y >= topY && player.Position.Y <= topY + Screen.Height + 200)
                return true;

            // Return false
            return false;
        }*/
    }
}
