using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ingenia.Data;
using Ingenia.Engine;

namespace Ingenia
{
    /// <summary>
    /// The animation data class.
    /// </summary>
    public class AnimationData
    {
        /// <summary>
        /// The position of this animation.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// The current frame.
        /// </summary>
        public int Frame { get; set; }

        /// <summary>
        /// The amount of frames in this animation.
        /// </summary>
        public int Frames { get; set; }

        /// <summary>
        /// The frame data of this animation.
        /// </summary>
        public Dictionary<int, List<FrameData>> Data { get; set; }

        /// <summary>
        /// The animation effects for this object.
        /// </summary>
        public List<AnimationEffect> Effects { get; set; }

        /// <summary>
        /// The time (in milliseconds) between each frame.
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// The current time.
        /// </summary>
        public int FrameTime { get; set; }

        /// <summary>
        /// The flashing time in milliseconds.
        /// </summary>
        public int FlashTime { get; set; }

        /// <summary>
        /// The flash color.
        /// </summary>
        public Color FlashColor { get; set; }

        /// <summary>
        /// The animation target of this object.
        /// </summary>
        public AnimationTarget Target { get; set; }

        /// <summary>
        /// True if the animation should still update and draw.
        /// </summary>
        public bool Active
        {
            get
            {
                return Frame <= Frames;
            }
        }

        /// <summary>
        /// Constructs a new animation data object.
        /// </summary>
        /// <param name="position">The center position of this animation.</param>
        /// <param name="data">The data to construct the object from.</param>
        public AnimationData(Vector2 position, AnimationTarget target, Base data)
        {
            // Initialize the data lists
            Data = new Dictionary<int, List<FrameData>>();
            Effects = new List<AnimationEffect>();

            // Set position
            Position = position;

            // Set target
            Target = target;

            // Set frame count and frame time
            Frames = data.Properties["frames"].AsInteger;
            Time = data.Properties["time"].AsInteger;

            // Process all frame and effect data
            foreach (DataElement element in data.Elements)
            {
                // Frame data
                if (element.Name.ToLower() == "frame")
                {
                    // Add new data list
                    Data.Add(element.Properties["number"].AsInteger, new List<FrameData>());

                    // Process all data within this frame element
                    foreach (DataElement frame in element.Elements)
                    {
                        // Ignore if not a data element
                        if (frame.Name.ToLower() != "data") continue;

                        // Create the new frame data object
                        FrameData f = new FrameData();

                        // Set frame coordinate
                        string[] joints = frame.Properties["frame"].Value.Split(',');
                        f.Frame = new Vector(int.Parse(joints[0]), int.Parse(joints[1]));

                        // Set frame color
                        joints = frame.Properties["color"].Value.Split(',');
                        f.FrameColor = new Color(int.Parse(joints[0]), int.Parse(joints[1]), int.Parse(joints[2]));

                        // Set frame graphic
                        f.Graphic = frame.Properties["graphic"].Value;

                        // Set frame size
                        joints = frame.Properties["size"].Value.Split(',');
                        f.Size = new Vector(int.Parse(joints[0]), int.Parse(joints[1]));

                        // Set opacity, scale and priority
                        f.Opacity = frame.Properties["opacity"].AsFloat;
                        f.Scale = frame.Properties["scale"].AsFloat;
                        f.Priority = frame.Properties["priority"].AsInteger;

                        // Add frame data to list
                        Data[element.Properties["number"].AsInteger].Add(f);
                    }
                }
                else if (element.Name.ToLower() == "effect")
                {
                    // Create new effect object
                    AnimationEffect effect = new AnimationEffect();

                    // Set properties
                    effect.Number = element.Properties["number"].AsInteger;
                    if (element.Properties.ContainsKey("sound"))
                        effect.Sound = element.Properties["sound"].Value;

                    // Set flash effect
                    if (element.Properties["flash"].AsBool)
                    {
                        effect.Flash = true;
                        effect.FlashDuration = element.Properties["duration"].AsInteger;
                        string[] joints = element.Properties["color"].Value.Split(',');
                        effect.FlashColor = new Color(int.Parse(joints[0]), int.Parse(joints[1]), int.Parse(joints[2]));
                    }

                    // Add effect to list
                    Effects.Add(effect);
                }
            }
        }
        public AnimationData() { }

