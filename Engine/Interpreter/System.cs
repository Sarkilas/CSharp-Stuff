using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ingenia.Data;
using Ingenia.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Ingenia.Engine
{
    /// <summary>
    /// System interpreter. This class is split into sections.
    /// </summary>
    static partial class Interpreter
    {
        /// <summary>
        /// True if an event successfully executed this frame.
        /// </summary>
        public static bool EventRun { get; set; }

        /// <summary>
        /// Prompts a message display.
        /// </summary>
        private static void Message(Dictionary<string, DataProperty> values)
        {
            // Check if a name is given
            if (values.ContainsKey("name"))
                Temp.Set("message_name", values["name"].Value);
            else
                Temp.Discard("message_name");

            // Check if a face graphic is given
            if (values.ContainsKey("face"))
                Temp.Set("message_face", values["face"].Value);
            else
                Temp.Discard("message_face");

            // Set the message body
            Temp.Set("message_body", values["text"].Value);

            // Set the message box type
            Temp.Set("message_type", Convert.ToInt32(values["boxtype"].Value));

            // Set the message showing flag
            Temp.Set("message_showing", true);
        }

        /// <summary>
        /// Gets whether the event condition is met or not.
        /// </summary>
        public static bool EventConditionMet(DataElement element, Component component = null)
        {
            // Immediately return false if unable to run
            if (EventRun) return false;

            foreach (DataElement e in element.Elements)
            {
                // Ignore if not a condition
                if (e.Name.ToLower() != "condition")
                    continue;

                // Name switch
                DropDown dd;
                switch (e.Properties["name"].Value.ToLower())
                {
                    case "interface hidden":
                        if (Game.scene.InterfaceVisible(e.Properties["interface"].Value))
                            return false;
                        break;
                    case "interface visible":
                        if (!Game.scene.InterfaceVisible(e.Properties["interface"].Value))
                            return false;
                        break;
                    case "selected index":
                        dd = (DropDown)component;
                        if (e.Properties["index"].Integer != dd.SelectedIndex)
                            return false;
                        break;
                    case "selected item":
                        dd = (DropDown)component;
                        if (e.Properties["index"].String != dd.SelectedItem)
                            return false;
                        break;
                    case "tag is null":
                        if (component.Tag != null)
                            return false;
                        break;
                    case "tag is not null":
                        if (component.Tag == null)
                            return false;
                        break;
                    case "save file found":
                        return false; // TODO: Check for save files
                    case "save file not found":
                        break; // TODO: Check for save files
                }
            }

            return true;
        }

        /// <summary>
        /// Executes a component event.
        /// </summary>
        public static void ExecuteEvent(Component component, DataElement element)
        {
            // Set properties
            Dictionary<string, DataProperty> properties = element.Properties;

            // Return if not met
            if (!EventConditionMet(element, component)) return;

            // Set layout
            if (properties.ContainsKey("setlayout"))
            {
                // Get layout key
                string layout = properties["setlayout"].Value;
                if (properties["setlayout"].Value.EndsWith("}"))
                    layout = (string)Script.Run(properties["setlayout"].Value);

                // Get layout data and process it
                Layout data = (Layout)GameData.GetData(layout);
                data.Process();

                // Set the current layout
                Processor.CurrentLayout = data;
            }

            // Set map
            if (properties.ContainsKey("setmap"))
            {
                // Get map key
                string map = properties["setmap"].Value;
                if (properties["setmap"].Value.EndsWith("}"))
                    map = (string)Script.Run(properties["setmap"].Value);

                // Set map
                Processor.CurrentLayout.SetMap(map);
            }

            // Show / hide interfaces
            if (properties.ContainsKey("showinterface"))
            {
                string[] interfaces = properties["showinterface"].Value.Split(',');
                foreach (string i in interfaces)
                    Game.scene.ToggleInterface(i, true);
            }
            if (properties.ContainsKey("hideinterface"))
            {
                string[] interfaces = properties["hideinterface"].Value.Split(',');
                foreach (string i in interfaces)
                    Game.scene.ToggleInterface(i, false);
            }

            // If script property found: execute script
            if (properties.ContainsKey("script"))
                Script.Run(properties["script"].Value);

            // If set scene found: set new scene
            if (properties.ContainsKey("setscene"))
                Game.newScene = (Scene)GameData.GetData(properties["setscene"].Value);

            // If process: execute process command
            if (properties.ContainsKey("process"))
            {
                string[] parameters = new string[0];
                if (properties["process"].Value.Contains('(') && properties["process"].Value.Contains(')'))
                {
                    string temp = properties["process"].Value.Substring(properties["process"].Value.LastIndexOf('(') + 1);
                    temp = temp.Substring(0, temp.Length - 1);
                    parameters = temp.Split(',');
                }
                int last = properties["process"].Value.Length;
                if (properties["process"].Value.LastIndexOf('(') > -1)
                    last = properties["process"].Value.LastIndexOf('(');
                string process = properties["process"].Value.Substring(0, last);
                switch (process.ToLower())
                {
                    
                }
            }

            // If enabled is set: set new flag
            if (properties.ContainsKey("enabled"))
                component.Enabled = properties["enabled"].AsBool;

            // If visible is set: set new flag
            if (properties.ContainsKey("visible"))
                component.Visible = properties["visible"].AsBool;

            // Pause value
            if (properties.ContainsKey("pause"))
                Game.Paused = properties["pause"].AsBool;

            // Play sound if required
            if (properties.ContainsKey("sound"))
                Audio.PlaySound(properties["sound"].Value);
        }

        /// <summary>
        /// Executes a non-component event.
        /// This method should run per event each frame. 
        /// The method manually checks if the event conditions are met.
        /// </summary>
        /// <param name="element">The DataElement that contains the event data.</param>
        public static void ExecuteEvent(DataElement element)
        {
            // Set up necessary variables
            Keys key;

            #region Condition checker
            bool met = false;
            switch (element.Properties["name"].Value.ToLower())
            {
                case "key trigger": // when a key is triggered
                    key = (Keys)Enum.Parse(typeof(Keys), element.Properties["key"].Value);
                    met = Input.Trigger(key);
                    break;
                case "key press": // when a key is pressed (held down)
                    key = (Keys)Enum.Parse(typeof(Keys), element.Properties["key"].Value);
                    met = Input.Pressed(key);
                    break;
                case "left click": // when left mouse button is clicked (once)
                    met = Input.LeftMouse;
                    break;
                case "right click": // when right mouse button is clicked (once)
                    met = Input.LeftMouse;
                    break;
            }
            #endregion

            // Continue the event if condition is met
            if (met)
            {
                // Process all triggers
                foreach (DataElement elem in element.Elements)
                {
                    // Ignore if not a trigger
                    if (elem.Name.ToLower() != "trigger")
                        continue;

                    // Set condition to true
                    bool condition = true;

                    // If condition is found: check it
                    foreach (DataElement e in elem.Elements)
                    {
                        // Ignore if not a condition
                        if (e.Name.ToLower() != "condition")
                            continue;

                        // Name switch
                        switch (e.Properties["name"].Value.ToLower())
                        { 
                            case "interface hidden":
                                condition = !Game.scene.InterfaceVisible(e.Properties["interface"].Value);
                                break;
                            case "interface visible":
                                condition = Game.scene.InterfaceVisible(e.Properties["interface"].Value);
                                break;
                        }
                    }

                    // Trigger name switch
                    if (condition)
                    {
                        switch (elem.Properties["name"].Value.ToLower())
                        {
                            case "toggle interface":
                                Game.scene.ToggleInterface(elem.Properties["interface"].Value);
                                break;
                            case "show interface":
                                Game.scene.ToggleInterface(elem.Properties["interface"].Value, true);
                                break;
                            case "hide interface":
                                Game.scene.ToggleInterface(elem.Properties["interface"].Value, false);
                                break;
                            case "toggle pause":
                                Game.Paused = !Game.Paused;
                                break;
                            case "pause":
                                Game.Paused = true;
                                break;
                            case "unpause":
                                Game.Paused = false;
                                break;
                            case "play sound":
                                Audio.PlaySound(elem.Properties["file"].Value);
                                break;
                            case "set temp value":
                                Temp.Set(elem.Properties["key"].Value, elem.Properties["value"].Value);
                                break;
                        }
                    }
                }
            }
        }
    }
}
