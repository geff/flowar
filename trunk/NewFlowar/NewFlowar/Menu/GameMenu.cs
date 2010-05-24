using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace NewFlowar
{
	public class GameMenu : GameBase
	{
		delegate void MenuItemDelegate();
		MenuItemDelegate currentMenuItem = null;

		public GameMenu(GameMain game, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, ContentManager contentManager)
			: base(game, spriteBatch, graphicsDevice, contentManager)
		{
			this.Game = game;

			Init();
		}

		#region Initialization
		public override void Init()
		{
			this.MenuAnimationCloseEnded += new MenuAnimationCloseEndedHandler(GameMenu_MenuAnimationCloseEnded);

			//btnTitle = new ClickableImage(
			//    ContentManager.Load<Texture2D>(@"Content\Pic\FlowarTitle_On"),
			//    ContentManager.Load<Texture2D>(@"Content\Pic\FlowarTitle_Off"),
			//    new Microsoft.Xna.Framework.Vector2(50, GraphicsDevice.Viewport.Height / 4 + 35));


			//this.AddClickableImage(btnTitle);
			//this.AddClickableImage(btnBloc);
			//this.AddClickableImage(btnNature);
			////this.AddClickableImage(btnSphere);
			//this.AddClickableImage(btnTriangle);


			ClickableText txtFlowar = new ClickableText(this, "Font0", "Font1", "floWAR", new Microsoft.Xna.Framework.Vector2(300, GraphicsDevice.Viewport.Height / 4 + 180));
			txtFlowar.Clicked += new ClickableZone.ClickZoneHandler(txtFlowar_ClickZone);

			this.AddClickableZone(txtFlowar);

			base.Init(true);
		}


		void GameMenu_MenuAnimationCloseEnded(GameTime gameTime)
		{
			currentMenuItem();
		}
		#endregion

		public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
		{
			base.Update(gameTime);
		}

		private void ShowFleurGame()
		{
			Game.GameCurrent = new GameFlowar(this.Game, this.SpriteBatch, this.GraphicsDevice, this.ContentManager);
		}

		public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
		{
			GraphicsDevice.Clear(new Color(25, 25, 30));

			SpriteBatch.Begin();

			base.Draw(gameTime);

			SpriteBatch.End();
		}

		#region Evènements
		void txtFlowar_ClickZone(ClickableZone image, Microsoft.Xna.Framework.Input.MouseState mouseState, GameTime gameTime)
		{
			this.StartMenuOff(gameTime);
			currentMenuItem = ShowFleurGame;
		}
		#endregion
	}
}
