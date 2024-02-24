using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ingenia.Interface;
using Ingenia.Engine;

namespace Ingenia.Data
{
    /// <summary>
    /// This is the data structure object for game interfaces.
    /// </summary>
    public class GameInterface : Base
    {
        /// <summary>
        /// The interface components. Only available after processing an interface.
        /// </summary>
        public List<Component> Components = new List<Component>();

        /// <summary>
        /// The windows. Only available after processing the interface.
        /// </summary>
        List<Window> Windows = new List<Window>();

        /// <summary>
        /// The visible flag.
        /// </summary>
        bool _visible; bool _updated = true;
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (!_visible && value)
                {
                    foreach (Component component in Components)
                        if (component.GetType() == typeof(Textbox))
                        {
                            Textbox box = (Textbox)component;
                            box.Text = null;
                        }
                    _updated = false;
                }
                _visible = value;
            }
        }

        /// <summary>
        /// The relative position.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Constructs an interface object.
        /// </summary>
        public GameInterface()
            : base()
        {
        }

        /// <summary>
        /// Processes the interface into visual components.
        /// </summary>
        public void Process(bool visible, Vector2 position)
        {
            // Process all the components
            foreach (DataElement element in Elements)
            {
                // Set up component
                Component component = null;

                // Component type switch
                Vector2 pos; Color color;
                switch (element.Properties["type"].Value.ToLower())
                {
                    case "image":
                        bool centerx = false, centery = false;
                        if (element.Properties.ContainsKey("centerx"))
                            centerx = element.Properties["centerx"].AsBool;
                        if (element.Properties.ContainsKey("centery"))
                            centery = element.Properties["centery"].AsBool;
                        Texture2D file = null; Animation animation = null;
                        if (element.Properties["file"].Value.EndsWith("}"))
                            if (Script.Run(
                                element.Properties["file"].Value).GetType() == typeof(String))
                                file = GameData.GetTexture(Script.Run(
                                    element.Properties["file"].Value).ToString());
                            else
                                animation = (Animation)Script.Run(element.Properties["file"].Value);
                        else
                            file = GameData.GetTexture(element.Properties["file"].Value);
                        int cx = element.Properties["x"].Integer,
                            cy = element.Properties["y"].Integer;
                        if(file != null)
                            component = new ImageComponent(file, new Vector2(cx, cy), centerx, centery);
                        else
                            component = new ImageComponent(animation, new Vector2(cx, cy), centerx, centery);
                        break;

                    case "image scroller":
                        component = new ImageScroller(GameData.GetTexture(element.Properties["file"].Value),
                            new Vector2(Convert.ToSingle(element.Properties["x"].Value),
                                Convert.ToSingle(element.Properties["y"].Value)), 
                                element.Properties["scrollx"].AsFloat, element.Properties["scrolly"].AsFloat,
                                element.Properties["opacity"].AsFloat);
                        break;

                    case "link":
                        bool center = false;
                        if (element.Properties.ContainsKey("center"))
                            center = Convert.ToBoolean(element.Properties["center"].Value);
                        color = Color.White;
                        if (element.Properties.ContainsKey("color"))
                        {
                            string[] joints = element.Properties["color"].Value.Split(',');
                            color = new Color(int.Parse(joints[0]), int.Parse(joints[1]), int.Parse(joints[2]));
                        }
                        bool adj = false;
                        pos = new Vector2(element.Properties["x"].Integer, element.Properties["y"].Integer);
                        if (element.Properties.ContainsKey("xplus"))
                            pos.X += element.Properties["xplus"].Integer;
                        if (element.Properties.ContainsKey("yplus"))
                            pos.Y += element.Properties["yplus"].Integer;
                        if (element.Properties.ContainsKey("adjustright"))
                            adj = element.Properties["adjustright"].Boolean;
                        component = new Label(element.Properties["text"].String,
                            pos, color, center, true, Game.Fonts[element.Properties["font"].Value], adj);
                        break;

                    case "label":
                        bool cent = false;
                        if (element.Properties.ContainsKey("center"))
                            cent = Convert.ToBoolean(element.Properties["center"].Value);
                        color = Color.White;
                        if (element.Properties.ContainsKey("color"))
                        {
                            string[] joints = element.Properties["color"].Value.Split(',');
                            color = new Color(int.Parse(joints[0]), int.Parse(joints[1]), int.Parse(joints[2]));
                        }
                        bool adjust = false;
                        pos = new Vector2(element.Properties["x"].Integer, element.Properties["y"].Integer);
                        if (element.Properties.ContainsKey("xplus"))
                            pos.X += element.Properties["xplus"].Integer;
                        if (element.Properties.ContainsKey("yplus"))
                            pos.Y += element.Properties["yplus"].Integer;
                        if (element.Properties.ContainsKey("adjustright"))
                            adjust = element.Properties["adjustright"].Boolean;
                        component = new Label(element.Properties["text"].String,
                            pos, color, cent, false, Game.Fonts[element.Properties["font"].Value], adjust);
                        break;

                    case "window":
                        Rectangle bounds = new Rectangle();
                        bounds.X = element.Properties["x"].Integer;
                        bounds.Y = element.Properties["y"].Integer;
                        if (element.Properties.ContainsKey("xplus"))
                            bounds.X += element.Properties["xplus"].Integer;
                        if (element.Properties.ContainsKey("yplus"))
                            bounds.Y += element.Properties["yplus"].Integer;
                        bounds.Width = element.Properties["width"].Integer;
                        bounds.Height = element.Properties["height"].Integer;
                        Windows.Add(new Window(element.Properties.ContainsKey("title") ? element.Properties["title"].Value : "", bounds, new Color(160, 109, 86)));
                        Windows[Windows.Count - 1].DialogFlag = element.Properties.ContainsKey("dialog") ? 
                            element.Properties["dialog"].AsBool : false;
                        break;

                    case "button":
                        pos = new Vector2(element.Properties["x"].Integer, element.Properties["y"].Integer);
                        if (element.Properties.ContainsKey("xplus"))
                            pos.X += element.Properties["xplus"].Integer;
                        if (element.Properties.ContainsKey("yplus"))
                            pos.Y += element.Properties["yplus"].Integer;
                        component = new Button(element.Properties["text"].String, pos,
                            Color.White, element.Properties.ContainsKey("width") ? element.Properties["width"].Integer : 0,
                            element.Properties.ContainsKey("adjustright") ? element.Properties["adjustright"].Boolean : false,
                            element.Properties.ContainsKey("nocenter") ? element.Properties["nocenter"].Boolean : false);
                        break;

                    case "checkbox":
                        pos = new Vector2(element.Properties["x"].Integer, element.Properties["y"].Integer);
                        if (element.Properties.ContainsKey("xplus"))
                            pos.X += element.Properties["xplus"].Integer;
                        if (element.Properties.ContainsKey("yplus"))
                            pos.Y += element.Properties["yplus"].Integer;
                        component = new Checkbox(element.Properties["text"].String, pos,
                            Color.White, element.Properties.ContainsKey("checked") ? element.Properties["checked"].Boolean : false);
                        break;

                    case "textbox":
                        int tx = element.Properties["x"].Integer;
                        int ty = element.Properties["y"].Integer;
                        if (element.Properties.ContainsKey("xplus"))
                            tx += element.Properties["xplus"].Integer;
                        if (element.Properties.ContainsKey("yplus"))
                            ty += element.Properties["yplus"].Integer;
                        component = new Textbox(tx, ty, element.Properties["width"].Integer,
                            element.Properties.ContainsKey("text") ? element.Properties["text"].String : "");
                            break;

                    case "dropdown":
                        List<string> items = new List<string>();
                        string dynamic = element.Properties.ContainsKey("dynamic") ?
                            element.Properties["dynamic"].Value : element.Properties.ContainsKey("items") ? 
                            element.Properties["items"].Value : null;
                        int dx = element.Properties["x"].Integer;
                        int dy = element.Properties["y"].Integer;
                        if (element.Properties.ContainsKey("xplus"))
                            dx += element.Properties["xplus"].Integer;
                        if (element.Properties.ContainsKey("yplus"))
                            dy += element.Properties["yplus"].Integer;
                        component = new DropDown(dx, dy,
                            element.Properties["width"].Integer, dynamic, element.Elements);   
                        break;

                    case "progress bar":
                        string[] rgb = element.Properties["color"].Value.Split(',');
                        Color barcolor = new Color(int.Parse(rgb[0]), int.Parse(rgb[1]), int.Parse(rgb[2]));
                        int max = element.Properties["max"].Integer,
                            min = element.Properties["min"].Integer,
                            value = element.Properties["value"].Integer,
                            x = element.Properties["x"].Integer,
                            y = element.Properties["y"].Integer,
                            width = element.Properties["width"].Integer,
                            height = element.Properties["height"].Integer;
                        if (element.Properties.ContainsKey("xplus"))
                            x += element.Properties["xplus"].Integer;
                        if (element.Properties.ContainsKey("yplus"))
                            y += element.Properties["yplus"].Integer;
                        component = new ProgressBar(x, y, max, min, value, width, height, barcolor);
                        break;

                    case "settings component":
                        component = new SettingsComponent();
                        break;
                }

                // Continue if component is null
                if (component == null) continue;

                // Add ID if required
                if (element.Properties.ContainsKey("id"))
                    component.ID = element.Properties["id"].Value;

                // Add tag if required
                if (element.Properties.ContainsKey("tag"))
                    component.Tag = element.Properties["tag"].Value;

                // Enabled
                if (element.Properties.ContainsKey("enabled"))
                    component.Enabled = element.Properties["enabled"].AsBool;

                // Dock?
                if(element.Properties.ContainsKey("dock"))
                    component.Dock = (Dock)Enum.Parse(typeof(Dock), element.Properties["dock"].Value);

                // Add key if present
                if (element.Properties.ContainsKey("key"))
                    component.Key = element.Properties["key"].Value;

                // Add events
                foreach (DataElement e in element.Elements)
                    if(e.Name.ToLower() == "event")
                        component.Events.Add(e);

                // Add component to list
                Components.Add(component);
            }

            // Set visible flag
            Visible = visible;

            // Set position
            Position = position;
        }

        /// <summary>
        /// Updates the interface.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Return if not updated since the visible flag was set
            if (!_updated)
            {
                _updated = true;
                return;
            }

            // Return if hidden
            if (!Visible) return;

            // Update all components
            foreach (Component component in Components)
            {
                // If the component is hidden: continue
                if (!component.Visible) continue;

                // Update the component
                if (component.GetType() == typeof(Textbox))
                {
                    Textbox textbox = (Textbox)component;
                    textbox.Update(gameTime);
                }
                else
                    component.Update(gameTime, Position);
            }
        }

        /// <summary>
        /// Draws the interface on the screen.
        /// </summary>
        public void Draw()
        {
            // Return if hidden
            if (!Visible) return;

            // Draw windows first
            foreach (Window window in Windows)
                window.Draw(Game.spriteBatch);

            // Draw all components
            foreach (Component component in Components)
            {
                // Continue if hidden
                if (!component.Visible) continue;

                // Draw the component
                if (component.GetType() == typeof(Textbox))
                {
                    Textbox textbox = (Textbox)component;
                    textbox.Draw(Game.spriteBatch);
                }
                else
                    component.Draw(Game.spriteBatch, Position);
            }
        }
    }
}
