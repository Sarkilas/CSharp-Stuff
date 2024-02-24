using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ingenia.Processing;
using System.Windows.Forms;
using Ingenia.Engine;

namespace Ingenia.Data
{
    public class Map : Base
    {
        /// <summary>
        /// The core layers.
        /// </summary>
        Dictionary<int, List<Tile>> Layers { get; set; }

        /// <summary>
        /// Tile width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Tile height.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The mapsize (in tiles).
        /// </summary>
        public Vector MapSize { get; set; }

        /// <summary>
        /// The tileset graphic.
        /// </summary>
        private Texture2D Graphic { get; set; }

        /// <summary>
        /// The fog object.
        /// </summary>
        private Fog Fog { get; set; }

        /// <summary>
        /// List of events for this map.
        /// </summary>
        public List<Event> Events { get; set; }

        /// <summary>
        /// Initializes a new map object.
        /// </summary>
        public Map()
            : base()
        {
            // Set required properties
            Required.Add("tileset");
            Required.Add("width");
            Required.Add("height");

            // Set tile sizes
            Width = Height = 32;

            // Initialize layers
            Layers = new Dictionary<int, List<Tile>>();

            // Initialize the events list
            Events = new List<Event>();
        }

        /// <summary>
        /// Processes the map data into more convenient structure and faster for loading.
        /// </summary>
        public void Process()
        {
            // Clear all lights
            Light.Engine.Lights.Clear();

            // Load tileset
            Graphic = GameData.GetTexture(Properties["tileset"].Value);

            // Get map size
            MapSize = new Vector(Properties["width"].Integer, Properties["height"].Integer);


            // Set ambient light if asked
            if (Properties.ContainsKey("ambientlight"))
            {
                string[] rgb = Properties["ambientlight"].Value.Split(',');
                if (rgb.Length == 3)
                {
                    Light.Engine.AmbientColor = new Color(
                        Convert.ToInt32(rgb[0]),
                        Convert.ToInt32(rgb[1]),
                        Convert.ToInt32(rgb[2]));
                }
                else
                    throw new Exception("Your ambient light color must contain 3 components!");
            } else
                // Set ambient light to pure white
                Light.Engine.AmbientColor = Color.White;

            #region Process all map layers
            // Create all layers
            for (int i = 0; i < 6; i++)
                Layers.Add(i, new List<Tile>());
            foreach (DataElement element in Elements)
            {
                // Ignore if not a chunk element
                if (element.Name.ToLower() != "layer")
                    continue;

                // Set up tileset
                Tileset tileset = new Tileset("", Width, Height);

                // Temporary tile
                Tile tile;

                // Go through all elements
                foreach (DataElement elem in element.Elements)
                {
                    // If tile: add to list
                    if (elem.Name.ToLower() == "tile")
                    {
                        string[] coords = elem.Properties["tile"].Value.Replace(" ", String.Empty).Split(',');
                        if (elem.Properties.ContainsKey("fill"))
                            tile = new Tile(new Vector2(Convert.ToInt32(coords[0]), Convert.ToInt32(coords[1])), Vector2.Zero);
                        else
                            tile = new Tile(new Vector2(Convert.ToInt32(coords[0]), Convert.ToInt32(coords[1])), new Vector2(
                                    Convert.ToInt32(elem.Properties["x"].Value), Convert.ToInt32(elem.Properties["y"].Value)));

                        // Add width and height if given
                        // These properties only increase the amount of tiles of this type are drawn next to the origin X,Y point
                        if (elem.Properties.ContainsKey("width"))
                            tile.Width = Convert.ToInt32(elem.Properties["width"].Value);
                        if (elem.Properties.ContainsKey("height"))
                            tile.Height = Convert.ToInt32(elem.Properties["height"].Value);

                        // Add priority if given (drawing priority)
                        if (elem.Properties.ContainsKey("priority"))
                            tile.Priority = Convert.ToInt32(elem.Properties["priority"].Value);

                        // Add passable flag if given
                        if (elem.Properties.ContainsKey("passable"))
                            tile.Passable = Convert.ToBoolean(elem.Properties["passable"].Value);

                        // Add fill flag if given
                        if (elem.Properties.ContainsKey("fill"))
                            tile.Fill = Convert.ToBoolean(elem.Properties["fill"].Value);

                        // Add to appropriate layer
                        Layers[tile.Priority].Add(tile);
                    }
                }
            }
            #endregion

            #region Process fog
            foreach (DataElement element in Elements)
            {
                // Ignore if not a fog element
                if (element.Name.ToLower() != "fog")
                    continue;

                // Create the fog object
                Fog = new Fog();

                // Get graphic
                Fog.Graphic = GameData.GetTexture(element.Properties["graphic"].Value);

                // Get move values
                if (element.Properties.ContainsKey("movex"))
                    Fog.MoveX = Convert.ToSingle(element.Properties["movex"].Value);
                if (element.Properties.ContainsKey("movey"))
                    Fog.MoveY = Convert.ToSingle(element.Properties["movey"].Value);

                // Get opacity value
                Fog.Opacity = Convert.ToInt32(element.Properties["opacity"].Value);
            }
            #endregion
        }

