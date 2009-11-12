using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Flowar
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            this.IsMouseVisible = true;

            Content.RootDirectory = "Content\\Pic";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            KeyboardState keyboardState = new KeyboardState();

            if (!IsGameInitialized)
            {
                IsGameInitialized = true;
                StartGame();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            DrawMap();

            DrawPlayerCards(1);

            base.Draw(gameTime);
        }

        private void StartGame()
        {
            InitGame();

            CreateMap();

            CreateCardModels();

            DistributeCards(1);
        }

        public Map Map { get; set; }
        public List<ModelCard> ListModelCard { get; set; }
        public Dictionary<int, List<PlayerCard>> ListAllPlayerCard { get; set; }
        public Dictionary<int, Color> ListAllPlayerColor { get; set; }
        private bool IsGameInitialized = false;

        private Vector2 posMap = new Vector2(50, 50);

        int caseSize = 73;
        int marge = 50;
        int smallMarge = 10;

        private void InitGame()
        {
            ListAllPlayerCard = new Dictionary<int, List<PlayerCard>>();
            ListAllPlayerColor = new Dictionary<int, Color>();

            ListAllPlayerColor.Add(1, Color.Orange);
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
            Random rnd = new Random();

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
            int width = cases.GetUpperBound(0)+1;
            int height = cases.GetUpperBound(1)+1;

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

                    if(caseValue == 0)
                        caseValue = 0;

                    casesValue[x, y] = caseValue;
                }
            }
        }

        private Boolean AreCasesEqual(Case initialCase, Case[,] cases, int offsetX, int offsetY)
        {
            bool casesAreEqual = false;

            int width = cases.GetUpperBound(0)+1;
            int height = cases.GetUpperBound(1)+1;

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
            float scale =0.5f;

            Texture2D texCaseGround = Content.Load<Texture2D>("Fond");

            Vector2 posDeck = new Vector2(posMap.X + Map.Width * caseSize + marge, posMap.Y);

            spriteBatch.Begin();

            for (int numCard = 0; numCard < ListAllPlayerCard[numberPlayer].Count; numCard++)
            {
                for (int x = 0; x < widthPlayerCard; x++)
                {
                    for (int y = 0; y < heightPlayerCard; y++)
                    {
                        int caseValue = ListAllPlayerCard[numberPlayer][numCard].DrawingCaseValue[x, y];
                        
                        if (caseValue >= 0)
                        {
                            Vector2 posCard = new Vector2(numCard * (smallMarge + widthPlayerCard * caseSize*scale) + x * caseSize*scale, y * caseSize*scale + (caseSize*scale * heightPlayerCard + marge) * (numberPlayer - 1));

                            //--- Affiche le fond
                            spriteBatch.Draw(
                                texCaseGround,
                                posDeck + posCard, 
                                null,
                                Color.White,
                                0f,Vector2.Zero, scale, SpriteEffects.None, 0f
                                );
                            //---

                            //--- Affiche le contour de la case
                            Texture2D texCase = Content.Load<Texture2D>(caseValue.ToString());

                            spriteBatch.Draw(
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

            spriteBatch.End();
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

            Texture2D texCaseGround = Content.Load<Texture2D>("Fond");

            spriteBatch.Begin();

            for (int x = 0; x < Map.Width; x++)
            {
                for (int y = 0; y < Map.Height; y++)
                {
                    if (Map.Cases[x, y] != null && Map.Cases[x,y].Player>0)
                    {
                        Case mapCase =Map.Cases[x, y];
                        int caseValue = Map.DrawingCaseValue[x,y];

                        if (caseValue >= 0)
                        {
                            Vector2 posCard = new Vector2(x * caseSize * scale, y * caseSize * scale);

                            //--- Affiche le fond
                            spriteBatch.Draw(
                                texCaseGround,
                                posMap + posCard,
                                null,
                                GetColorFlower(mapCase.FlowerType),
                                0f, Vector2.Zero, scale, SpriteEffects.None, 0f
                                );
                            //---

                            //--- Affiche le contour de la case
                            Texture2D texCase = Content.Load<Texture2D>(caseValue.ToString());

                            spriteBatch.Draw(
                                texCase,
                                posMap + posCard,
                                null,
                                ListAllPlayerColor[mapCase.Player],
                                0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                            //---
                        }
                    }
                }
            }

            spriteBatch.End();
        }
        #endregion
    }
}
