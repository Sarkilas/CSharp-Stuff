using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ingenia
{
    /// <summary>
    /// The switches class.
    /// </summary>
    static class Switches
    {
        /// <summary>
        /// The switch collection.
        /// </summary>
        static Dictionary<string, bool> Collection = new Dictionary<string, bool>();

        /// <summary>
        /// Sets a switch to a value.
        /// </summary>
        /// <param name="key">The key switch.</param>
        /// <param name="value">The boolean value.</param>
        public static void Set(string key, bool value)
        {
            if (Collection.ContainsKey(key))
                Collection.Remove(key);

            Collection.Add(key, value);
        }
        
        /// <summary>
        /// Resets the switch collection.
        /// </summary>
        public static void Reset() { Collection.Clear(); }

        /// <summary>
        /// Enables a switch.
        /// </summary>
        /// <param name="key">The switch key.</param>
        public static void Enable(string key) { Set(key, true); }

        /// <summary>
        /// Disables a switch.
        /// </summary>
        /// <param name="key">The switch key.</param>
        public static void Disable(string key) { Set(key, false); }

        /// <summary>
        /// Gets a switch value.
        /// </summary>
        /// <param name="key">The key of the switch.</param>
        /// <returns>Returns true if the switch is ON, false if not.</returns>
        public static bool Get(string key)
        {
            return Collection.ContainsKey(key) ? Collection[key] : false;
        }
    }
}
