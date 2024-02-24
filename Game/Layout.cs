using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ingenia.Data;
using Microsoft.Xna.Framework;

namespace Ingenia.Engine
{
    /// <summary>
    /// The layout class. Stores and handles layout links and data for maps etc.
    /// </summary>
    public class Layout : Base
    {
        /// <summary>
        /// The list of maps linked to this layout.
        /// </summary>
        public List<Map> Maps { get; set; }

        /// <summary>
        /// The current active map index for this layout.
        /// </summary>
        public int MapIndex { get; set; }

        /// <summary>
        /// The current active map.
        /// </summary>
        public Map CurrentMap { get { return MapIndex >= 0 ? Maps[MapIndex] : null; } }

        /// <summary>
        /// Constructs a layout object.
        /// </summary>
        public Layout()
            : base()
        {
            // Initialize the map list
            Maps = new List<Map>();

            // Set map index to -1
            MapIndex = -1;
        }

        /// <summary>
        /// Processes the layout base data into logical data.
        /// </summary>
        public void Process()
        {
            // Process all maps
            foreach (DataElement map in Elements)
            {
                // If not a map element: continue
                if (map.Name.ToLower() != "map") continue;

                // Load map
                Map data = (Map)GameData.GetData(map.Properties["key"].String);

                // If active: process and set index
                if(map.Properties.ContainsKey("active"))
                    if (map.Properties["active"].Boolean)
                    {
                        data.Process();
                        MapIndex = Maps.Count;
                    }

                // Add all events to the map
                foreach (DataElement e in map.Elements)
                {
                    // If not an event element: continue
                    if (e.Name.ToLower() != "event") continue;

                    // Load event
                    Event evnt = (Event)GameData.GetData(e.Properties["key"].Value);

                    // Process the event
                    evnt.Process();

                    // Add to map
                    data.Events.Add(evnt);
                }

                // Add map to list
                Maps.Add(data);
            }
        }

        /// <summary>
        /// Set active map to a current key.
        /// Must be found within this layout, or it will throw an exception.
        /// </summary>
        /// <param name="key">The map key to set to.</param>
        public void SetMap(string key)
        {
            // Iterate through maps
            for(int i = 0; i < Maps.Count; i++)
                if (Maps[i].Key.ToLower() == key.ToLower())
                {
                    Maps[i].Process();
                    MapIndex = i;
                    return;
                }

            // No map was found: throw exception
            throw new KeyNotFoundException("The map key '" + key + "' is not valid for the layout '" + Key + "'.");
        }

        /// <summary>
        /// Updates the layout object.
        /// </summary>
        /// <param name="gameTime">A snapshot of game timing values.</param>
        public void Update(GameTime gameTime)
        {
            // Update the current map
            if(CurrentMap != null)
                CurrentMap.Update(gameTime);
        }

        /// <summary>
        /// Draws the layout object.
        /// </summary>
        public void Draw()
        {
            // Draw map if required
            if (CurrentMap != null)
            {
                // Draw the bottom layer
                CurrentMap.DrawBottom();

                // TODO: Draw bottom part of units

                // Draw the middle layer
                CurrentMap.DrawMiddle();

                // TODO: Draw the top part of units

                // Draw the top layer
                CurrentMap.DrawTop();
            }
        }
    }
}