        /// <summary>
        /// Updates the map.
        /// </summary>
        /// <param name="gameTime">A snapshot of game timing values.</param>
        public void Update(GameTime gameTime)
        {
            // Fix camera
            if (Screen.Camera.X < 0)
                Screen.Camera.X = 0;
            if (Screen.Camera.Y < 0)
                Screen.Camera.Y = 0;
            if (Screen.Camera.X > MapSize.X * Width - Screen.Width)
                Screen.Camera.X = MapSize.X * Width - Screen.Width;
            if (Screen.Camera.Y > MapSize.Y * Height - Screen.Height)
                Screen.Camera.X = MapSize.Y * Height - Screen.Height;
        }

        /// <summary>
        /// Gets the flag if the current position is passable or not.
        /// </summary>
        /// <param name="hitbox">The hitbox rectangle.</param>
        /// <returns>Returns true if passable, false if not.</returns>
        public bool Passable(Rectangle hitbox)
        {
            // Check if out of bounds
            if (hitbox.X >= MapSize.X * Width - Width) return false;
            if (hitbox.Y >= MapSize.Y * Height - Height) return false;
            if (hitbox.X < 0) return false;
            if (hitbox.Y + Height < 0) return false;

            // Go through the layers
            for (int i = 0; i < Layers.Count; i++)
                foreach (Tile tile in Layers[i])
                    if (!tile.Passable &&
                        hitbox.Intersects(new Rectangle((int)tile.Relative.X * Width, (int)tile.Relative.Y * Height, Width, Height)))
                        return false;

            // Return true
            return true;
        }
        public bool Impassable(Rectangle hitbox)
        {
            return !Passable(hitbox);
        }

        /// <summary>
        /// Draws the middle layer of the map.
        /// </summary>
        public void DrawMiddle()
        {
            // Draw second layer
            foreach (Tile tile in Layers[1])
                tile.Draw(Graphic, Width, Height);
        }

        /// <summary>
        /// Draws the top layer of the map.
        /// </summary>
        public void DrawTop()
        {
            // Draw the rest of the layers
            for (int i = 2; i < 6; i++)
                foreach (Tile tile in Layers[i])
                    tile.Draw(Graphic, Width, Height);

            // Draw fog
            if (Fog != null)
                Fog.Draw();
        }

        /// <summary>
        /// Draws the bottom layer of the map.
        /// </summary>
        public void DrawBottom()
        {
            // Clear lights
            Light.Engine.Lights.Clear();

            // Draw bottom layer first
            foreach (Tile tile in Layers[0])
                tile.Draw(Graphic, Width, Height);
        }
    }

    class MapChunk
    {
        public List<Tile> Tiles = new List<Tile>();
        public Vector2 Origin;
        public Tileset Tileset;

        public MapChunk(Vector2 origin, Tileset tileset, params Tile[] tiles)
        {
            Origin = origin; Tileset = tileset;
            Tiles = tiles.ToList();
        }

