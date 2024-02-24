using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Input;

namespace Ingenia.Engine
{
    [Serializable]
    public class Settings
    {
        // Full screen resolution
        public bool Fullscreen = true;

        // Use OS cursor?
        public bool OSCursor = false;

        // Gamepad enabled
        public bool Gamepad = false;

        // Audio settings
        public float SoundVolume = .8f;
        public float AmbienceVolume = .6f;
        public float MusicVolume = .6f;

        // Core keys
        public Keybinding UpKey = new Keybinding(Keys.W);
        public Keybinding DownKey = new Keybinding(Keys.S);
        public Keybinding LeftKey = new Keybinding(Keys.A);
        public Keybinding RightKey = new Keybinding(Keys.D);

        // Pulse keys
        public Keybinding PulseKey = new Keybinding(Keys.Q);
        public Keybinding SoundKey = new Keybinding(Keys.E);

        // Other settings
        public bool Hints = true;
        public bool ScreenGlow = true;

        // Interaction keys
        public Keybinding InteractKey = new Keybinding(Keys.F);
        public Keybinding DataKey = new Keybinding(Keys.R);

        // Always show status text
        public bool AlwaysStatus = false;

        // Constructor
        public Settings()
        {
        } 

        // Fix key
        public void FixKey(Keybinding key, Keybinding oldKey)
        {
            // Check all keys
            if (UpKey.Equals(key))
                UpKey = oldKey;
            else if (LeftKey.Equals(key))
                LeftKey = oldKey;
            else if (DownKey.Equals(key))
                DownKey = oldKey;
            else if (RightKey.Equals(key))
                RightKey = oldKey;
            else if (PulseKey.Equals(key))
                PulseKey = oldKey;
            else if (SoundKey.Equals(key))
                SoundKey = oldKey;
            else if (InteractKey.Equals(key))
                InteractKey = oldKey;
        }

        // No bind key?
        public bool NoBindKey(Keys key)
        {
            return (key == Keys.Enter ||
                key == Keys.Escape || key == Keys.Tab);
        }

        /// <summary>
        /// Gets a binding by a specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Returns the binding represented by this index.</returns>
        public Keybinding GetBinding(int index)
        {
            switch (index)
            {
                case 0: // Up key
                    return UpKey;
                case 1: // Left key
                    return LeftKey;
                case 2: // Down key
                    return DownKey;
                case 3: // Right key
                    return RightKey;
                case 4: // Healing key
                    return PulseKey;
                case 5: // Item key
                    return InteractKey;
                case 6: // Team chat key
                    return DataKey;
                case 7: // Mana key
                    return SoundKey;
            }

            // Return null
            return null;
        }

        /// <summary>
        /// Binds a new binding.
        /// </summary>
        /// <param name="binding">The new binding.</param>
        /// <param name="index">The binding index. </param>
        public void Bind(Keybinding binding, int index)
        {
            // Index switch
            Keybinding bind = null;
            switch (index)
            {
                case 0: // Up key
                    bind = UpKey;
                    FixKey(binding, bind);
                    UpKey = binding;
                    break;
                case 1: // Left key
                    bind = LeftKey;
                    FixKey(binding, bind);
                    LeftKey = binding;
                    break;
                case 2: // Down key
                    bind = DownKey;
                    FixKey(binding, bind);
                    DownKey = binding;
                    break;
                case 3: // Right key
                    bind = RightKey;
                    FixKey(binding, bind);
                    RightKey = binding;
                    break;
                case 4: // Healing key
                    bind = PulseKey;
                    FixKey(binding, bind);
                    PulseKey = binding;
                    break;
                case 5: // Chat key
                    bind = InteractKey;
                    FixKey(binding, bind);
                    InteractKey = binding;
                    break;
                case 6: // Team chat key
                    bind = DataKey;
                    FixKey(binding, bind);
                    DataKey = binding;
                    break;
                case 7: // Mana key
                    bind = SoundKey;
                    FixKey(binding, bind);
                    SoundKey = binding;
                    break;
            }
        }

        // Load settings
        public static Settings LoadSettings()
        {
            // Filename
            string filename = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                "\\My Games\\Ingenia\\Settings.xml";
            // If file doesn't exist: return
            if (!File.Exists(filename)) return null;
            // Read XML file
            Stream stream = File.Open(filename, FileMode.Open);
            XmlSerializer f = new XmlSerializer(typeof(Settings));
            Settings s = (Settings)f.Deserialize(stream);
            stream.Close();
            return s;
        }

        // Save settings (to XML)
        public void SaveSettings()
        {
            // Filename
            string filename = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                "\\My Games\\Ingenia";
            // If path doesn't exist: create
            if (!Directory.Exists(filename)) Directory.CreateDirectory(filename);
            // If file exists: delete
            if (File.Exists(filename + "\\Settings.xml")) File.Delete(filename + "\\Settings.xml");
            // Write to XML file
            Stream stream = File.Open(filename + "\\Settings.xml", FileMode.Create);
            XmlSerializer f = new XmlSerializer(typeof(Settings));
            f.Serialize(stream, this);
            stream.Close();
        }
    }
}
