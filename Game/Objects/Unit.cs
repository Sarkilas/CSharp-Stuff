using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ingenia.Engine
{
    /// <summary>
    /// The unit class. Holds unit specific data.
    /// </summary>
    public class Unit : Character
    {
        /// <summary>
        /// The current sight range for this unit (measured in pixel radius).
        /// </summary>
        public int Sight { get { return Properties["sight"].Integer; } }

        /// <summary>
        /// The time remaining for this unit to be disabled.
        /// </summary>
        private TimeSpan _disableTime = TimeSpan.Zero;

        /// <summary>
        /// True if this unit currently is disabled by power pulse.
        /// </summary>
        public bool Disabled { get { return _disableTime > TimeSpan.Zero; } }

        /// <summary>
        /// The resistance of this unit. 
        /// Determines how resistant this unit is to power pulse.
        /// 1.0 = not at all resistant
        /// 0.5 = 50% resistant (half duration)
        /// </summary>
        public float Resistance { get { return Properties["resistance"].Float; } }

        /// <summary>
        /// The time this unit will be disabled when struck by power pulse.
        /// </summary>
        private TimeSpan DisableTime { get { return TimeSpan.FromSeconds(5 * Resistance); } }

        /// <summary>
        /// Constructs a new unit.
        /// </summary>
        public Unit()
            : base()
        {
            Required.Add("sight");
            Required.Add("resistance");
        }

        /// <summary>
        /// Processes this unit.
        /// </summary>
        public override void Process()
        {
            // Process the character
            base.Process();
        }

        /// <summary>
        /// Disables this unit when struck by power pulse.
        /// </summary>
        public void Disable()
        {
            // Set disable time
            _disableTime = DisableTime;
        }

        /// <summary>
        /// Updates this unit per frame.
        /// </summary>
        /// <param name="gameTime">A snapshot of game timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Update the character class
            base.Update(gameTime);

            // Alter disable time when required
            if (_disableTime > TimeSpan.Zero)
                _disableTime -= gameTime.ElapsedGameTime;
        }
    }
}