        public void Draw(Texture2D Graphic)
        {
            // Sort all tiles by priority
            Dictionary<int, List<Tile>> tiles = new Dictionary<int, List<Tile>>();

            // Add all tiles to the list
            foreach (Tile tile in Tiles)
            {
                if (!tiles.ContainsKey(tile.Priority))
                    tiles.Add(tile.Priority, new List<Tile>());
                tiles[tile.Priority].Add(tile);
            }

            // Sort keys
            List<int> list = tiles.Keys.ToList();
            list.Sort();

            // Draw all tiles
            foreach(int key in list)
                foreach (Tile tile in tiles[key])
                {
                    // If fill tile: fill screen
                    if (tile.Fill)
                    {
                        int cx = (int)Screen.Camera.X / Tileset.Width - 4,
                            cy = (int)Screen.Camera.Y / Tileset.Height - 4;
                        for (int x = cx; x < cx + (Screen.Width / Tileset.Width + 6); x++)
                        {
                            for (int y = cy; y < cy + (Screen.Height / Tileset.Height + 7); y++)
                            {
                                // Get rectangles
                                Rectangle source = new Rectangle((int)tile.Location.X * Tileset.Width,
                                    (int)tile.Location.Y * Tileset.Height, Tileset.Width, Tileset.Height),
                                    destination = new Rectangle(x * Tileset.Width - (int)Screen.Camera.X,
                                        y * Tileset.Height - (int)Screen.Camera.Y, Tileset.Width, Tileset.Height);

                                // Continue if not on screen
                                if (!Screen.OnScreen(destination)) continue;

                                // Draw the actual tile
                                Game.spriteBatch.Draw(Graphic, destination, source, Color.White);
                            }
                        }
                    }
                    else
                    {
                        // Get rectangles
                        Rectangle source = new Rectangle((int)tile.Location.X * Tileset.Width, (int)tile.Location.Y * Tileset.Height,
                            Tileset.Width, Tileset.Height),
                            destination = new Rectangle((int)Origin.X * Tileset.Width + (int)tile.Relative.X * Tileset.Width - (int)Screen.Camera.X,
                                (int)Origin.Y * Tileset.Height + (int)tile.Relative.Y * Tileset.Height - 
                                (int)Screen.Camera.Y, Tileset.Width, Tileset.Height);

                        // Continue if not on screen
                        if (!Screen.OnScreen(destination)) continue;

                        // Draw the actual tile
                        Game.spriteBatch.Draw(Graphic, destination, source, Color.White);
                    }
                }
        }
    }

    class Tileset
    {
        public string Graphic;
        public int Width;
        public int Height;

        public Tileset(string graphic, int width, int height)
        {
            Graphic = graphic; Width = width; Height = height;
        }
    }

    class Tile
    {
        public Vector2 Location;
        public Vector2 Relative;
        public int Width = 1;
        public int Height = 1;
        public int Priority = 0;
        public bool Passable = true;
        public bool Fill = false;

        public Tile(Vector2 tile, Vector2 relative)
        {
            Location = tile; Relative = relative;
        }

        public void Draw(Texture2D Graphic, int Width, int Height)
        {
            // If fill tile: fill screen
            if (Fill)
            {
                int cx = (int)Screen.Camera.X / Width - 4,
                    cy = (int)Screen.Camera.Y / Height - 4;
                for (int x = cx; x < cx + (Screen.Width / Width + 6); x++)
                {
                    for (int y = cy; y < cy + (Screen.Height / Height + 7); y++)
                    {
                        // Get rectangles
                        Rectangle source = new Rectangle((int)Location.X * Width,
                            (int)Location.Y * Height, Width, Height),
                            destination = new Rectangle(x * Width - (int)Screen.Camera.X,
                                y * Height - (int)Screen.Camera.Y, Width, Height);

                        // Continue if not on screen
                        if (!Screen.OnScreen(destination)) continue;

                        // Draw the actual tile
                        Game.spriteBatch.Draw(Graphic, destination, source, Color.White);
                    }
                }
            }
            else
            {
                // Get rectangles
                Rectangle source = new Rectangle((int)Location.X * Width, (int)Location.Y * Height,
                    Width, Height),
                    destination = new Rectangle((int)Relative.X * Width - (int)Screen.Camera.X,
                        (int)Relative.Y * Height -
                        (int)Screen.Camera.Y, Width, Height);

                // Continue if not on screen
                if (!Screen.OnScreen(destination)) return;

                // Draw the actual tile
                Game.spriteBatch.Draw(Graphic, destination, source, Color.White);
            }
        }
    }

