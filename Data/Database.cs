using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Ingenia.Engine
{
    public static class Database
    {
        // Scale up
        public static int Scale = 40;
        // Master colors
        public static Color WindowColor = Color.White;
        // Settings 
        public static Settings settings;
        public static bool NewSettings = false;
        // Display modes
        public static List<DisplayMode> DisplayModes = new List<DisplayMode>();
        public static int DisplayIndex = 0;
        // Game instance
        public static Game game;
        // Tooltip font
        public static SpriteFont TooltipFont;
        static bool Fixed = false;

        // Load database
        public static void Load(ContentManager Content, Game game)
        {
            Database.game = game;

            #region Loading Core
            // Create settings
            settings = Settings.LoadSettings();
            if (settings == null) { settings = new Settings(); NewSettings = true; }
            #endregion
        }

        private static bool ContainsMode(DisplayMode mode)
        {
            foreach (DisplayMode m in DisplayModes)
                if (m.Width == mode.Width && m.Height == mode.Height)
                    return true;
            return false;
        }
    }
}
