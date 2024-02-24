using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Ingenia.Data;
using Ingenia.Interface;

namespace Ingenia.Engine
{
    /// <summary>
    /// Scripting class. Used for script data.
    /// </summary>
    static class Script
    {
        /// <summary>
        /// Runs a script function.
        /// </summary>
        public static object Run(string script)
        {
            // First check if the given script is valid
            Match match = Regex.Match(script, @"{(.+?)}");

            // Throw exception if invalid
            if (!match.Success)
                throw new Exception("The given value (" + script + ") is not a valid script item.");

            // Fix the string
            script = script.Replace("{", String.Empty);
            script = script.Replace("}", String.Empty);

            // Split the script
            string[] joints = script.ToLower().Split('.');

            // If length is 0: return
            if (joints.Length == 0) return null;

            // Check first joint
            if(joints[0] == "game")
            {
                // Invalid script if joints don't proceed
                if(joints.Length < 2)
                    throw new Exception("Script directive 'GAME' needs a second parameter.");

                // Check next joint
                if (joints[1] == "exit")
                {
                    Game.Quit();
                    return null;
                }
            }
            else if (joints[0] == "screen")
            {
                // Invalid script if joints don't proceed
                if (joints.Length < 2)
                    throw new Exception("Script directive 'SCREEN' needs a second parameter.");

                // Switch for second parameter
                switch (joints[1])
                {
                    case "width":
                        return Screen.Width;
                    case "height":
                        return Screen.Height;
                    case "centerx":
                        return Screen.Width / 2;
                    case "centery":
                        return Screen.Height / 2;
                    case "camera":
                        // Invalid script if joints don't proceed
                        if (joints.Length < 3)
                            throw new Exception("Script directive 'CAMERA' needs a second parameter.");

                        // Second switch
                        switch (joints[2])
                        {
                            case "x":
                                return Screen.Camera.X;
                            case "y":
                                return Screen.Camera.Y;
                        }
                        break;
                }
            }
            else if (joints[0] == "strings")
            {
                // Invalid script if joints don't proceed
                if (joints.Length < 2)
                    throw new Exception("Script directive 'STRINGS' needs a second parameter.");

                return new string[] { };
            }
            else if (joints[0] == "tag")
            {
                if (joints.Length < 3)
                    throw new Exception("The 'TAG' directive requires at least 2 parameters.");
                foreach (GameInterface ui in Game.scene.Interfaces)
                    foreach (Component component in ui.Components)
                    {
                        if (joints[1] == (string)component.Tag)
                        {
                            switch (joints[2])
                            {
                                case "text":
                                    return component.Text;
                                case "boolean":
                                    return component.Boolean;
                            }
                        }
                    }
            }
            else if (joints[0] == "temp")
            {
                // Invalid script if joints don't proceed
                if (joints.Length < 2)
                    throw new Exception("Script directive 'TEMP' needs a second parameter.");

                // Return the value
                return Temp.Get(joints[1]);
            }
            else if (joints[0] == "mouse")
            {
                // Invalid script if joints don't proceed
                if (joints.Length < 2)
                    throw new Exception("Script directive 'MOUSE' needs a second parameter.");

                // Parameter switch
                switch (joints[1])
                {
                    case "x":
                        return Input.Mouse.X;
                    case "y":
                        return Input.Mouse.Y;
                    case "position":
                        return new Vector2(Input.Mouse.X, Input.Mouse.Y);
                }
            }

            // Return null
            return null;
        }

        /// <summary>
        /// Executes a script function for this particular character, returning the value required.
        /// </summary>
        /*public static object Execute(this Character character, string script, bool fromscript = false)
        {
            // Return null if character is null
            if (character == null) return null;

            // First check if the given script is valid
            Match match = Regex.Match(script, @"{(.+?)}");

            // Throw exception if invalid
            if (!match.Success)
                throw new Exception("The given value (" + script + ") is not a valid script item.");

            // Fix the string
            script = script.Replace("{", String.Empty);
            script = script.Replace("}", String.Empty);

            // Split the script
            string[] joints = script.ToLower().Split('.');

            // If length is 0: return
            if (joints.Length == 0) return null;

            // If script is not null: return value
            if (!fromscript && joints[0] != "self" && joints[0] != "stats")
                if (Run("{" + script + "}") != null)
                    return Run("{" + script + "}");

            // Return data for different directives
            /*if (joints[0] == "self")
            {
                if (joints.Length < 2)
                    throw new Exception("Script directive 'SELF' requires a second parameter.");
                
                switch (joints[1])
                {
                    case "x":
                        return character.Origin.X;
                    case "y":
                        return character.Origin.Y;
                    case "name":
                        return character.Name;
                    case "graphic":
                        if (joints.Length < 3)
                            throw new Exception("Script directive 'GRAPHIC' requires a second parameter.");
                        if (joints[2] == "name")
                            return character.Animation.image;
                        if (joints[2] == "animation")
                            return character.Animation;
                        if (joints[2] == "width")
                            return character.Animation.Width;
                        if (joints[2] == "height")
                            return character.Animation.Height;
                        return null;
                    case "hardcore_flag":
                        if (character.GetType() == typeof(Player))
                        {
                            Player player = (Player)character;
                            return player.Hardcore ? Game.Globals.Properties["hardcore"].String : "";
                        }
                        return "";
                    case "playtime":
                        if (character.GetType() == typeof(Player))
                        {
                            Player player = (Player)character;
                            return String.Format("{0:00}", player.PlayTime.Hours) + ":" +
                                String.Format("{0:00}", player.PlayTime.Minutes) + ":" + 
                                String.Format("{0:00}", player.PlayTime.Seconds);
                        }
                        return "";
                }
            }

            // Return null
            return null;
        }*/
    }
}
