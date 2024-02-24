using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Ingenia
{
    /// <summary>
    /// A temporary class used for storing temporary data that is quickly discarded.
    /// </summary>
    public static class Temp
    {
        /// <summary>
        /// The collection of temporary variables.
        /// 
        /// A list of used variables that affects gameplay:
        /// 
        ///     * message_showing : True if a message box is currently active. False or null if not.
        ///     * message_name : Null of no name is given. A real string if given.
        ///     * message_body : The body text of the message.
        ///     * message_type : The message box type.
        ///     * burst_power : Represents the current burst power as an Integer from 0 to 5
        ///     * force_move : True if a player or entity is being forced a custom movement. This should lock player controls if true.
        ///     * wait_frames : If above 0 the Interpreter halts the game until the counter reaches 0.
        /// </summary>
        static Hashtable Collection = new Hashtable();

        /// <summary>
        /// Gets a temporary variable from the collection.
        /// </summary>
        /// <param name="key">The key to collect data from.</param>
        /// <returns>Returns the object of the given key. Returns null if key does not exist.</returns>
        public static object Get(string key)
        {
            return Collection.ContainsKey(key) ? Collection[key] : null;
        }

        /// <summary>
        /// Sets a temporary key to an object value.
        /// </summary>
        /// <param name="key">The key to use.</param>
        /// <param name="value">The object to use.</param>
        public static void Set(string key, object value)
        {
            if(Collection.ContainsKey(key))
                Collection[key] = value;
            else
                Collection.Add(key, value);
        }

        /// <summary>
        /// Discards a given key from the collection.
        /// </summary>
        /// <param name="key">The given key.</param>
        public static void Discard(string key)
        {
            if(Collection.ContainsKey(key))
                Collection.Remove(key);
        }
    }
}
