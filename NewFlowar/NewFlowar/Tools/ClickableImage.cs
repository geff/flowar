using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NewFlowar
{
	public class ClickableImage : ClickableZone
	{
		#region Propriétés
		private Texture2D _textureMouseIn;
		private Texture2D _textureMouseOut;

		public override int Width
		{
			get { return _textureMouseIn.Width; }
		}

		public override int Height
		{
			get { return _textureMouseIn.Height; }
		}
		#endregion

		public ClickableImage(Texture2D textureMouseIn, Texture2D textureMouseOut, Vector2 position)
			: base(position, textureMouseIn.Width, textureMouseIn.Height)
		{
			this._textureMouseIn = textureMouseIn;
			this._textureMouseOut = textureMouseOut;
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
