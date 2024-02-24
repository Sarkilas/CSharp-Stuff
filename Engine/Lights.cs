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
    /// <summary>
    /// This part defines all the static elements for light emmision.
    /// Make sure the name of the lights are identical (case sensitive) to 
    /// the names of the object that will emit the light. However, spaces
    /// should be excluded.
    /// </summary>
    public partial class Light
    {
        // Krypton variables
        public static KryptonEngine Engine;
        
        // Null time
        static TimeSpan NullTime = new TimeSpan();

        // Light dictionary
        static Dictionary<string,Light> Lights = new Dictionary<string,Light>();

        /// <summary>
        /// Loads all the lights into the static cache.
        /// </summary>
        public static void LoadLights(Game game){
            // Create engine
            Engine = new KryptonEngine(game, "Light");
            Engine.Initialize();
            Engine.CullMode = CullMode.None;
            Engine.AmbientColor = Color.White;

            // Set texture
            Texture = LightTextureBuilder.CreatePointLight(Light.Engine.GraphicsDevice, 512); 

            // Add all light types
            Lights.Add("omni", new Light(Color.White, 64, null, NullTime, 0f));
        }

        /// <summary>
        /// Updates a selected light.
        /// </summary>
        /// <param name="lightName">The name of the light as a string.</param>
        /// <param name="gameTime">The specified GameTime object.</param>
        public static void UpdateLight(string lightName, GameTime gameTime) { 
            // Update the selected light
            Lights[lightName].Update(gameTime);
        }

        /// <summary>
        /// Draws a selected light.
        /// </summary>
        /// <param name="lightName">The name of the light as a string.</param>
        /// <param name="spriteBatch">The SpriteBatch to draw to.</param>
        public static void DrawLight(string lightName, Vector2 position) {
            // Draw the selected light
            Lights[lightName].Draw(position);
        }

        /// <summary>
        /// Prepares the light engine for drawing.
        /// </summary>
        public static void Prepare()
        {
            // Assign the matrix and pre-render the lightmap.
            Engine.Matrix = CameraMatrix;
            Engine.Bluriness = 40;
            Engine.LightMapPrepare();
        }

        /// <summary>
        /// Draws the lights that are given.
        /// </summary>
        /// <param name="gameTime">Current game time object.</param>
        public static void Draw(GameTime gameTime)
        {
            // Draw lightmap
            Engine.Draw(gameTime);
        }
    }
}