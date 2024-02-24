using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Ingenia.Engine;
using Ingenia.Data;
using EventInput;

namespace Ingenia.Interface
{
    public delegate void TextBoxEvent(Textbox sender);

    public class Textbox : Component, IKeyboardSubscriber
    {
        Texture2D _textBoxTexture;

        SpriteFont _font;

        /// <summary>
        /// Visible state. This will be true if any text box is visible on screen.
        /// </summary>
        public static bool Visible = false;

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; private set; }

        public bool Highlighted { get; set; }

        public bool PasswordBox { get; set; }

        public event TextBoxEvent Clicked;
        int caretTimer = 0; bool caretVisible = false;

        string _text = "";
        public override string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                if (_text == null)
                    _text = "";

                if (_text != "")
                {
                    //if you attempt to display a character that is not in your font
                    //you will get an exception, so we filter the characters
                    String filtered = "";
                    foreach (char c in value)
                    {
                        if (_font.Characters.Contains(c))
                            filtered += c;
                    }

                    _text = filtered;

                    if (_font.MeasureString(_text).X > Width)
                    {
                        //recursion to ensure that text cannot be larger than the box
                        Text = _text.Substring(0, _text.Length - 1);
                    }
                }
            }
        }

        public Textbox(int x, int y, int width, string text = "")
        {
            X = x; Y = y; Width = width; 
            _textBoxTexture = GameData.GetTexture("Textbox_Background.png");
            _font = Game.Fonts["ChatFont"];
            _text = text;

            _previousMouse = Mouse.GetState();

            Game.Dispatcher.Subscriber = this;

            OnEnterPressed += new TextBoxEvent(UpdateEvents);
        }

        MouseState _previousMouse;
        public void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            Point mousePoint = new Point(mouse.X, mouse.Y);

            Rectangle position = new Rectangle(X + (int)Screen.Offset.X, Y + (int)Screen.Offset.Y, Width, Height);
            if (position.Contains(mousePoint))
            {
                Highlighted = true;
                if (_previousMouse.LeftButton == ButtonState.Released && mouse.LeftButton == ButtonState.Pressed)
                {
                    Game.Dispatcher.Subscriber = this;
                    if (Clicked != null)
                        Clicked(this);
                }
            }
            else
            {
                Highlighted = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Visible = true;
            if (caretTimer > 0)
                caretTimer -= 1;
            else{
                caretTimer = 30;
                caretVisible = !caretVisible;
            }

            String toDraw = Text;

            if (PasswordBox)
            {
                toDraw = "";
                for (int i = 0; i < Text.Length; i++)
                    toDraw += (char)0x2022;
            }

            if (caretVisible && Selected) toDraw += "|";

            Vector2 size = _font.MeasureString(toDraw);
            Height = (int)_font.MeasureString("W").Y + 8;

            Rectangle bounds = new Rectangle(X, Y, Width, Height);
            if (Dock == Dock.Right)
                bounds.X = Screen.Width - bounds.X;
            else if (Dock != Dock.Left && Dock != Dock.Bottom)
                bounds.X += (int)Screen.Offset.X;
            if (Dock == Dock.Bottom)
                bounds.Y = Screen.Height - bounds.Y;
            else if (Dock != Dock.Top)
                bounds.Y += (int)Screen.Offset.Y;

            spriteBatch.Draw(_textBoxTexture, bounds, new Rectangle(0, Highlighted ? (_textBoxTexture.Height / 2) : 0, 
                    _textBoxTexture.Width, _textBoxTexture.Height / 2), Color.White * .6f);

            //shadow first, then the actual text
            Tools.DrawBorderText(toDraw, new Vector2(bounds.X + 4, bounds.Y + 4), Color.White, Color.Black, _font, spriteBatch);
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 relative)
        {
            throw new NotImplementedException();
        }
        public override void Update(GameTime gameTime, Vector2 relative)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the events for this component.
        /// </summary>
        private void UpdateEvents(Textbox sender)
        {
            // Ignore if hidden
            if (!Textbox.Visible) return;

            // Go through each event
            foreach (DataElement element in sender.Events)
            {
                // Ignore if property is missing
                if (!element.Properties.ContainsKey("name"))
                    continue;

                // Click event
                if (element.Properties["name"].Value.ToLower() == "enter")
                {
                    // Show / hide interfaces
                    if (element.Properties.ContainsKey("showinterface"))
                        Game.scene.ToggleInterface(element.Properties["showinterface"].Value, true);
                    if (element.Properties.ContainsKey("hideinterface"))
                        Game.scene.ToggleInterface(element.Properties["hideinterface"].Value, false);
                }
            }
        }


        public void RecieveTextInput(char inputChar)
        {
            Text = Text + inputChar;
        }
        public void RecieveTextInput(string text)
        {
            Text = Text + text;
        }
        public void RecieveCommandInput(char command)
        {
            switch (command)
            {
                case '\b': //backspace
                    if (Text.Length > 0)
                        Text = Text.Substring(0, Text.Length - 1);
                    break;
                case '\r': //return
                    if (OnEnterPressed != null)
                        OnEnterPressed(this);
                    break;
                case '\t': //tab
                    if (OnTabPressed != null)
                        OnTabPressed(this);
                    break;
                default:
                    break;
            }
        }
        public void RecieveSpecialInput(System.Windows.Forms.Keys key)
        {

        }

        public event TextBoxEvent OnEnterPressed;
        public event TextBoxEvent OnTabPressed;

        public bool Selected
        {
            get;
            set;
        }
    }
}
