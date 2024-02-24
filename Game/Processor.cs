using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ingenia.Data;

namespace Ingenia.Engine
{
    /// <summary>
    /// The processor for the game.
    /// </summary>
    public class Processor
    {
        /// <summary>
        /// The list of animations.
        /// </summary>
        public static List<Base> Animations { get; set; }

        /// <summary>
        /// The current layout active in the processor.
        /// Only stores one layout at a time to avoid extra memory usage.
        /// </summary>
        public static Layout CurrentLayout { get; set; }

        /// <summary>
        /// Initializes the processor.
        /// </summary>
        public static void Initialize()
        {
            // Initialize the animation set
            Animations = new List<Base>();
        }

        /// <summary>
        /// Updates the processor.
        /// </summary>
        public static void Update()
        {
        }

        /// <summary>
        /// Draws the processor content to the screen.
        /// </summary>
        public static void Draw()
        {
            // Draw the current layout (unless null)
            if(CurrentLayout != null)
                CurrentLayout.Draw();
        }
    }
}
