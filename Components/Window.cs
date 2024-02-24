using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ingenia.Engine;
using Ingenia.Data;

namespace Ingenia.Interface
{
    /// <summary>
    /// A window for storing and showing components inside.
    /// Windows can be locked or unlocked (dragging enabled when mouse is over the title bar).
    /// </summary>
    public class Window
    {
        // Static window fields
        static Texture2D Graphic = GameData.GetTexture("Rectangle.png");
        static Texture2D Border = GameData.GetTexture("Border.png");
        static SpriteFont Font = Game.Fonts["NormalFont"];

        // Active
        public static bool Open = false;

        // Fields
        public string Title;
        public Rectangle Bounds;
        Color color;

        // Active flag
        public bool Active = true;

        // Dialog flag
        public bool DialogFlag = false;

        /// <summary>
        /// Font replacement if needed.
        /// </summary>
        public SpriteFont FontReplacement;

        // Component list
        Dictionary<string, Component> Components = new Dictionary<string, Component>();

        /// <summary>
        /// Constructs a new window.
        /// </summary>
        /// <param name="title">The title of the window.</param>
        /// <param name="bounds">The bounds of the window.</param>
        /// <param name="color">The color of the window.</param>
        public Window(string title, Rectangle bounds, Color color)
        {
            Title = title;
            Bounds = bounds;
            this.color = color;
        }

        /// <summary>
        /// Clears the pane of components.
        /// </summary>
        public void Clear()
        {
            Components.Clear();
        }

        /// <summary>
        /// The amount of components in this window.
        /// </summary>
        public int ComponentCount
        {
            get
            {
                return Components.Count;
            }
        }

        /// <summary>
        /// Creates a dialog box and returns its Window object.
        /// </summary>
        /// <param name="Type">The DialogType to use.</param>
        /// <param name="color">The Color for the Window and it's components.</param>
        /// <param name="args">Title [, button1, button2..] - Brackets only for custom types.</param>
        /// <returns>Returns a Window object, where buttons in order will have keys from 0 to count-1.</returns>
        public static Window Dialog(Color color, SpriteFont replacement = null, params string[] args)
        {
            // If arguments are empty: return
            if (args.Length == 0) return null;
            // Get title
            string title = args[0];
            // Get size vector
            Vector2 size = Font.MeasureString(title);
            if (replacement != null)
                size = replacement.MeasureString(title);
            // Create components
            List<Component> Components = new List<Component>();
            // Get rectangle bounds
            Rectangle bounds = new Rectangle(Screen.Width / 2 - ((int)size.X / 2 + 32),
                Screen.Height / 2 - ((int)size.Y + 64), (int)size.X + 64, (int)size.Y + 64);
            // Iterate through each button
            Button b;
            int x = bounds.Width - 6, y = bounds.Height - 24;
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (Components.Count > 0)
                {
                    b = (Button)Components[Components.Count - 1];
                    x = b.Bounds.X - 4;
                    y = b.Bounds.Y;
                }
                b = new Button(args[i + 1], new Vector2(x, y), color, 0, true);
                Components.Add(b);
            }
            // Reduce bounds if no buttons
            if (Components.Count == 0)
            {   
                bounds.Height -= 64;
                bounds.Width -= 56;
            }
            // Create the window object
            Window window = new Window(title, bounds, color);
            // Add components
            int a = 0;
            foreach (Component component in Components)
            {
                window.AddComponent(a.ToString(), component);
                a++;
            }
            // Set dialog flag
            window.DialogFlag = true;
            // Set replacement font
            window.FontReplacement = replacement;

            // Return window
            return window;
        }

        /// <summary>
        /// Adds a component to the window. All positions are relative to the window.
        /// </summary>
        /// <param name="key">The string key for later collection.</param>
        /// <param name="component">The component to add.</param>
        public void AddComponent(string key, Component component)
        {
            Components.Add(key, component);
        }

        /// <summary>
        /// Gets a component from the dictionary.
        /// </summary>
        /// <param name="key">The key to collect from.</param>
        /// <returns>Returns the component with the given key.</returns>
        public Component GetComponent(string key)
        {
            return Components[key];
        }

        /// <summary>
        /// Removes a component from the window.
        /// </summary>
        /// <param name="key">The component key to remove.</param>
        public void RemoveComponent(string key)
        {
            if (Components.ContainsKey(key))
                Components.Remove(key);
        }

        /// <summary>
        /// Updates the window and its components.
        /// </summary>
        /// <param name="gameTime">The current GameTime object.</param>
        public void Update(GameTime gameTime)
        {
            if (!Active) return;
            // Set bounds
            Rectangle bounds = Bounds;

            // If not dialog flag: set offset
            if (!DialogFlag)
            {
                bounds.X += (int)Screen.Offset.X;
                bounds.Y += (int)Screen.Offset.Y;
            }
            foreach (Component component in Components.Values)
                component.Update(gameTime, new Vector2(bounds.X, bounds.Y));

            if (Input.MouseOver(Bounds))
                Component.MouseOver = true;
        }

        /// <summary>
        /// Fixes dialog position based on screen bounds.
        /// </summary>
        public void DialogFix()
        {
            Bounds = new Rectangle(Screen.Width / 2 - Bounds.Width / 2, Screen.Height / 2 - Bounds.Height / 2,
                Bounds.Width, Bounds.Height);
        }

        /// <summary>
        /// Draws the window and its components.
        /// </summary>
        /// <param name="spriteBatch">The Spritebatch to draw to.</param>
        public void Draw(SpriteBatch spriteBatch, float fade = 1f, Rectangle? box = null)
        {
            // Return if inactive
            if (!Active) return;

            // Fix bounds if dialog
            if (DialogFlag)
                DialogFix();

            // Set bounds
            Rectangle bounds = box.HasValue ? box.Value : Bounds;

            // Get font
            SpriteFont thisFont = FontReplacement == null ? Font : FontReplacement;

            // Draw the window
            spriteBatch.Draw(Graphic, bounds, (Color.Lerp(color, Color.Black, .7f) * .90f) * fade);

            // Draw borders
            spriteBatch.Draw(Border, new Rectangle(bounds.X - 1, bounds.Y - 1, bounds.Width + 1, 1), color * fade);
            spriteBatch.Draw(Border, new Rectangle(bounds.X - 1, bounds.Y + bounds.Height, bounds.Width + 2, 1), color * fade);
            spriteBatch.Draw(Border, new Rectangle(bounds.X - 1, bounds.Y, 1, bounds.Height), color * fade);
            spriteBatch.Draw(Border, new Rectangle(bounds.X + bounds.Width, bounds.Y, 1, bounds.Height), color * fade);

            // Draw title
            Tools.DrawShadowText(Title, new Vector2(bounds.X + 4, bounds.Y + 2), (color * 1.5f) * fade, Color.Black, thisFont, spriteBatch);

            // Draw all components
            foreach (Component component in Components.Values)
                component.Draw(spriteBatch, new Vector2(bounds.X, bounds.Y));
        }
    }
}
