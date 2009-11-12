using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Flowar
{
    public class ClickableImage
    {
        #region Propriétés
        private Texture2D _textureMouseIn;
        private Texture2D _textureMouseOut;
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
            get { return _textureMouseIn.Width; }
        }

        public int Height
        {
            get { return _textureMouseIn.Height; }
        }
        #endregion

        #region Evènements
        public delegate void ClickImageHandler(ClickableImage image, MouseState mouseState, GameTime gameTime);
        public event ClickImageHandler ClickImage;
        #endregion

        public ClickableImage(Texture2D textureMouseIn, Texture2D textureMouseOut, Vector2 position)
        {
            this._textureMouseIn = textureMouseIn;
            this._textureMouseOut = textureMouseOut;
            this.Position = position;
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
                else if (mouseState.LeftButton == ButtonState.Released && leftMouseButtonState == ButtonState.Pressed && ClickImage != null)
                {
                    leftMouseButtonState = ButtonState.Released;
                    ClickImage(this, mouseState, gameTime);
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
                    spriteBatch.Draw(this._textureMouseOut, this.Position, null, color);
                else
                    spriteBatch.Draw(this._textureMouseIn, this.Position, null, color);
            }
            else
            {
                if (isIn)
                    spriteBatch.Draw(this._textureMouseIn, this.Position, null, color);
                else
                    spriteBatch.Draw(this._textureMouseOut, this.Position, null, color);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, Color.White);
        }
    }
}
