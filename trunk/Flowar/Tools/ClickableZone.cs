using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Flowar
{
	public class ClickableZone
	{
		#region Propriétés
		protected Vector2 _position;
		protected bool isIn = false;
		protected ButtonState leftMouseButtonState = ButtonState.Released;
		public bool IsOn = false;

		public Vector2 Position
		{
			get { return _position; }
			set { _position = value; }
		}

		public virtual int Width { get; set; }

		public virtual int Height { get; set; }

		public Object Tag { get; set; }
		#endregion

		#region Evènements
		public delegate void ClickZoneHandler(ClickableZone zone, MouseState mouseState, GameTime gameTime);
		public event ClickZoneHandler Clicked;

		public delegate void ClickZoneMouseEnterHandler(ClickableZone zone, MouseState mouseState, GameTime gameTime);
		public event ClickZoneMouseEnterHandler MouseEnter;

		public delegate void ClickZoneMouseLeaveHandler(ClickableZone zone, MouseState mouseState, GameTime gameTime);
		public event ClickZoneMouseLeaveHandler MouseLeave;
		#endregion

		public ClickableZone(Vector2 position, int width, int height)
		{
			this.Position = position;
			this.Width = width;
			this.Height = height;
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
				else if (mouseState.LeftButton == ButtonState.Released && leftMouseButtonState == ButtonState.Pressed && Clicked != null)
				{
					leftMouseButtonState = ButtonState.Released;
					Clicked(this, mouseState, gameTime);
				}

				if (MouseEnter != null)
					MouseEnter(this, mouseState, gameTime);
			}
			else
			{
				if (isIn && MouseLeave != null)
					MouseLeave(this, mouseState, gameTime);

				isIn = false;
			}
		}
	}
}
