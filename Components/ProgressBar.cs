using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ingenia.Data;
using Ingenia.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ingenia.Interface
{
    /// <summary>
    /// The progress bar component.
    /// </summary>
    class ProgressBar : Component
    {
        /// <summary>
        /// The texture required for drawing.
        /// </summary>
        Texture2D Texture = GameData.GetTexture("white.png"),
            Fill = GameData.GetTexture("Fill.png");

        /// <summary>
        /// The position of this progress bar.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// The maximum value of this progress bar.
        /// </summary>
        public int Max { get; set; }

        /// <summary>
        /// The minimum value of this progress bar.
        /// </summary>
        public int Min { get; set; }

        /// <summary>
        /// The actual value of this progress bar.
        /// </summary>
        public int Value
        {
            get { return _value; }
            set
            {
                OldValue = _value;
                _value = value;
            }
        }
        private int _value;

        /// <summary>
        /// The width of this progress bar.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height of this progress bar.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The color of the fill bar.
        /// </summary>
        public Color BarColor { get; set; }

        /// <summary>
        /// The last value the component had before changing.
        /// </summary>
        public int OldValue { get; set; }

        /// <summary>
        /// Constructs a progress bar.
        /// </summary>
        public ProgressBar(float x, float y, int max, int min, int value, int width, int height, Color color)
        {
            Position = new Vector2(x, y);
            Max = max;
            Min = min;
            Value = value;
            Width = width;
            Height = height;
            BarColor = color;
        }

        /// <summary>
        /// Updates the progress bar.
        /// </summary>
        public override void Update(GameTime gameTime, Vector2 relative)
        {
            // Update events first
            UpdateEvents();

            // Set the old value to the new value
            OldValue = Value;
        }

        /// <summary>
        /// Updates the events for this component.
        /// </summary>
        private void UpdateEvents()
        {
            // Go through each event
            foreach (DataElement element in Events)
            {
                // Ignore if property is missing
                if (!element.Properties.ContainsKey("name"))
                    continue;

                // Value changed event
                if (element.Properties["name"].Value.ToLower() == "value changed" && OldValue != Value)
                    Interpreter.ExecuteEvent(element);

                // Tick event
                if (element.Properties["name"].Value.ToLower() == "tick")
                    Interpreter.ExecuteEvent(element);
            }
        }

        /// <summary>
        /// Draws the progress bar.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch, Vector2 relative)
        {
            // Fix values
            if (Value < Min) Value = Min;
            if (Value > Max) Value = Max;

            // Get the rectangles
            Rectangle container = new Rectangle((int)Position.X - 1, 
                (int)Position.Y - 1, Width + 2, Height + 2);
            float scale = (float)Value / (float)Max;
            Rectangle target = new Rectangle((int)Position.X, 
                (int)Position.Y, (int)(Width * scale), Height);

            // Draw the container
            spriteBatch.Draw(Texture, container, Color.Black);

            // Draw the progress fill
            spriteBatch.Draw(Fill, target, BarColor);
        }
    }
}
