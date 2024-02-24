using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ingenia.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ingenia.Engine
{
    /// <summary>
    /// The character class. The base class for all dynamic objects in a scene.
    /// Always inherit this class when creating new dynamic object classes.
    /// </summary>
    public class Character : Base
    {
        /// <summary>
        /// The name of this character object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The current position of the character (exact coordinate).
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// The current position of the character on screen relative to camera.
        /// </summary>
        public Vector2 ScreenPosition { get { return Position - Screen.Camera; } }

        /// <summary>
        /// The direction this character is facing.
        /// </summary>
        public int Direction { get; set; }

        /// <summary>
        /// The current graphic for this character.
        /// </summary>
        protected Animation Graphic { get; set; }

        /// <summary>
        /// The current active action graphic.
        /// Will render instead of the current graphic when applicable.
        /// No action graphic should ever be set to loop.
        /// </summary>
        protected Animation ActionGraphic { get; set; }

        /// <summary>
        /// The set of available animations for this character.
        /// </summary>
        public Dictionary<string, Animation> AnimationSet { get; set; }

        /// <summary>
        /// The current movement speed for this character.
        /// </summary>
        public float Speed { get { return Properties["speed"].Float; } }

        /// <summary>
        /// The hitbox for this character. 
        /// Can be overridden.
        /// </summary>
        public virtual Rectangle Hitbox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y + 
                    (Graphic.Height - 32), Graphic.Width, 32);
            }
        }

        /// <summary>
        /// The character constructor.
        /// </summary>
        public Character()
            : base()
        {
            Required.Add("speed");
        }

        /// <summary>
        /// Processes character base data into logical properties.
        /// Can be overridden.
        /// </summary>
        public virtual void Process()
        {
        }

        /// <summary>
        /// Updates the character object.
        /// Can be overridden.
        /// </summary>
        /// <param name="gameTime">A snapshot of game timing values.</param>
        public virtual void Update(GameTime gameTime)
        {
            // Update action graphic if necessary
            if (ActionGraphic != null)
            {
                ActionGraphic.Update(gameTime);

                // Remove action graphic if finished playing
                if (ActionGraphic.GetFrame() < 0)
                    ActionGraphic = null;
            }

            // Update current graphic
            Graphic.Update(gameTime);
        }

        /// <summary>
        /// Draws the character top (front layer) to screen.
        /// Can be overridden.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void DrawTop(GameTime gameTime)
        {
            // Get graphic height
            int height = ActionGraphic != null ? ActionGraphic.Height : Graphic.Height;

            // Set source rectangle
            Rectangle source = new Rectangle(0, 0, 0, height - 32);

            // If action animation is active: draw that
            if (ActionGraphic != null)
                ActionGraphic.Draw(Game.spriteBatch, ScreenPosition, source);
            else
                Graphic.Draw(Game.spriteBatch, ScreenPosition, source);
        }

        /// <summary>
        /// Draws the character bottom (back layer) to screen.
        /// Can be overridden.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void DrawBottom(GameTime gameTime)
        {
            // Get graphic height
            int height = ActionGraphic != null ? ActionGraphic.Height : Graphic.Height;

            // Set source rectangle
            Rectangle source = new Rectangle(0, height - 32, 0, 32);

            // If action animation is active: draw that
            if (ActionGraphic != null)
                ActionGraphic.Draw(Game.spriteBatch, ScreenPosition + new Vector2(0, height - 32), source);
            else
                Graphic.Draw(Game.spriteBatch, ScreenPosition + new Vector2(0, height - 32), source);
        }
    }
}
