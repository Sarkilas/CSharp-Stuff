using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ingenia.Processing;
using Ingenia.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ingenia.Engine;

namespace Ingenia.Data
{
    public class Scene : Base
    {
        /// <summary>
        /// The list of scene interfaces.
        /// </summary>
        public List<GameInterface> Interfaces = new List<GameInterface>();

        /// <summary>
        /// The specific event list. This is to prevent cycling through useless elements to process events.
        /// </summary>
        List<DataElement> Events = new List<DataElement>();

        /// <summary>
        /// The fader float value.
        /// </summary>
        public float Fade = 1f;

        /// <summary>
        /// Constructs a scene object.
        /// </summary>
        public Scene()
            : base()
        {
            Required.Add("rendermap");
        }

        /// <summary>
        /// Processes the scene interfaces into more convenient formats.
        /// </summary>
        public void Process()
        {
            // Clear interfaces
            Interfaces.Clear();

            // Clear events
            Events.Clear();

            // Go through each interface
            foreach (DataElement element in Elements)
            {
                // Continue if not an interface element
                if (element.Name.ToLower() != "interface")
                    continue;

                // Load interface
                GameInterface data = (GameInterface)GameData.GetData(element.Properties["key"].Value);

                // Get relative vector
                Vector2 vector = Vector2.Zero;
                if(element.Properties.ContainsKey("x"))
                    vector.X = Convert.ToInt32(element.Properties["x"].Value);
                if(element.Properties.ContainsKey("y"))
                    vector.Y = Convert.ToInt32(element.Properties["y"].Value);

                // Process the interface
                data.Process(Convert.ToBoolean(element.Properties["visible"].Value), vector);

                // Add the interface
                Interfaces.Add(data);
            }

            // Go through each event
            foreach (DataElement element in Elements)
            {
                // Continue if not an interface element
                if (element.Name.ToLower() != "event")
                    continue;

                // Add the event to the list
                Events.Add(element);
            }

            // Play music if asked
            if (Properties.ContainsKey("music"))
                Audio.PlayMusic(Properties["music"].String);
        }

        /// <summary>
        /// Toggles an interface visibility.
        /// </summary>
        public void ToggleInterface(string name, bool visible)
        {
            foreach (GameInterface obj in Interfaces)
                if (obj.Properties["key"].Value == name)
                    obj.Visible = visible;
        }
        public void ToggleInterface(string name)
        {
            foreach (GameInterface obj in Interfaces)
                if (obj.Properties["key"].Value == name)
                    obj.Visible = !obj.Visible;
        }

        /// <summary>
        /// Gets the visibility state of an interface.
        /// </summary>
        /// <param name="name">The interface name.</param>
        /// <returns>Returns true if visible, false if not.</returns>
        public bool InterfaceVisible(string name)
        {
            foreach (GameInterface obj in Interfaces)
                if (obj.Properties["key"].Value == name)
                    return obj.Visible;

            return false;
        }

        /// <summary>
        /// Updates the scene.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Update the interface objects
            foreach (GameInterface obj in Interfaces)
                obj.Update(gameTime);

            // Update any events
            foreach (DataElement element in Events)
                Interpreter.ExecuteEvent(element);

            // Set flag
            Interpreter.LockControls = false;
        }

        /// <summary>
        /// Gets the text from a textbox control if found.
        /// The textbox must have a key property provided to work.
        /// </summary>
        /// <param name="key">The key to look for.</param>
        /// <returns>Returns the text of the control, or null if none was found.</returns>
        public string GetTextFromControl(string key)
        {
            // Check all controls
            foreach(GameInterface obj in Interfaces)
                foreach(Component component in obj.Components)
                    if (component.GetType() == typeof(Textbox) &&
                        component.Key == key)
                    {
                        Textbox box = (Textbox)component;
                        return box.Text;
                    }

            // Return null
            return null;
        }

        /// <summary>
        /// Draws the scene.
        /// </summary>
        public void Draw()
        {
            // Draw all the interfaces
            foreach (GameInterface obj in Interfaces)
                obj.Draw();

            // Fade the scene if asked
            if (Convert.ToBoolean(Properties["enterfade"].Value))
                Game.spriteBatch.Draw(Game.Fader, new Rectangle(0, 0, Screen.Width, Screen.Height), Color.Black * Fade);

            // Increase fade if not maxed
            if (Fade > 0f)
                Fade -= .025f;
        }
    }
}
