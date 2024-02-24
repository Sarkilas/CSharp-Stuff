using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ingenia.Data;

namespace Ingenia.Engine
{
    /// <summary>
    /// Core feature interpreter. This class is split into sections.
    /// </summary>
    static partial class Interpreter
    {
        public static bool LockControls = false;

        /// <summary>
        /// Gets the interpreter running state.
        /// </summary>
        public static bool Running
        {
            get {
                return false;
            }
        }

        /// <summary>
        /// Updates the interpreter.
        /// </summary>
        /// <param name="gameTime">A snapshot of game timings.</param>
        /// <returns>Returns true if not interrupting gameplay. False if it does.</returns>
        public static bool Update(GameTime gameTime, bool map = false)
        {
            // Return if map is false
            if (!map) return true;

            // Check for game halt (waiting)
            if (Temp.Get("wait_frames") != null)
            {
                int wait = (int)Temp.Get("wait_frames");
                if (wait > 0)
                {
                    // Reduce counter by elapsed time
                    Temp.Set("wait_frames", wait - gameTime.ElapsedGameTime.Milliseconds);
                    if (wait - gameTime.ElapsedGameTime.Milliseconds > 0) return false;
                }
            }

            // Return false
            return false;
        }
    }
}