    class Fog
    {
        public Texture2D Graphic;
        public float MoveX;
        public float MoveY;
        public int Opacity;
        public Vector2 Offset = Vector2.Zero;

        public void Draw()
        {
            // Get opacity
            float opacity = (float)Opacity / 255f;

            // Move the fog
            Offset += new Vector2(MoveX, MoveY);

            // Check if X and Y have reached bounds
            if (Offset.X >= Graphic.Width)
                Offset.X -= Graphic.Width;
            if (Offset.Y >= Graphic.Height)
                Offset.Y -= Graphic.Height;

            // Find amount of fogs vertically and horizontally
            int topX = (int)Math.Ceiling(Screen.Camera.X / Graphic.Width),
                topY = (int)Math.Ceiling(Screen.Camera.Y / Graphic.Height),
                maxX = (int)Math.Ceiling((decimal)Screen.Width / Graphic.Width),
                maxY = (int)Math.Ceiling((decimal)Screen.Height / Graphic.Height);
            maxX++; maxY++;

            // Fix top X
            while (topX > 0){
                topX--; maxX++;
            }
            while (topY > 0){
                topY--; maxY++;
            }

            // Offset fix
            if (Offset.X > 0)
                topX--; maxX++;
            if (Offset.Y > 0)
                topY--; maxY++;

            // Draw fogs
            for (int i = 0; i < maxX; i++)
            {
                for (int a = 0; a < maxY; a++)
                {
                    float x = (float)(topX + i) * Graphic.Width;
                    float y = (float)(topY + a) * Graphic.Height;
                    Game.spriteBatch.Draw(Graphic, new Vector2(x - Screen.Camera.X + Offset.X, y - 
                        Screen.Camera.Y + Offset.Y), Color.White * opacity);
                }
            }
        }
    }

    public class Node
    {
        public string Key;
        public Vector Location;
        public string Light = null;
        public Light Illumination = null;
        public bool Passable = true;
        Animation Animation = null;
        public Vector2 LightOffset = Vector2.Zero;
        public Vector2 NodeOffset = Vector2.Zero;
        public string Link = null;
        public List<DataElement> Elements = new List<DataElement>();
        public int Custom = -1;

        public Node(string key, Vector location)
        {
            Key = key; Location = location;
            switch (key)
            {
                case "dungeon_torch":
                    Animation = new Animation("Torch.png", 4);
                    break;
            }
        }

        public Rectangle Hitbox(int width, int height)
        {
            if (Animation != null)
                return new Rectangle(Location.X * width + (int)NodeOffset.X, Location.Y * height + 
                    height - Animation.Height + Animation.Height / 3 + (int)NodeOffset.Y,
                    Animation.Width, Animation.Height - Animation.Height / 3);
            else
                return new Rectangle(Location.X * width, Location.Y * height, width, height);
        }

        public void Update(GameTime gameTime)
        {
            if (Animation != null)
                Animation.Update(gameTime);
        }

        public void Draw(int width, int height)
        {
            // Draw light if active
            if (Light != null)
                Ingenia.Engine.Light.DrawLight(Light, new Vector2(Location.X * width + 16, 
                    Location.Y * height - 20) - Screen.Camera + LightOffset);

            // If light is valid: draw it
            if(Illumination != null)
                Illumination.Draw(new Vector2(Location.X * width + 16, Location.Y * height - 20) - Screen.Camera + LightOffset);

            if (Animation != null)
            {
                Vector2 position = new Vector2(Location.X * width + width / 2 -
                    Animation.Width / 2, Location.Y * height + height - Animation.Height) + NodeOffset - Screen.Camera;
                if(Screen.OnScreen(new Rectangle((int)position.X, (int)position.Y, Animation.Width, Animation.Height)))
                    Animation.Draw(Game.spriteBatch, position);
            }
        }
    }

    class Panorama
    {
    }
}