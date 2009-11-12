using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Flowar
{
    public class GameFlowar : GameBase
    {
        private Texture2D tex2DGround;

        private int currentPlayer = 1;

        public Map Map { get; set; }
        public List<ModelCard> ListModelCard { get; set; }
        public Dictionary<int, List<PlayerCard>> ListAllPlayerCard { get; set; }
        public Dictionary<int, Color> ListAllPlayerColor { get; set; }

        private Vector2 posMap = new Vector2(50, 70);

        int caseSize = 73;
        int marge = 35;
        int smallMarge = 10;
        Random rnd = new Random();

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

        void GameFlowar_MouseRightButtonClicked(MouseState mouseState, GameTime gameTime)
        {
        }

        void GameFlowar_MenuAnimationOpenEnded(GameTime gameTime)
        {
        }

        void GameFlowar_KeyPressed(Keys key, GameTime gameTime)
        {
        }

        void GameFlowar_MouseLeftButtonClicked(MouseState mouseState, GameTime gameTime)
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            //if (!IsGameInitialized)
            //{
            //    IsGameInitialized = true;
            //    StartGame();
            //}

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(20, 20, 20));

            SpriteBatch.Begin();

            DrawMap();

            for (int i = 1; i <= ListAllPlayerColor.Keys.Count; i++)
            {
                DrawPlayerCards(i);
            }

            base.Draw(gameTime);

            SpriteBatch.End();
        }

        private void SelectCard()
        {
        }

        private void StartGame()
        {
            CreateMap();

            CreateCardModels();

            for (int i = 1; i <= ListAllPlayerColor.Keys.Count; i++)
            {
                DistributeCards(i);
            }
        }


        private void CreateMap()
        {
            int width = 6;
            int height = 6;

            this.Map = new Map(width, height);
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

        #region Drawing


        private void DrawPlayerCards(int numberPlayer)
        {
            int widthPlayerCard = 2;
            int heightPlayerCard = 4;
            float scale = 0.5f;

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
                                0f, Vector2.Zero, scale, SpriteEffects.None, 0f
                                );
                            //---

                            //--- Affiche le contour de la case
                            Texture2D texCase = ContentManager.Load<Texture2D>(String.Format(@"Content\Pic\{0}",caseValue));

                            SpriteBatch.Draw(
                                texCase,
                                posDeck + posCard,
                                null,
                                ListAllPlayerColor[numberPlayer],
                                0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                            //---
                        }
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
                            Texture2D texCase = ContentManager.Load<Texture2D>(caseValue.ToString());

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
        #endregion
    }
}
