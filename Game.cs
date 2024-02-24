using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Ingenia.Data;
using Ingenia.Engine;
using Ingenia.Processing;
using Ingenia.Interface;
using System.Windows.Forms;
using EventInput;

namespace Ingenia
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static Base Globals;
        public static Texture2D Cursor;
        public static Dictionary<string, SpriteFont> Fonts = new Dictionary<string, SpriteFont>();
        static bool ExitGame = false;
        public static bool ShowMouse = true;
        public static Scene scene, newScene;
        public static Texture2D Fader;
        public static bool Paused = false;

        public static KeyboardDispatcher Dispatcher;

        public static GraphicsDevice CurrentDevice;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Load database
            Database.Load(Content, this);

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();
        }

        public static void Quit() { ExitGame = true; }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Set up dispatcher
            Dispatcher = new KeyboardDispatcher(Window);

            // Set device
            GameData.Device = GraphicsDevice;
            CurrentDevice = GraphicsDevice;

            // Initialize the processor
            Processor.Initialize();

            // Initialize all packages
            DataReader reader = new DataReader();
            foreach (string file in Directory.GetFiles("Data"))
                foreach (KeyValuePair<string, byte[]> pair in Packing.Load(file))
                {
                    if (GameData.Data.ContainsKey(pair.Key))
                        GameData.Data[pair.Key] = pair.Value;
                    else
                        GameData.Data.Add(pair.Key, pair.Value);

                    if(pair.Key.ToLower().EndsWith("ogg") ||
                        pair.Key.ToLower().EndsWith("mp3") ||
                        pair.Key.ToLower().EndsWith("wav") ||
                        pair.Key.ToLower().EndsWith("png") ||
                        (pair.Key.ToLower().EndsWith("jpg")) ||
                        (pair.Key.ToLower().EndsWith("dds")) ||
                        (pair.Key.ToLower().EndsWith("tga")) ||
                        (pair.Key.ToLower().EndsWith("bmp")))
                        continue;
                }

            // Load the globals document (required data file)
            Globals = GameData.GetData("globals");

            // Load the cursor
            Cursor = GameData.GetTexture(Globals.Properties["normalcursor"].Value);

            // Load light engines
            Light.LoadLights(this);

            // Set timestep value
            IsFixedTimeStep = false;

            // Set mouse flag
            IsMouseVisible = false;

            // Set up light engine
            Light.Engine.AmbientColor = Color.White;
            Light.Engine.SpriteBatchCompatablityEnabled = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load all textures
            DataReader reader = new DataReader();
            foreach (string key in GameData.Data.Keys)
            {
                if ((key.ToLower().EndsWith("png")) ||
                    (key.ToLower().EndsWith("jpg")) ||
                    (key.ToLower().EndsWith("dds")) ||
                    (key.ToLower().EndsWith("tga")) ||
                    (key.ToLower().EndsWith("bmp"))){
                    GameData.GetTexture(key);
                    continue;
                }

                if (key.ToLower().EndsWith("ogg") ||
                    key.ToLower().EndsWith("mp3") ||
                    key.ToLower().EndsWith("wav") ||
                    reader.Type(new MemoryStream(GameData.Data[key])) != "base")
                    continue;

                Base data = GameData.GetData(key);
                if (data.Properties.ContainsKey("istype"))
                    if (data.Properties["istype"].Value.ToLower() == "animation")
                        Processor.Animations.Add(data);
            }

            // Clear fonts
            Fonts.Clear();

            // Load fonts (only fonts added to this internal list can be utilized through data objects)
            Fonts.Add("NormalFont", Content.Load<SpriteFont>("Fonts/NormalFont"));
            Fonts.Add("SpeechFont", Content.Load<SpriteFont>("Fonts/SpeechFont"));
            Fonts.Add("TitleFont", Content.Load<SpriteFont>("Fonts/MenuFont"));
            Fonts.Add("MenuFont", Content.Load<SpriteFont>("Fonts/MenuFont"));
            Fonts.Add("DamageFont", Content.Load<SpriteFont>("Fonts/DamageFont"));
            Fonts.Add("LootFont", Content.Load<SpriteFont>("Fonts/LootFont"));
            Fonts.Add("ChatFont", Content.Load<SpriteFont>("Fonts/ChatFont"));
            Fonts.Add("BigFont", Content.Load<SpriteFont>("Fonts/BigFont"));
            Fonts.Add("HugeFontRed", Content.Load<SpriteFont>("Fonts/HugeFontRed"));
            Fonts.Add("GraphicFontRed", Content.Load<SpriteFont>("Fonts/GraphicFontRed"));
            Fonts.Add("GraphicFontBlue", Content.Load<SpriteFont>("Fonts/GraphicFontBlue"));
            Fonts.Add("GraphicFontSmallBlue", Content.Load<SpriteFont>("Fonts/GraphicFontSmallBlue"));
            Fonts.Add("GraphicFontSmallOrange", Content.Load<SpriteFont>("Fonts/GraphicFontSmallOrange"));

            // Set tooltip font
            Database.TooltipFont = Game.Fonts["ChatFont"];

            // Load fader
            Fader = Content.Load<Texture2D>("Fader");

            // Set up scene
            scene = (Scene)GameData.GetData(Globals.Properties["main"].Value);

            // Process the scene
            scene.Process();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                this.Exit();

            // If fullscreen: force fullscreen
            if (!scene.InterfaceVisible("settings_window") && scene.Fade < 1f)
            {
                if(Application.VisualStyleState != System.Windows.Forms.VisualStyles.VisualStyleState.ClientAndNonClientAreasEnabled)
                    Application.EnableVisualStyles();
                Form gameForm = (Form)Form.FromHandle(Window.Handle);

                if (Database.settings.Fullscreen && Screen.Width < GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                {
                    graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                    Screen.Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    Screen.Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                    graphics.ApplyChanges();
                    gameForm.FormBorderStyle = FormBorderStyle.None;
                    gameForm.WindowState = FormWindowState.Maximized;
                    scene.Process();
                }
                else if (!Database.settings.Fullscreen && gameForm.WindowState != FormWindowState.Normal)
                {
                    gameForm.FormBorderStyle = FormBorderStyle.Fixed3D;
                    gameForm.WindowState = FormWindowState.Normal;
                    graphics.PreferredBackBufferWidth = 800;
                    graphics.PreferredBackBufferHeight = 600;
                    graphics.IsFullScreen = Database.settings.Fullscreen;
                    graphics.ApplyChanges();
                    Screen.Width = 800;
                    Screen.Height = 600;
                    scene.Process();
                }
            }

            // Fix OS cursor
            if (!Database.settings.OSCursor && IsMouseVisible)
                IsMouseVisible = false;
            else if (Database.settings.OSCursor && !IsMouseVisible)
                IsMouseVisible = true;
            Game.ShowMouse = !IsMouseVisible;

            // Update input only if window is active
            if (IsActive)
                Input.Update();

            // Update audio
            Audio.Update(gameTime);

            // Update lights
            Light.Engine.Update(gameTime);

            // Update scene
            scene.Update(gameTime);

            // Set new scene if required
            if (newScene != null)
            {
                scene = newScene;
                scene.Process();
                newScene = null;
            }

            // Exit game if prompted
            if (ExitGame) this.Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Textbox.Visible = false;

            GraphicsDevice.Clear(Color.Black);

            // Prepare the lightmap
            if (scene.Properties["rendermap"].Value.ToLower() == "true") 
                Light.Prepare();

            // Begin drawing
            spriteBatch.Begin();

            // Draw processor first
            if (scene.Properties["rendermap"].Value.ToLower() == "true")
                Processor.Draw();

            // Finish drawing
            spriteBatch.End();

            // Start lightmap drawing
            if (scene.Properties["rendermap"].Value.ToLower() == "true")
            {
                // Start the drawing
                spriteBatch.Begin();

                // Draw the lightmap
                Light.Draw(gameTime);

                // End the drawing
                spriteBatch.End();
            }

            // Start drawing
            spriteBatch.Begin();

            // Draw scene
            scene.Draw();

            // Draw the mouse cursor last if enabled
            if (Game.ShowMouse)
                Game.spriteBatch.Draw(Cursor, new Vector2(Input.Mouse.X, Input.Mouse.Y), Color.White);

            // End drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
