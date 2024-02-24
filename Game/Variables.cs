using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ingenia
{
    /// <summary>
    /// The switches class.
    /// </summary>
    static class Variables
    {
        /// <summary>
        /// The switch collection.
        /// </summary>
        static Dictionary<string, int> Collection = new Dictionary<string, int>();

        /// <summary>
        /// Sets a variable to a value.
        /// </summary>
        /// <param name="key">The key variable.</param>
        /// <param name="value">The integer value.</param>
        public static void Set(string key, int value)
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
        /// Gets a switch value.
        /// </summary>
        /// <param name="key">The key of the variable.</param>
        /// <returns>Returns the value. Non-defined variables always return 0.</returns>
        public static int Get(string key)
        {
            return Collection.ContainsKey(key) ? Collection[key] : 0;
        }
    }
}
