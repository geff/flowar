using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NewFlowar
{
	public class ClickableText : ClickableZone
	{
		#region Propriétés
		private SpriteFont _spriteFontMouseIn;
		private SpriteFont _spriteFontMouseOut;
		private String _text;

		public override int Width
		{
			get { return (int)_spriteFontMouseIn.MeasureString(_text).X; }
		}

		public override int Height
		{
			get { return (int)_spriteFontMouseIn.MeasureString(_text).Y; }
		}
		#endregion

		public ClickableText(GameBase gameParent, string spriteFontMouseIn, string spriteFontMouseOut, string text, Vector2 position)
			: base(position, 0, 0)
		{
			this._spriteFontMouseIn = gameParent.ContentManager.Load<SpriteFont>(@"Content\Font\" + spriteFontMouseIn);
			this._spriteFontMouseOut = gameParent.ContentManager.Load<SpriteFont>(@"Content\Font\" + spriteFontMouseOut);
			this._text = text;
			this._position = position;
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