        /// <summary>
        /// Updates the animation data.
        /// </summary>
        /// <param name="gameTime">A snapshot of game timing values.</param>
        public void Update(GameTime gameTime)
        {
            // Return if not active
            if (!Active) return;

            // Update first effect frame
            bool first = false;
            if (FrameTime == Time && Frame == 1)
            {
                // Update effects (only happens once per frame)
                foreach (AnimationEffect effect in Effects)
                {
                    if (effect.Number == Frame)
                    {
                        // Play the sound if one is given
                        if (effect.Sound != null)
                            Audio.PlaySound(effect.Sound);

                        // Flash if given
                        // (flashes are overridden if a new one occurs before the last finished)
                        if (effect.Flash)
                        {
                            FlashColor = effect.FlashColor;
                            FlashTime = effect.FlashDuration;
                        }
                    }
                }
                first = true;
            }

            // Reduce the time
            FrameTime -= gameTime.ElapsedGameTime.Milliseconds;

            // If the frame time is at or below 0: advance frame and apply effects
            if (FrameTime <= 0)
            {
                Frame++;
                FrameTime = Time;

                // Update effects (only happens once per frame)
                foreach (AnimationEffect effect in Effects)
                {
                    if (effect.Number == Frame)
                    {
                        // Play the sound if one is given
                        if (effect.Sound != null)
                            Audio.PlaySound(effect.Sound);

                        // Flash if given
                        // (flashes are overridden if a new one occurs before the last finished)
                        if (effect.Flash)
                        {
                            FlashColor = effect.FlashColor;
                            FlashTime = effect.FlashDuration;
                        }
                    }
                }
            }
            else if(!first)
                FlashTime = 0;
        }

        /// <summary>
        /// Draws the animation on screen.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position = new Vector2())
        {
            // Return if inactive
            if (!Active) return;

            // Fix position
            if (position != Vector2.Zero)
                Position = position;

            // Drawing priorities are given first
            for (int i = 0; i <= 5; i++)
            {
                // Draw each relative frame
                foreach (FrameData frame in Data[Frame])
                {
                    // Only draw if frame and priority 
                    // match the required values
                    if (frame.Priority == i)
                        frame.Draw(Position, spriteBatch);
                }
            }
        }

        /// <summary>
        /// Draws the animation on screen.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawFrame(int frame, SpriteBatch spriteBatch, Vector2 position = new Vector2())
        {
            // Fix position
            if (position != Vector2.Zero)
                Position = position;

            // Drawing priorities are given first
            for (int i = 0; i <= 5; i++)
            {
                // Draw each relative frame
                foreach (FrameData data in Data[frame])
                {
                    // Only draw if frame and priority 
                    // match the required values
                    if (data.Priority == i)
                        data.Draw(Position, spriteBatch);
                }
            }
        }
    }

    /// <summary>
    /// Contains frame data for animation data objects.
    /// </summary>
    public class FrameData
    {
        /// <summary>
        /// The frame number (when it should appear) of this frame.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// The frame drawing priority. Using 0-5
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// The position of this frame relative to the animation center position.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// The graphic used for this frame data object.
        /// </summary>
        public string Graphic { get; set; }

        /// <summary>
        /// The vector size (X = frames, Y = rows) of the animation graphic.
        /// </summary>
        public Vector Size { get; set; }

        /// <summary>
        /// The selected frame (x, y) from the graphic.
        /// </summary>
        public Vector Frame { get; set; }

        /// <summary>
        /// The color of this frame (for the graphic that is to be drawn).
        /// </summary>
        public Color FrameColor { get; set; }

        /// <summary>
        /// The scale of this frame.
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// The opacity for this frame.
        /// </summary>
        public float Opacity { get; set; }

        /// <summary>
        /// Draws the frame.
        /// </summary>
        /// <param name="relative">The animation center point.</param>
        /// <param name="spriteBatch">The spritebatch to draw to.</param>
        public void Draw(Vector2 relative, SpriteBatch spriteBatch)
        {
            // Return if texture is non-existant
            if (!GameData.Data.ContainsKey(Graphic)) return;

            // Get the texture
            Texture2D texture = GameData.GetTexture(Graphic);

            // Get the frame size
            Vector size = new Vector((int)((texture.Width / Size.X) * Scale),
                (int)((texture.Height / Size.Y) * Scale)),
                srcsize = new Vector((int)(texture.Width / Size.X),
                (int)(texture.Height / Size.Y));

            // Fix the position
            relative -= new Vector2(size.X / 2, size.Y / 2) - Position;

            // Create the destination rectangle
            Rectangle rect = new Rectangle((int)relative.X, 
                (int)relative.Y, size.X, size.Y);

            // Draw the graphic
            spriteBatch.Draw(texture, rect, new Rectangle(srcsize.X * Frame.X,
                srcsize.Y * Frame.Y, srcsize.X, srcsize.Y), FrameColor * Opacity);
        }
    }

    /// <summary>
    /// Contains effect data for sounds or animation flashes (or both).
    /// </summary>
    public class AnimationEffect
    {
        /// <summary>
        /// The frame number (when it should appear) of this frame.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// The sound to play for this effect. Null if no sound is to be played.
        /// </summary>
        public string Sound { get; set; }

        /// <summary>
        /// True if the target should flash at this effect.
        /// </summary>
        public bool Flash { get; set; }

        /// <summary>
        /// The flash color for this effect.
        /// </summary>
        public Color FlashColor { get; set; }

        /// <summary>
        /// The flash duration of this animation effect's flash.
        /// </summary>
        public int FlashDuration { get; set; }
    }

    /// <summary>
    /// The animation target (used for flashing).
    /// </summary>
    public enum AnimationTarget
    {
        Target,
        Screen
    }
}