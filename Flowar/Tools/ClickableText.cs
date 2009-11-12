using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Flowar
{
    public class ClickableText
    {
        #region Propriétés
        private SpriteFont _spriteFontMouseIn;
        private SpriteFont _spriteFontMouseOut;
        private String _text;
        private Vector2 _position;
        private bool isIn = false;
        private ButtonState leftMouseButtonState = ButtonState.Released;
        public bool IsOn = false;

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public int Width
        {
            get { return (int)_spriteFontMouseIn.MeasureString(_text).X; }
        }

        public int Height
        {
            get { return (int)_spriteFontMouseIn.MeasureString(_text).Y; }
        }
        #endregion

        #region Evènements
        public delegate void ClickTextHandler(ClickableText clickableText, MouseState mouseState, GameTime gameTime);
        public event ClickTextHandler ClickText;
        #endregion

        public ClickableText(GameBase gameParent, string spriteFontMouseIn, string spriteFontMouseOut, string text, Vector2 position)
        {
            this._spriteFontMouseIn = gameParent.ContentManager.Load<SpriteFont>(@"Content\Font\" + spriteFontMouseIn);
            this._spriteFontMouseOut = gameParent.ContentManager.Load<SpriteFont>(@"Content\Font\" +spriteFontMouseOut);
            this._text = text;
            this._position = position;
        }

        public void UpdateMouse()
        {
            MouseState mouseState = Mouse.GetState();
            UpdateMouse(mouseState, null);
        }

        public void UpdateMouse(MouseState mouseState, GameTime gameTime)
        {
            if (this.Position.X <= mouseState.X && this.Position.X + this.Width >= mouseState.X &&
                this.Position.Y <= mouseState.Y && this.Position.Y + this.Height >= mouseState.Y)
            {
                isIn = true;

                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    leftMouseButtonState = ButtonState.Pressed;
                }
                else if (mouseState.LeftButton == ButtonState.Released && leftMouseButtonState == ButtonState.Pressed && ClickText != null)
                {
                    leftMouseButtonState = ButtonState.Released;
                    ClickText(this, mouseState, gameTime);
                }
            }
            else
            {
                isIn = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            if (IsOn)
            {
                if (isIn)
                    spriteBatch.DrawString(_spriteFontMouseIn, _text, this.Position, color);
                else
                    spriteBatch.DrawString(_spriteFontMouseOut, _text, this.Position, color);
            }
            else
            {
                if (isIn)
                    spriteBatch.DrawString(_spriteFontMouseIn, _text, this.Position, color);
                else
                    spriteBatch.DrawString(_spriteFontMouseOut, _text, this.Position, color);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, Color.White);
        }
    }
}
