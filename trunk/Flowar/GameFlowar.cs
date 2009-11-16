using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Flowar.Tools;

namespace Flowar
{
	public class GameFlowar : GameBase
	{
		private Texture2D tex2DGround;

		private int currentPlayer = 0;
		private PlayerCard currentPlayerCard = null;
		private float currentCardRotation;
		private int[,] tabCurrentCard;
		private int cycleRotationCard;

		private ContextType contextType = ContextType.None;

		public Map Map { get; set; }
		public List<ModelCard> ListModelCard { get; set; }
		public Dictionary<int, List<PlayerCard>> ListAllPlayerCard { get; set; }
		public Dictionary<int, Color> ListAllPlayerColor { get; set; }

		private Vector2 posMap = new Vector2(50, 70);

		int caseSize = 73;
		int marge = 35;
		int smallMarge = 10;
		int widthPlayerCard = 2;
		int heightPlayerCard = 4;
		float scale = 0.5f;
		Random rnd = new Random();

		//Lerp lerpScale;
		Lerp lerpRotation;

		public GameFlowar(GameMain game, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, ContentManager contentManager)
			: base(game, spriteBatch, graphicsDevice, contentManager)
		{
			this.Game = game;

			Init();
		}

		public override void Init()
		{
			this.MouseRightButtonClicked += new MouseRightButtonClickedHandler(GameFlowar_MouseRightButtonClicked);
			this.MouseLeftButtonClicked += new MouseLeftButtonClickedHandler(GameFlowar_MouseLeftButtonClicked);
			this.MouseWheelChanged += new MouseWheelChangeddHandler(GameFlowar_MouseWheelChanged);
			this.KeyPressed += new KeyPressedHandler(GameFlowar_KeyPressed);
			this.MenuAnimationOpenEnded += new MenuAnimationOpenEndedHandler(GameFlowar_MenuAnimationOpenEnded);

			//---
			//txtMenu = new ClickableImage(ContentManager.Load<Texture2D>(@"Content\Pic\Menu_On"), ContentManager.Load<Texture2D>(@"Content\Pic\Menu_Off"), new Vector2(50, 18));
			//txtMenu.ClickImage += new ClickableImage.ClickImageHandler(txtMenu_ClickImage);
			//this.AddClickableImage(txtMenu);
			//---

			//--- Chargement des textures
			tex2DGround = ContentManager.Load<Texture2D>(@"Content\Pic\Fond");
			//fleur0Tx = ContentManager.Load<Texture2D>(@"Content\Pic\Fleur1");
			//---

			//---
			rnd = new Random();
			this.ShowMiniMenu = true;
			ListAllPlayerCard = new Dictionary<int, List<PlayerCard>>();
			ListAllPlayerColor = new Dictionary<int, Color>();

			ListAllPlayerColor.Add(1, Color.Orange);
			ListAllPlayerColor.Add(2, Color.Violet);
			ListAllPlayerColor.Add(3, Color.SteelBlue);
			//---

			//---
			StartGame();
			//---


			base.Init();
		}

		void GameFlowar_MouseLeftButtonClicked(MouseState mouseState, GameTime gameTime)
		{
		}

		void GameFlowar_MouseRightButtonClicked(MouseState mouseState, GameTime gameTime)
		{
		}

		void GameFlowar_MouseWheelChanged(MouseState mouseState, GameTime gameTime)
		{
			//---> Rotation de la carte lorsqu'ele est sélectionnée
			if (contextType == ContextType.CardSelected)
			{
				float nextCardRotation = (float)mouseState.ScrollWheelValue / 120f * MathHelper.PiOver2;
				lerpRotation = new Lerp(currentCardRotation, nextCardRotation, false, false, 100, -1);

				int sens = nextCardRotation < currentCardRotation ? -1 : 1;
				cycleRotationCard = (cycleRotationCard + sens) % 4;
				if (cycleRotationCard < 0)
					cycleRotationCard += 4;

				contextType = ContextType.CardRotated;

				CalcTabSelectedCard();
			}

			//---> Rotation de la carte lorsqu'elle est au dessus de la map
			if (contextType == ContextType.CardOverMap)
			{
				float nextCardRotation = (float)mouseState.ScrollWheelValue / 120f * MathHelper.PiOver2;

				int sens = nextCardRotation < currentCardRotation ? -1 : 1;

				cycleRotationCard = (cycleRotationCard + sens) % 4;
				if (cycleRotationCard < 0)
					cycleRotationCard += 4;

				currentCardRotation = nextCardRotation;
				CalcTabSelectedCard();
			}

			this.Game.Window.Title = cycleRotationCard.ToString();
		}

