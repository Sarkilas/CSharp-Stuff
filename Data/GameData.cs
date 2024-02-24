using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ingenia.Processing;
using System.Windows.Forms;

namespace Ingenia.Data
{
    /// <summary>
    /// This class holds and handles game data.
    /// Please note that all data contained in this class are streams.
    /// Use the methods within this class to process and collect them.
    /// </summary>
    static class GameData
    {
        // The stream dictionary
        public static Dictionary<string, byte[]> Data = new Dictionary<string, byte[]>();

        // The graphics device (must be set up on load)
        public static GraphicsDevice Device;

        // Loaded textures
        public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();

        /// <summary>
        /// Gets a data object based on the given type to return it as.
        /// </summary>
        /// <param name="key">The key to return.</param>
        /// <returns>Returns a base object from the stream represented by the given key.</returns>
        public static Base GetData(string key)
        {
            // Set up
            Base data = null;

            // Create a data rader
            DataReader reader = new DataReader();

            // Import the data using the data reader
            data = reader.Import(new MemoryStream(Data[key]), key);

            // If the data is invalid: throw exception
            if (!data.Valid)
                throw new InvalidDataException(data.ErrorMessage(key));

            // Return the object
            return data;
        }
        /// <summary>
        /// Gets a data object based on the given type to return it as.
        /// </summary>
        /// <param name="bytes">The array of bytes to load the data from.</param>
        /// <returns>Returns the data object. Will always be inheriting the Base class.</returns>
        public static Base GetData(byte[] bytes)
        {
            // Set up
            Base data = null;

            // Create a data rader
            DataReader reader = new DataReader();

            // Import the data using the data reader
            data = reader.Import(new MemoryStream(bytes));

            // If the data is invalid: throw exception
            if (!data.Valid)
                throw new InvalidDataException(data.ErrorMessage("bytes"));

            // Return the object
            return data;
        }

        /// <summary>
        /// Gets a texture from the given key.
        /// A key must be a valid filename including extensions!
        /// </summary>
        /// <param name="key">The key to use.</param>
        /// <returns>Returns the texture associated with this key.</returns>
        public static Texture2D GetTexture(string key)
        {
            // Throw exception if invalid
            if(!(key.ToLower().EndsWith("png")) &&
                !(key.ToLower().EndsWith("jpg")) &&
                !(key.ToLower().EndsWith("dds")) &&
                !(key.ToLower().EndsWith("tga")) &&
                !(key.ToLower().EndsWith("bmp")))
                throw new Exception("Image files must have a valid extension.\n\nReceived image key: " + key);

            // Ignore if the given key is invalid
            if (String.IsNullOrEmpty(key)) return null;

            // If loaded before: return the same texture
            if (Textures.ContainsKey(key.ToLower()))
                return Textures[key.ToLower()];

            try
            {
                // Get the stream
                MemoryStream stream = new MemoryStream(Data[key.ToLower()]);

                // Load the texture
                RenderTarget2D result = null;
                Texture2D file = Texture2D.FromStream(Device, stream), texture,
                    flash;

                // Set all color values to white
                Color[] flashdata = new Color[file.Width * file.Height];
                file.GetData<Color>(flashdata);

                for (int i = 0; i < flashdata.Length; i++)
                    flashdata[i] = new Color(255, 255, 255, flashdata[i].A);

                flash = new Texture2D(Device, file.Width, file.Height);
                flash.SetData<Color>(flashdata);

                //Setup a render target to hold our final texture which will have premulitplied alpha values
                result = new RenderTarget2D(Device, file.Width, file.Height);

                Device.SetRenderTarget(result);
                Device.Clear(Color.Black);

                //Multiply each color by the source alpha, and write in just the color values into the final texture
                BlendState blendColor = new BlendState();
                blendColor.ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue;

                blendColor.AlphaDestinationBlend = Blend.Zero;
                blendColor.ColorDestinationBlend = Blend.Zero;

                blendColor.AlphaSourceBlend = Blend.SourceAlpha;
                blendColor.ColorSourceBlend = Blend.SourceAlpha;

                SpriteBatch spriteBatch = new SpriteBatch(Device);
                spriteBatch.Begin(SpriteSortMode.Immediate, blendColor);
                spriteBatch.Draw(file, file.Bounds, Color.White);
                spriteBatch.End();

                //Now copy over the alpha values from the PNG source texture to the final one, without multiplying them
                BlendState blendAlpha = new BlendState();
                blendAlpha.ColorWriteChannels = ColorWriteChannels.Alpha;

                blendAlpha.AlphaDestinationBlend = Blend.Zero;
                blendAlpha.ColorDestinationBlend = Blend.Zero;

                blendAlpha.AlphaSourceBlend = Blend.One;
                blendAlpha.ColorSourceBlend = Blend.One;

                spriteBatch.Begin(SpriteSortMode.Immediate, blendAlpha);
                spriteBatch.Draw(file, file.Bounds, Color.White);
                spriteBatch.End();

                //Release the GPU back to drawing to the screen
                Device.SetRenderTarget(null);

                // Create texture
                texture = new Texture2D(Device, result.Width, result.Height, true, result.Format);

                // Set data
                Color[] data = new Color[result.Width * result.Height];
                result.GetData<Color>(data);
                texture.SetData<Color>(data);

                // Add to dictionary
                Textures.Add(key.ToLower(), texture);

                result = new RenderTarget2D(Device, file.Width, file.Height);
                Device.SetRenderTarget(result);

                spriteBatch.Begin(SpriteSortMode.Immediate, blendColor);
                spriteBatch.Draw(flash, flash.Bounds, Color.White);
                spriteBatch.End();

                //Now copy over the alpha values from the PNG source texture to the final one, without multiplying them
                spriteBatch.Begin(SpriteSortMode.Immediate, blendAlpha);
                spriteBatch.Draw(flash, flash.Bounds, Color.White);
                spriteBatch.End();

                //Release the GPU back to drawing to the screen
                Device.SetRenderTarget(null);

                // Create texture
                flash = new Texture2D(Device, result.Width, result.Height, true, result.Format);

                // Dispose spritebatch
                spriteBatch.Dispose();

                // Create texture
                flash = new Texture2D(Device, result.Width, result.Height, true, result.Format);

                // Set data
                data = new Color[result.Width * result.Height];
                result.GetData<Color>(data);
                flash.SetData<Color>(data);

                // Add to dictionary
                Textures.Add("flash_" + key.ToLower(), flash);

                // Dispose spritebatch
                spriteBatch.Dispose();

                // Return the texture
                return texture;
            }
            catch (Exception e)
            {
                throw new Exception("Error with image '" + key + "':\n\n" + e.Message, e);
            }
        }
    }
}
