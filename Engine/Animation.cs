using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ingenia.Data;

namespace Ingenia.Engine
{
    [Serializable]
    public class Animation
    {
        // Sprite (framed)
        public Texture2D sprite;
        public string image;
        // Frames (per row) and row count
        public int frames;
        public int rows;
        int frame;
        int row;
        // Animation interval (in miliseconds)
        public int time;
        int interval;
        // Looping flag (stop drawing after animation if false)
        public bool loop;
        // Pause flag
        bool paused;
        // Active flag (stop drawing if false)
        bool active;
        public Vector2 position = Vector2.Zero;

        // Constructor
        public Animation(string image, int frames, int rows = 1, int time = 250, bool loop = true)
        {
            // Set sprite
            this.image = image;
            sprite = GameData.GetTexture(image);
            // Set frames and rows
            this.frames = frames;
            this.rows = rows;
            row = 0; frame = 0;
            // Set animation interval
            this.time = time;
            interval = time;
            // Set loop flag
            this.loop = loop;
            // Set pause flag to false
            paused = false;
            // Set animation to active
            active = true;
        }
        public Animation() { }

        // Set frame
        public void SetFrame(int frame) { this.frame = frame; }

        // Update animation
        public void Update(GameTime gameTime)
        {
            // Return if inactive
            if (!active || frame < 0) return;
            // If frame is equal to or over max frames: reset
            if (frame >= frames) frame = loop ? 0 : -1;

            // Return if paused
            if (paused) return;

            // Reduce interval by milliseconds
            time -= gameTime.ElapsedGameTime.Milliseconds;

            // Until time is above 0: check for negative values
            while (time <= 0) {
                time = interval - time;
                frame++;
            }

            // If frame is equal to or over max frames: reset
            if (frame >= frames) frame = loop ? 0 : -1;
        }

        // Pause
        public void Pause() { paused = true; }

        // Play
        public void Play() { paused = false; }

        // Set Row
        public void SetRow(int row, bool reset = true)
        {
            // Return if row is invalid
            if (row >= rows) return;
            if (this.row == row) return;
            // Set new row and reset frame
            this.row = row;
            if (reset) frame = 0;
        }

        public int GetRow() { return row; }
        public int GetFrame() { return frame; }

        public int Width
        {
            get { return sprite.Width / frames; }
        }

        public int Height
        {
            get { return sprite.Height / rows; }
        }

        public Rectangle Bounds(int X, int Y)
        {
            return new Rectangle(X, Y, Width, Height);
        }

        // Draw animation
        public void Draw(SpriteBatch spriteBatch, Vector2 vector, Rectangle? direct = null, bool flip = false, float alpha = 1f, Color? color = null)
        {
            // Return if inactive
            if (!active) return;

            if (position != Vector2.Zero)
                vector = position - vector;

            // Get source rectangle
            Rectangle Source;
            if(direct.HasValue)
                Source = new Rectangle(frame * Width, row * Height + direct.Value.Y, Width, direct.Value.Height);
            else
                Source = new Rectangle(frame * Width, row * Height, Width, Height);

            // Get destination rectangle
            Rectangle Destination = new Rectangle((int)vector.X, (int)vector.Y, Source.Width, Source.Height);

            // Draw the selective frame
            Color c = color.HasValue ? color.Value : Color.White;
            if (flip)
            {
                spriteBatch.Draw(sprite, Destination, Source, c * alpha, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
            }
            else
            {
                spriteBatch.Draw(sprite, Destination, Source, c * alpha);
            }
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 vector, float angle, bool flip = false, float alpha = 1f, Color? color = null)
        {
            // Return if inactive
            if (!active) return;

            // Get source rectangle
            Rectangle Source = new Rectangle(frame * Width, row * Height, Width, Height);

            // Get destination rectangle
            Rectangle Destination = new Rectangle((int)vector.X, (int)vector.Y, Source.Width, Source.Height);

            // Draw the selective frame
            Color c = color.HasValue ? color.Value : Color.White;
            if (flip)
            {
                spriteBatch.Draw(sprite, Destination, Source, c * alpha, angle, new Vector2(Width / 2, Height), SpriteEffects.FlipHorizontally, 0);
            }
            else
            {
                spriteBatch.Draw(sprite, Destination, Source, c * alpha, angle, new Vector2(Width / 2, Height), SpriteEffects.None, 0);
            }
        }

        // Draw to destination
        public void DrawToDestination(SpriteBatch spriteBatch, Rectangle Destination, bool flip = false, float alpha = 1f, Color? color = null)
        {
            // Return if inactive
            if (!active) return;

            // Get source rectangle
            Rectangle Source = new Rectangle(frame * Width, row * Height, Width, Height);

            // Draw the selective frame
            Color c = color.HasValue ? color.Value : Color.White;
            if (flip)
            {
                spriteBatch.Draw(sprite, Destination, Source, c * alpha, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
            }
            else
            {
                spriteBatch.Draw(sprite, Destination, Source, c * alpha);
            }
        }
    }
}