		void GameFlowar_MenuAnimationOpenEnded(GameTime gameTime)
		{
		}

		void GameFlowar_KeyPressed(Keys key, GameTime gameTime)
		{
		}


		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update(GameTime gameTime)
		{
			//scale = lerpScale.Eval(gameTime);
			//rotation =lerpRotation.Eval(gameTime);

			if (contextType == ContextType.CardRotated)
			{
				currentCardRotation = lerpRotation.Eval(gameTime);

				if (lerpRotation.IsFinished(gameTime))
					contextType = ContextType.CardSelected;
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(new Color(20, 20, 20));

			//---
			SpriteBatch.Begin();

			DrawMap();

			for (int i = 1; i <= ListAllPlayerColor.Keys.Count; i++)
			{
				DrawPlayerCards(i);
			}

			if (contextType == ContextType.CardOverMap)
				DrawSelectedCardOverMap();

			SpriteBatch.End();
			//---

			//---
			if (
					currentPlayerCard != null &&
					(contextType == ContextType.CardSelected || contextType == ContextType.CardRotated)
				)
			{
				DrawSelectedCard();
			}
			//---

			//---
			SpriteBatch.Begin();

			base.Draw(gameTime);

			SpriteBatch.End();
			//---
		}

		private void StartGame()
		{
			CreateMap();

			CreateCardModels();

			for (int i = 1; i <= ListAllPlayerColor.Keys.Count; i++)
			{
				DistributeCards(i);
			}

			CreateCardClickableZone();

			NextPlayerToPlay();
		}

		private void CreateCardClickableZone()
		{
			Vector2 posDeck = new Vector2(posMap.X + Map.Width * caseSize + marge, posMap.Y);


			for (int numberPlayer = 1; numberPlayer <= ListAllPlayerColor.Keys.Count; numberPlayer++)
			{
				for (int numCard = 0; numCard < ListAllPlayerCard[numberPlayer].Count; numCard++)
				{
					Vector2 posCard = posDeck + new Vector2(
						numCard * (smallMarge + widthPlayerCard * caseSize * scale),
						(caseSize * scale * heightPlayerCard + marge) * (numberPlayer - 1));

					ClickableZone clickableCardZone = new ClickableZone(posCard, (int)(caseSize * scale * widthPlayerCard), (int)(caseSize * scale * heightPlayerCard));
					clickableCardZone.Tag = ListAllPlayerCard[numberPlayer][numCard];

					clickableCardZone.Clicked += new ClickableZone.ClickZoneHandler(clickableCardZone_ClickZone);

					this.AddClickableZone(clickableCardZone);
				}
			}
		}

		private void CreateMap()
		{
			int width = 6;
			int height = 6;

			this.Map = new Map(width, height);

			ClickableZone mapZone = new ClickableZone(posMap, Map.Width * caseSize, Map.Height * caseSize);
			mapZone.MouseEnter += new ClickableZone.ClickZoneMouseEnterHandler(mapZone_MouseEnter);
			mapZone.MouseLeave += new ClickableZone.ClickZoneMouseLeaveHandler(mapZone_MouseLeave);

			this.AddClickableZone(mapZone);
		}

		private void CreateCardModels()
		{
			this.ListModelCard = new List<ModelCard>();

			//--- L
			ModelCard modelCard = new ModelCard();

			modelCard.Cases[0, 0] = new Case() { FlowerType = FlowerType.None };
			modelCard.Cases[0, 1] = new Case() { FlowerType = FlowerType.None };
			modelCard.Cases[0, 2] = new Case() { FlowerType = FlowerType.None };
			modelCard.Cases[1, 2] = new Case() { FlowerType = FlowerType.None };

			this.ListModelCard.Add(modelCard);
			//---

			//--- []
			modelCard = new ModelCard();

			modelCard.Cases[0, 0] = new Case() { FlowerType = FlowerType.None };
			modelCard.Cases[0, 1] = new Case() { FlowerType = FlowerType.None };
			modelCard.Cases[1, 0] = new Case() { FlowerType = FlowerType.None };
			modelCard.Cases[1, 1] = new Case() { FlowerType = FlowerType.None };

			this.ListModelCard.Add(modelCard);
			//---

			//--- I
			modelCard = new ModelCard();

			modelCard.Cases[0, 0] = new Case() { FlowerType = FlowerType.None };
			modelCard.Cases[0, 1] = new Case() { FlowerType = FlowerType.None };
			modelCard.Cases[0, 2] = new Case() { FlowerType = FlowerType.None };
			modelCard.Cases[0, 3] = new Case() { FlowerType = FlowerType.None };

			this.ListModelCard.Add(modelCard);
			//---

			//--- T
			modelCard = new ModelCard();

			modelCard.Cases[0, 0] = new Case() { FlowerType = FlowerType.None };
			modelCard.Cases[0, 1] = new Case() { FlowerType = FlowerType.None };
			modelCard.Cases[0, 2] = new Case() { FlowerType = FlowerType.None };
			modelCard.Cases[1, 1] = new Case() { FlowerType = FlowerType.None };

			this.ListModelCard.Add(modelCard);
			//---

			//--- Z
			modelCard = new ModelCard();

			modelCard.Cases[0, 0] = new Case() { FlowerType = FlowerType.None };
			modelCard.Cases[0, 1] = new Case() { FlowerType = FlowerType.None };
			modelCard.Cases[1, 1] = new Case() { FlowerType = FlowerType.None };
			modelCard.Cases[1, 2] = new Case() { FlowerType = FlowerType.None };

			this.ListModelCard.Add(modelCard);
			//---


			foreach (ModelCard card in ListModelCard)
			{
				int[,] drawingCaseValue = card.DrawingCaseValue;
				CalcDrawingCaseValue(card.Cases, ref drawingCaseValue);
				card.DrawingCaseValue = drawingCaseValue;
			}

		}

		private void DistributeCards(int numberPlayer)
		{
			int numberCards = 5;

			//--- Création de la liste des cartes des joueurs
			List<PlayerCard> ListPlayerCard = new List<PlayerCard>();
			ListAllPlayerCard.Add(numberPlayer, ListPlayerCard);
			//---

			for (int i = 0; i < numberCards; i++)
			{
				PlayerCard playerCard = new PlayerCard();
				ModelCard modeCard = ListModelCard[rnd.Next(0, ListModelCard.Count)];

				playerCard.Cases = modeCard.Cases;
				playerCard.DrawingCaseValue = modeCard.DrawingCaseValue;
				playerCard.Player = numberPlayer;

				ListAllPlayerCard[numberPlayer].Add(playerCard);
			}
		}

		private void CalcDrawingCaseValue(Case[,] cases, ref int[,] casesValue)
		{
			int width = cases.GetUpperBound(0) + 1;
			int height = cases.GetUpperBound(1) + 1;

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					int caseValue = -1;

					if (cases[x, y] != null)
					{
						caseValue = 0;

						if (AreCasesEqual(cases[x, y], cases, x, y - 1))
							caseValue += (int)Math.Pow(2, 2);
						if (AreCasesEqual(cases[x, y], cases, x, y + 1))
							caseValue += (int)Math.Pow(2, 8);
						if (AreCasesEqual(cases[x, y], cases, x - 1, y))
							caseValue += (int)Math.Pow(2, 4);
						if (AreCasesEqual(cases[x, y], cases, x + 1, y))
							caseValue += (int)Math.Pow(2, 6);
					}

					if (caseValue == 0)
						caseValue = 0;

					casesValue[x, y] = caseValue;
				}
			}
		}

