using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ingenia.Engine
{
    /// <summary>
    /// The player class. Only one instance should ever exist within a game process.
    /// Inherits the character class.
    /// </summary>
    public class Player : Character
    {
        /// <summary>
        /// The maximum life for the player.
        /// </summary>
        public int MaxLife { get { return Properties["life"].Integer; } }

        /// <summary>
        /// Determines how many times the player can take damage before losing.
        /// Will regenerate when out of range. This is the current life.
        /// </summary>
        public int Life { get; set; }

        /// <summary>
        /// The player constructor.
        /// </summary>
        public Player()
            : base()
        {
            Required.Add("life");
        }
    }
}
