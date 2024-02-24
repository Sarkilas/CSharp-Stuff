using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ingenia.Data;

namespace Ingenia.Interface
{
    /// <summary>
    /// The abstract class that all components must inherit.
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        /// The tag for this container. Used for storing specific data.
        /// </summary>
        public object Tag { get; set; }
        public virtual string Text { get; set; }
        public virtual bool Boolean { get; set; }
        public virtual bool Clicked { get; set; }
        public virtual bool RightClicked { get; set; }
        public bool Visible = true;

        /// <summary>
        /// The identifier for this component. 
        /// Used for individual obtaining of components within data.
        /// This value is not required to be set. Can be null.
        /// </summary>
        public virtual string ID { get; set; }

        public bool Enabled = true;
        protected Rectangle _bounds = new Rectangle();
        public Dock Dock = Dock.Free;
        public Rectangle Bounds
        {
            get {
                return _bounds; 
            }
            set
            {
                _bounds = value;
            }
        }
        public SpriteFont Font = Game.Fonts["ChatFont"];
        public string Key = null;

        public static bool MouseOver = false;
        public static bool Focused = false;

        public List<DataElement> Events = new List<DataElement>();

        public abstract void Update(GameTime gameTime, Vector2 relative);
        public abstract void Draw(SpriteBatch spriteBatch, Vector2 relative);
    }

    public enum Dock
    {
        None, Left, Right, Bottom, Top, Free
    }
}