		private void CalcTabSelectedCard()
		{
			Case[,] caseSelectedCard = new Case[0,0];

			//--- Détermine la taille réelle de la carte sélectionnée
			int width = currentPlayerCard.Cases.GetUpperBound(0) + 1;
			int height = currentPlayerCard.Cases.GetUpperBound(1) + 1;

			int realWidth = 0;
			int realHeight = 0;

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					if (currentPlayerCard.Cases[x, y] != null)
					{
						if (realWidth < x)
							realWidth = x;
						if (realHeight < y)
							realHeight = y;
					}
				}
			}
			//---

			//--- Incrémente de 1 les dimensions
			realWidth++;
			realHeight++;
			//---

			//--- Angle 0
			//  1 _ 2
			//  3|_|4
			//---
			if (cycleRotationCard == 0)
			{
				//---> Redimensionne le tableau représentant la carte
				//	   sur la map selon sa taille réelle
				tabCurrentCard = new int[realWidth, realHeight];
				caseSelectedCard = new Case[realWidth, realHeight];

				for (int x = 0; x < realWidth; x++)
				{
					for (int y = 0; y < realHeight; y++)
					{
						//tabCurrentCard[x, y] = currentPlayerCard.DrawingCaseValue[x, y];
						caseSelectedCard[x,y] = currentPlayerCard.Cases[x,y];
					}
				}
			}

			//--- Angle Pi/2
			//  3 _ 1
			//  4|_|2
			//---
			if (cycleRotationCard == 1)
			{
				//---> Redimensionne le tableau représentant la carte
				//	   sur la map selon sa taille réelle
				tabCurrentCard = new int[realHeight, realWidth];
				caseSelectedCard = new Case[realHeight, realWidth];

				for (int x = 0; x < realHeight; x++)
				{
					for (int y = 0; y < realWidth; y++)
					{
						//tabCurrentCard[x, y] = currentPlayerCard.DrawingCaseValue[y, x];
						caseSelectedCard[x, y] = currentPlayerCard.Cases[y,realHeight-1- x];
					}
				}
			}

			//--- Angle Pi
			//  4 _ 3
			//  2|_|1
			//---
			if (cycleRotationCard == 2)
			{
				//---> Redimensionne le tableau représentant la carte
				//	   sur la map selon sa taille réelle
				tabCurrentCard = new int[realWidth, realHeight];
				caseSelectedCard = new Case[realWidth, realHeight];

				for (int x = 0; x < realWidth; x++)
				{
					for (int y = 0; y < realHeight; y++)
					{
						//tabCurrentCard[x, y] = currentPlayerCard.DrawingCaseValue[realWidth - x - 1, realHeight - y - 1];
						caseSelectedCard[x,y] = currentPlayerCard.Cases[realWidth - x - 1, realHeight - y - 1];
					}
				}
			}

			//--- Angle 3 Pi/2
			//  2 _ 4
			//  1|_|3
			//---
			if (cycleRotationCard == 3)
			{
				//---> Redimensionne le tableau représentant la carte
				//	   sur la map selon sa taille réelle
				tabCurrentCard = new int[realHeight, realWidth];
				caseSelectedCard = new Case[realHeight, realWidth];

				for (int x = 0; x < realHeight; x++)
				{
					for (int y = 0; y < realWidth; y++)
					{
						//tabCurrentCard[x, y] = currentPlayerCard.DrawingCaseValue[realWidth - y - 1, realHeight - x - 1];
						caseSelectedCard[x,y] = currentPlayerCard.Cases[realWidth - y - 1, x ];
					}
				}
			}

			//--- Calcul les valeurs de cases (2,4,8,16, etc...)
			CalcDrawingCaseValue(caseSelectedCard, ref tabCurrentCard);
			//---
		}

		private void CalcCardPositionOverMap()
		{
		}

		private Boolean AreCasesEqual(Case initialCase, Case[,] cases, int offsetX, int offsetY)
		{
			bool casesAreEqual = false;

			int width = cases.GetUpperBound(0) + 1;
			int height = cases.GetUpperBound(1) + 1;

			if (offsetX >= 0 && offsetX < width && offsetY >= 0 && offsetY < height)
			{
				Case offsetCase = cases[offsetX, offsetY];

				if (offsetCase != null &&
					initialCase.Player == offsetCase.Player &&
					initialCase.FlowerType == offsetCase.FlowerType)
					casesAreEqual = true;
			}

			return casesAreEqual;
		}

		private void NextPlayerToPlay()
		{
			if (currentPlayer == 0)
				currentPlayer = 1;
			else if (currentPlayer <= ListAllPlayerColor.Count)
				currentPlayer++;
			else
				currentPlayer = 1;
		}

		#region Drawing
		private void DrawPlayerCards(int numberPlayer)
		{
			Vector2 posDeck = new Vector2(posMap.X + Map.Width * caseSize + marge, posMap.Y);

			for (int numCard = 0; numCard < ListAllPlayerCard[numberPlayer].Count; numCard++)
			{
				for (int x = 0; x < widthPlayerCard; x++)
				{
					for (int y = 0; y < heightPlayerCard; y++)
					{
						int caseValue = ListAllPlayerCard[numberPlayer][numCard].DrawingCaseValue[x, y];

						if (caseValue >= 0)
						{
							Vector2 posCard = new Vector2(numCard * (smallMarge + widthPlayerCard * caseSize * scale) + x * caseSize * scale, y * caseSize * scale + (caseSize * scale * heightPlayerCard + marge) * (numberPlayer - 1));

							//--- Affiche le fond
							SpriteBatch.Draw(
								tex2DGround,
								posDeck + posCard,
								null,
								Color.White,
								0f,
								Vector2.Zero, scale, SpriteEffects.None, 0f
								);
							//---

							//--- Affiche le contour de la case
							Texture2D texCase = ContentManager.Load<Texture2D>(String.Format(@"Content\Pic\{0}", caseValue));

							SpriteBatch.Draw(
								texCase,
								posDeck + posCard,
								null,
								ListAllPlayerColor[numberPlayer],
								0f,
								Vector2.Zero, scale, SpriteEffects.None, 0f);
							//---
						}
					}
				}
			}
		}

		private void DrawSelectedCard()
		{
			//--- Calcul du centre de gravité
			Vector2 center = Vector2.Zero;
			for (int x = 0; x < widthPlayerCard; x++)
			{
				for (int y = 0; y < heightPlayerCard; y++)
				{
					int caseValue = currentPlayerCard.DrawingCaseValue[x, y];

					if (caseValue >= 0)
					{
						Vector2 posCard = new Vector2((float)x * (float)caseSize * scale * 1.5f, (float)y * (float)caseSize * scale * 1.5f);
						center += posCard;
					}
				}
			}

			center /= 4f;
			//---

			for (int x = 0; x < widthPlayerCard; x++)
			{
				for (int y = 0; y < heightPlayerCard; y++)
				{
					int caseValue = currentPlayerCard.DrawingCaseValue[x, y];

					if (caseValue >= 0)
					{
						Vector2 posCard = new Vector2((float)x * (float)caseSize * scale, (float)y * (float)caseSize * scale);

						Matrix mtxTransform =
							Matrix.CreateTranslation(new Vector3(-center, 0f)) *
							Matrix.CreateRotationZ(currentCardRotation) *
							Matrix.CreateTranslation(mouseState.X, mouseState.Y, 0f);

						SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState, mtxTransform);

						//--- Affiche le fond
						SpriteBatch.Draw(
							tex2DGround,
							posCard,
							null,
							Color.White,
							0, Vector2.Zero,
							scale, SpriteEffects.None, 0f
							);
						//---

						//--- Affiche le contour de la case
						Texture2D texCase = ContentManager.Load<Texture2D>(String.Format(@"Content\Pic\{0}", caseValue));

						SpriteBatch.Draw(
							texCase,
							posCard,
							null,
							ListAllPlayerColor[currentPlayerCard.Player],
							0, Vector2.Zero,
							scale, SpriteEffects.None, 0f);
						//---

						SpriteBatch.End();
					}
				}
			}
		}

		private Color GetColorFlower(FlowerType flowerType)
		{
			Color color = Color.White;

			switch (flowerType)
			{
				case FlowerType.None:
					break;
				case FlowerType.Red:
					color = Color.Red;
					break;
				case FlowerType.Green:
					color = Color.Green;
					break;
				case FlowerType.Blue:
					break;
					color = Color.Blue;
				default:
					break;
			}

			return color;
		}

		private void DrawMap()
		{
			float scale = 1f;

			for (int x = 0; x < Map.Width; x++)
			{
				for (int y = 0; y < Map.Height; y++)
				{
					if (Map.Cases[x, y] != null && Map.Cases[x, y].Player > 0)
					{
						Case mapCase = Map.Cases[x, y];
						int caseValue = Map.DrawingCaseValue[x, y];

						if (caseValue >= 0)
						{
							Vector2 posCard = new Vector2(x * caseSize * scale, y * caseSize * scale);

							//--- Affiche le fond
							SpriteBatch.Draw(
								tex2DGround,
								posMap + posCard,
								null,
								GetColorFlower(mapCase.FlowerType),
								0f, Vector2.Zero, scale, SpriteEffects.None, 0f
								);
							//---

							//--- Affiche le contour de la case
							Texture2D texCase = ContentManager.Load<Texture2D>(String.Format(@"Content\Pic\{0}", caseValue));

							SpriteBatch.Draw(
								texCase,
								posMap + posCard,
								null,
								ListAllPlayerColor[mapCase.Player],
								0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
							//---
						}
					}
					else
					{
						Vector2 posCard = new Vector2(x * caseSize * scale, y * caseSize * scale);

						//--- Affiche le fond
						SpriteBatch.Draw(
							tex2DGround,
							posMap + posCard + new Vector2(caseSize) * (scale + 0.1f) / 2f,
							null,
							Color.Peru,
							0f, new Vector2(caseSize) * (scale + 0.1f) / 2f, scale + 0.1f, SpriteEffects.None, 0f
							);
						//---

						//--- Affiche le fond
						SpriteBatch.Draw(
							tex2DGround,
							posMap + posCard,
							null,
							Color.SaddleBrown,
							0f, Vector2.Zero, scale, SpriteEffects.None, 0f
							);
						//---
					}
				}
			}
		}

		private void DrawSelectedCardOverMap()
		{
			int width = tabCurrentCard.GetUpperBound(0) + 1;
			int height = tabCurrentCard.GetUpperBound(1) + 1;

			float scale = 1f;
			Vector2 centerCase = new Vector2(caseSize / 2f);

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					int caseValue = tabCurrentCard[x, y];

					if (caseValue >= 0)
					{
						Vector2 posCard = new Vector2(x * caseSize * scale, y * caseSize * scale) + centerCase;

						//--- Affiche le fond
						SpriteBatch.Draw(
							tex2DGround,
							posMap + posCard,
							null,
							GetColorFlower(currentPlayerCard.Cases[0, 0].FlowerType),
							0f,
							//0f,
							centerCase, 
							//Vector2.Zero,
							scale, SpriteEffects.None, 0f
							);
						//---

						//--- Affiche le contour de la case
						Texture2D texCase = ContentManager.Load<Texture2D>(String.Format(@"Content\Pic\{0}", caseValue));

						SpriteBatch.Draw(
							texCase,
							posMap + posCard,
							null,
							ListAllPlayerColor[currentPlayer],
							0f, 
							//0f,
							centerCase, 
							//Vector2.Zero,
							scale, SpriteEffects.None, 0f);
						//---
					}
				}
			}
		}
		#endregion

		#region Events
		void clickableCardZone_ClickZone(ClickableZone zone, MouseState mouseState, GameTime gameTime)
		{
			//---> La carte peut être sélectionnée
			PlayerCard playerCard = (PlayerCard)zone.Tag;
			if (
					(contextType == ContextType.None && playerCard.Player == currentPlayer) ||
					(contextType == ContextType.CardSelected && playerCard.Player == currentPlayer && currentPlayerCard != playerCard)
				)
			{
				currentPlayerCard = playerCard;
				contextType = ContextType.CardSelected;

				CalcTabSelectedCard();
				return;
			}

			//---> Déselectionnne la carte si elle est de nouveau cliquée
			if (contextType == ContextType.CardSelected && playerCard.Player == currentPlayer && currentPlayerCard == playerCard)
			{
				currentPlayerCard = null;
				contextType = ContextType.None;
				return;
			}
		}

		void mapZone_MouseEnter(ClickableZone zone, MouseState mouseState, GameTime gameTime)
		{
			//---> Si le pointeur est au dessus de la Map alors le context change
			if (contextType == ContextType.CardSelected)
				contextType = ContextType.CardOverMap;

			//---> Si le pointeur est au dessus de la Map et la carte est en train de tourner
			//	   alors le context change et la rotation prend fin
			if (contextType == ContextType.CardRotated)
			{
				contextType = ContextType.CardOverMap;
				currentCardRotation = lerpRotation.Max;
			}

			//---> Si la carte est au dessus de la map
			//	   calcul de la position de la carte sur la map
			if (contextType == ContextType.CardOverMap)
			{
				CalcCardPositionOverMap();
			}
		}

		void mapZone_MouseLeave(ClickableZone zone, MouseState mouseState, GameTime gameTime)
		{
			if (contextType == ContextType.CardOverMap)
				contextType = ContextType.CardSelected;
		}
		#endregion
	}
}
