using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Krypton;
using Krypton.Common;
using Krypton.Lights;

namespace Ingenia.Engine
{
    public partial class Light
    {
        // Light texture
        public static Texture2D Texture;
        
        // Properties
        Color color;
        int Range;
        float Intensity;
        Animation Emitter;
        TimeSpan EmitterInterval;
        float Angle;

        // Variables
        DateTime EmitterTime = DateTime.Now;

        /// <summary>
        /// Creates a light object.
        /// </summary>
        /// <param name="color">The color of the light.</param>
        /// <param name="range">The range of the light (in tiles).</param>
        /// <param name="emitter">Animation to play each emitter interval (for light effects).</param>
        /// <param name="interval">The time between each emitter play.</param>
        /// <param name="type">The type of the source the light emits from.</param>
        /// <param name="source">The name of the source the light emits from.</param>
        public Light(Color color, int range, Animation emitter, TimeSpan interval, float angle, float intensity = 1f)
        {
            this.color = color;
            Range = range;
            Emitter = emitter;
            EmitterInterval = interval;
            Intensity = intensity;
            Angle = angle;
        }

        public static Matrix CameraMatrix
        {
            get
            {
                return Matrix.CreateTranslation(Vector3.Zero) *
                Matrix.CreateScale(1f) *
                Matrix.CreateRotationZ(0f) *
                Matrix.CreateTranslation(0f, 0f, 0);
            }
        }

        /// <summary>
        /// Updates the light.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            // If new emitter needs to be played: play it
            if (Emitter != null &&
                EmitterTime + EmitterInterval <= DateTime.Now)
            {
                Emitter.SetFrame(0);
                Emitter.Play();
                EmitterTime = DateTime.Now;
            }

            // Update emitter
            Emitter.Update(gameTime);
        }

        /// <summary>
        /// Draws the light.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to draw the light to.</param>
        public void Draw(Vector2 position) {
            // Create light
            Light2D light = new Light2D()
            {
                // Set properties to the light's properties
                Color = color,
                Intensity = Intensity,
                Position = position,
                Range = Range * 2,
                Texture = Texture,
                Angle = Angle,
                X = position.X,
                Y = position.Y
            };   
            // Add to engine for later drawing
            Engine.Lights.Add(light);
        }
    }
}
