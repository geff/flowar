using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Flowar.Tools;
using Flowar.Model.Enum;
using Flowar.Render;

namespace Flowar
{
    public class GameFlowar : GameBase
    {
        #region Fields
        /// <summary>
        /// Joueur actif
        /// </summary>
        public int CurrentPlayer = 0;

        /// <summary>
        /// Carte sélectionnée par le joueur actif
        /// </summary>
        public PlayerCard CurrentPlayerCard = null;

        /// <summary>
        /// Rotation de la carte sélectionnée
        /// </summary>
        public float CurrentCardRotation;

        /// <summary>
        /// Clone de la carte sélectionnée (pour les rotations)
        /// </summary>
        public PlayerCard CloneSelectedCard = null;

        /// <summary>
        /// Cycle de rotation de la carte
        /// </summary>
        private int cycleRotationCard;

        /// <summary>
        /// Case de la carte sélectionnée en cours de pose sur la map
        /// </summary>
        private int currentCasePuttingDown = -1;

        /// <summary>
        /// Position de la carte sélectionnée sur la map
        /// </summary>
        public Point PosSelectedCardOnMap;

        /// <summary>
        /// précédente valeur de la  roulette, pour tourner la carte
        /// </summary>
        private int prevMouseWheel = 0;

        /// <summary>
        /// Contexte utilisateur
        /// </summary>
        public ContextType ContextType = ContextType.None;

        /// <summary>
        /// Map
        /// </summary>
        public Map Map { get; set; }

        /// <summary>
        /// Liste des cartes modèles
        /// </summary>
        public List<ModelCard> ListModelCard { get; set; }

        /// <summary>
        /// Liste des cattes distribuées pour chaque joueur
        /// </summary>
        public Dictionary<int, List<PlayerCard>> ListAllPlayerCard { get; set; }

        /// <summary>
        /// Liste des couleurs pour les joueurs (détermine également le nombre de joueurs)
        /// </summary>
        public Dictionary<int, Color> ListAllPlayerColor { get; set; }

        private int[,] battleCoeff;

        /// <summary>
        /// Position de la map dessinée
        /// </summary>
        public Vector2 PosMap = new Vector2(50, 120);

        /// <summary>
        /// Taille en pixel d'une case
        /// </summary>
        public int CaseSize = 64;

        /// <summary>
        /// Marge entrer la map et les cartes
        /// </summary>
        public int Marge = 10;

        /// <summary>
        /// Marge entre les cartes
        /// </summary>
        public int SmallMarge = 10;

        /// <summary>
        /// Largeur d'une carte standard
        /// </summary>
        public int WidthPlayerCard = 2;

        /// <summary>
        /// Hauteur d'une carte standard
        /// </summary>
        public int HeightPlayerCard = 4;

        /// <summary>
        /// Echelle des cartes
        /// </summary>
        public float scale = 0.5f;

        /// <summary>
        /// Echelle de la carte sélectionnée (animation)
        /// </summary>
        public float ScaleSelectedCard = 0.5f;

        /// <summary>
        /// Echelle de la carte lorsque'elle est posée sur la map
        /// </summary>
        public float ScaleCardPuttingDown = 0f;

        public float ScaleMap = 1f;
        public float ScaleCardOverMap = 1.15f;

        /// <summary>
        /// Générateur de nombres aléatoires
        /// </summary>
        Random rnd = new Random();

        /// <summary>
        /// Lerp pour l'échelle de la carte sélectionnée
        /// </summary>
        Lerp lerpScale;

        /// <summary>
        /// Lerp pour la rotation de la carte sélectionnée
        /// </summary>
        Lerp lerpRotation;

        /// <summary>
        /// Lerp de taille de la carte lorsqu'elle est posée sur la map
        /// </summary>;
        Lerp lerpScaleCardPuttingDown;

        /// <summary>
        /// Lerp Alpha pour la case en cours lorsque la carte est posée sur la map
        /// </summary>;
        Lerp lerpAlphaCasePuttingDown;

        Lerp lerpCardTranslationX;
        Lerp lerpCardTranslationY;

        public Vector2 VecPosSelectedCardOnMap = Vector2.Zero;

        int durationCardAnimation = 100;
        int durationCardPuttingDown = 175;

        /// <summary>
        /// Carte clonée avant animation
        /// </summary>
        PlayerCard oldCloneSelectedCard;

        public bool DoScreenShot = false;

        public Dictionary<int, MapTile> ListMapTile { get; set; }

        private const int TILE_ID_GRASS = 1000;

        public List<Point> ListPlayerBasePosition { get; set; }

        private int mapWidth;
        private int mapHeight;

        private int curNumberCaseGrowing;
        #endregion

        RenderLogic renderLogic;

        #region Constructeur / Init
        public GameFlowar(GameMain game, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, ContentManager contentManager)
            : base(game, spriteBatch, graphicsDevice, contentManager)
        {
            this.Game = game;

            Init();
        }

        public override void Init()
        {
            //--- Initialisation des logic
            renderLogic = new RenderLogic(this);
            //---

            this.MouseRightButtonClicked += new MouseRightButtonClickedHandler(GameFlowar_MouseRightButtonClicked);
            this.MouseLeftButtonClicked += new MouseLeftButtonClickedHandler(GameFlowar_MouseLeftButtonClicked);
            this.MouseWheelChanged += new MouseWheelChangeddHandler(GameFlowar_MouseWheelChanged);
            this.KeyPressed += new KeyPressedHandler(GameFlowar_KeyPressed);
            this.MenuAnimationOpenEnded += new MenuAnimationOpenEndedHandler(GameFlowar_MenuAnimationOpenEnded);

            //---
            //txtMenu = new ClickableImage(ContentManager.Load<Texture2D>(@"Content\Pic\Menu_On"), ContentManager.Load<Texture2D>(@"Content\Pic\Menu_Off"), new Vector2(50, 18));
            //txtMenu.ClickImage += new ClickableImage.ClickImageHandler(txtMenu_ClickImage);
            //this.AddClickableImage(txtMenu);
            ClickableText txtNewMap = new ClickableText(this, "FontCase1", "FontCase1", "New map", new Vector2(50, 18));
            txtNewMap.Clicked += new ClickableZone.ClickZoneHandler(txtNewMap_Clicked);
            this.AddClickableZone(txtNewMap);

            //---
            this.AddKeys(Keys.P);

            //--- Chargement des images de la carte
            ListMapTile = new Dictionary<int, MapTile>();

            ListMapTile.Add(1000, new MapTile(1000, "Herbe", 0f, new Vector2(0f, -28f)));
            ListMapTile.Add(1260, new MapTile(1260, "Eau_H", 0f, Vector2.Zero));
            ListMapTile.Add(1080, new MapTile(1080, "Eau_H", MathHelper.PiOver2, Vector2.Zero));
            ListMapTile.Add(1320, new MapTile(1320, "Eau_C", 0f, new Vector2(-16, -16f)));
            ListMapTile.Add(1272, new MapTile(1272, "Eau_C", MathHelper.PiOver2, new Vector2(16f, -16f)));
            ListMapTile.Add(1020, new MapTile(1020, "Eau_C", MathHelper.Pi, new Vector2(-16, -16f)));
            ListMapTile.Add(1680, new MapTile(1680, "Eau_C", -MathHelper.PiOver2, new Vector2(-16, 16f)));
            //ListMapTile.Add(2000, new MapTile(2000, "FleurRelais", 0f, new Vector2(0f, -28f), Color.Green));
            //ListMapTile.Add(2001, new MapTile(2001, "FleurRelais", 0f, new Vector2(0f, -28f), Color.Yellow));
            //ListMapTile.Add(2002, new MapTile(2002, "FleurRelais", 0f, new Vector2(0f, -28f), Color.Aqua));
            //ListMapTile.Add(2003, new MapTile(2003, "FleurRelais", 0f, new Vector2(0f, -28f), Color.Orange));
            //---

            //---
            mapWidth = 10;
            mapHeight = 10;
            //---

            //---
            rnd = new Random();
            this.ShowMiniMenu = true;
            ListAllPlayerCard = new Dictionary<int, List<PlayerCard>>();
            ListAllPlayerColor = new Dictionary<int, Color>();
            lerpScale = new Lerp(scale, scale + 0.05f, true, true, 400, -1);

            ListAllPlayerColor.Add(1, Color.Orange);
            ListAllPlayerColor.Add(2, Color.Coral);
            ListAllPlayerColor.Add(3, Color.SteelBlue);
            //---

            //--- Position des bases des joueurs
            ListPlayerBasePosition = new List<Point>();

            ListPlayerBasePosition.Add(new Point(0, 0));
            ListPlayerBasePosition.Add(new Point(mapWidth - 1, 0));
            ListPlayerBasePosition.Add(new Point(mapWidth - 1, mapHeight - 1));
            //---


            //--- Initialisation des coefficients de batailles
            //---> La première dimension correspond aux défenseurs,
            //	   tandis que la second dimension correspond aux attaquants
            //	   Si le rapport est en faveur du défenseur la valeur = 1
            //	   Si le rapport est neutre la valeur est 0
            //	   Si le rapport est en faveur de l'attaquant la valeur est -1

            battleCoeff = new int[4, 4];

            battleCoeff[(int)FlowerType.Red, (int)FlowerType.Red] = 0;
            battleCoeff[(int)FlowerType.Red, (int)FlowerType.Green] = 1;
            battleCoeff[(int)FlowerType.Red, (int)FlowerType.Blue] = -1;

            battleCoeff[(int)FlowerType.Green, (int)FlowerType.Red] = -1;
            battleCoeff[(int)FlowerType.Green, (int)FlowerType.Green] = 0;
            battleCoeff[(int)FlowerType.Green, (int)FlowerType.Blue] = 1;

            battleCoeff[(int)FlowerType.Blue, (int)FlowerType.Red] = 1;
            battleCoeff[(int)FlowerType.Blue, (int)FlowerType.Green] = -1;
            battleCoeff[(int)FlowerType.Blue, (int)FlowerType.Blue] = 0;
            //---

            //---
            StartGame();
            //---

            base.Init();
        }
        #endregion

        #region Évènements Input / Menu
        void GameFlowar_MouseLeftButtonClicked(MouseState mouseState, GameTime gameTime)
        {
        }

        void GameFlowar_MouseRightButtonClicked(MouseState mouseState, GameTime gameTime)
        {
        }

        void GameFlowar_MouseWheelChanged(MouseState mouseState, GameTime gameTime)
        {
            //---> Si la différence de scroll est inférieure à 120
            //	   ne pas considérer le scroll
            if (Math.Abs(prevMouseWheel - mouseState.ScrollWheelValue) < 120)
                return;

            //---> Rotation de la carte lorsqu'ele est sélectionnée
            if (ContextType == ContextType.CardSelected ||
                ContextType == ContextType.CardOverMap)
            {
                int sens = prevMouseWheel >= mouseState.ScrollWheelValue ? 1 : -1;

                lerpRotation = new Lerp(CurrentCardRotation, CurrentCardRotation + MathHelper.PiOver2 * sens, false, false, durationCardAnimation, -1);

                cycleRotationCard = (cycleRotationCard + sens) % 4;
                if (cycleRotationCard < 0)
                    cycleRotationCard += 4;

                if (ContextType == ContextType.CardSelected)
                    ContextType = ContextType.CardRotated;

                if (ContextType == ContextType.CardOverMap)
                    ContextType = ContextType.CardRotatedOverMap;

                //oldCenterRotated = cloneSelectedCard.Center;
                oldCloneSelectedCard = CloneSelectedCard.Clone();

                CalcTabRotatedCard();

                CalcCardPositionOverMap(mouseState, true, true);
            }

            prevMouseWheel = mouseState.ScrollWheelValue;
        }

        void GameFlowar_MenuAnimationOpenEnded(GameTime gameTime)
        {
        }

        void GameFlowar_KeyPressed(Keys key, GameTime gameTime)
        {
            if (key == Keys.P)
            {
                this.DoScreenShot = true;
            }
        }

        void txtNewMap_Clicked(ClickableZone zone, MouseState mouseState, GameTime gameTime)
        {
            StartGame();
        }
        #endregion

        public override void Update(GameTime gameTime)
        {

            //---> Grossissement de la carte lorsqu'elle est sélectionnée
            if (ContextType == ContextType.CardRotated ||
                ContextType == ContextType.CardSelected)
            {
                ScaleSelectedCard = lerpScale.Eval(gameTime);
            }

            //---> Animation de rotation de la carte
            if (ContextType == ContextType.CardRotated)
            {
                CurrentCardRotation = lerpRotation.Eval(gameTime);

                if (lerpRotation.IsFinished(gameTime))
                    ContextType = ContextType.CardSelected;
            }

            //---> Animation de rotation de la carte sur la map
            if (ContextType == ContextType.CardRotatedOverMap)
            {
                CurrentCardRotation = lerpRotation.Eval(gameTime);

                if (lerpRotation.IsFinished(gameTime))
                {
                    ContextType = ContextType.CardOverMap;
                    //TODO : voir si cette ligne de code est utile par la suite
                    oldCloneSelectedCard = CloneSelectedCard;
                }
            }

            //---> Animation de translation sur la map
            if (lerpCardTranslationX != null)
            {
                VecPosSelectedCardOnMap.X = (int)lerpCardTranslationX.Eval(gameTime);
                VecPosSelectedCardOnMap.Y = (int)lerpCardTranslationY.Eval(gameTime);

                if (lerpCardTranslationX.IsFinished(gameTime) &&
                    lerpCardTranslationY.IsFinished(gameTime))
                {
                    lerpCardTranslationX = null;
                    lerpCardTranslationY = null;
                }
            }

            //---> Animation des cases lorsque la carte est posée sur la map
            if (ContextType == ContextType.PutDownCard)
            {
                //if (currentCasePuttingDown == -1 ||
                //    (lerpCasePuttingDown != null && lerpCasePuttingDown.IsFinished(gameTime)))
                //{
                //    //--- Passe à la case suivante
                //    lerpCasePuttingDown = new Lerp(0, 1f, false, false, 150, -1);
                //    PutDownNextCase();
                //    //---
                //}

                //---> Mise à l'échelle de la case en cours de pose
                ScaleCardPuttingDown = lerpScaleCardPuttingDown.Eval(gameTime);

                if (lerpScaleCardPuttingDown.IsFinished(gameTime))
                {
                    CalcMapAfterCardPuttingDown();

                    //NextPlayerToPlay();
                }
            }

            if (ContextType == ContextType.GrowingCase)
            {
                UpdateGrowingCase(gameTime, CurrentPlayer, 0.001f);
            }

            if (ContextType == ContextType.NextPlayerToPlay)
            {
                NextPlayerToPlay();
            }

            base.Update(gameTime);
        }

        private void UpdateGrowingCase(GameTime gameTime, int player, float speedGrowing)
        {
            bool areCaseGrowing = false;

            for (int x = 0; x < Map.Width; x++)
            {
                for (int y = 0; y < Map.Height; y++)
                {
                    if (Map.Cases[x, y] is PlayerCase &&
                        ((PlayerCase)Map.Cases[x, y]).Player == player &&
                        ((PlayerCase)Map.Cases[x, y]).GrowingCase)
                    {
                        areCaseGrowing = true;

                        PlayerCase playerCase = (PlayerCase)Map.Cases[x, y];

                        if (playerCase.GrowingStartTime == TimeSpan.Zero)
                            playerCase.GrowingStartTime = gameTime.TotalGameTime;

                        //--- Met à jour le pourcentage d'avancement de la croissance
                        playerCase.PercentageGrowingCase =
                            (float)gameTime.TotalGameTime.Subtract(playerCase.GrowingStartTime).TotalMilliseconds *
                            speedGrowing /
                            Math.Abs(playerCase.StartValueGrowingCase - playerCase.EndValueGrowingCase);
                        //---

                        //--- Si la croissance est terminée, passe aux cases voisines
                        if (playerCase.PercentageGrowingCase >= 1f)
                        {
                            playerCase.PercentageGrowingCase = 1f;
                            playerCase.GrowingCase = false;
                            playerCase.NewCase = false;

                            for (int x2 = -1; x2 < 2; x2++)
                            {
                                for (int y2 = -1; y2 < 2; y2++)
                                {
                                    if ((x2 == 0 || y2 == 0) &&
                                        x + x2 >= 0 && x + x2 < Map.Width &&
                                        y + y2 >= 0 && y + y2 < Map.Height)
                                    {
                                        Point curPoint = new Point(x + x2, y + y2);

                                        if (Map.Cases[curPoint.X, curPoint.Y] is PlayerCase &&
                                            ((PlayerCase)Map.Cases[curPoint.X, curPoint.Y]).Player == player &&
                                            ((PlayerCase)Map.Cases[curPoint.X, curPoint.Y]).NewCase)
                                        {
                                            ((PlayerCase)Map.Cases[curPoint.X, curPoint.Y]).GrowingCase = true;
                                        }
                                    }
                                }
                            }
                        }
                        //---
                    }
                }
            }

            //--- Si aucune case n'est en cours de croissance passe à l'état suivant
            if (!areCaseGrowing)
            {
                ContextType = ContextType.NextPlayerToPlay;
            }
            //---
        }

        private void StartGame()
        {
            ListAllPlayerCard = new Dictionary<int, List<PlayerCard>>();

            CreateMap();

            CreateCardModels();

            for (int i = 1; i <= ListAllPlayerColor.Keys.Count; i++)
            {
                DistributeCards(i);
            }

            CreateCardClickableZone();

            NextPlayerToPlay();
        }

        private void CreateMap()
        {
            this.Map = new Map(mapWidth, mapHeight);

            for (int x = 0; x < Map.Width; x++)
            {
                for (int y = 0; y < Map.Height; y++)
                {
                    //---> Recouvre le terrain d'herbe
                    this.Map.Cases[x, y] = new BaseCase(TILE_ID_GRASS);
                }
            }

            //=== Calcul de la position de la rivière

            //---> Le début de la rivière est dans le deuxième quart de la carte
            int riverStart = rnd.Next((int)((float)this.Map.Width * 0.25f), this.Map.Width / 2 + 1);

            //---> La fin de la rivière est dans le troisième quart de la carte
            int riverEnd = rnd.Next(this.Map.Width / 2, (int)((float)this.Map.Width * 0.75f) + 1);

            int numberOfRiverCorner = riverEnd - riverStart;
            int riverX = riverStart;
            int[] riverYCorner = new int[numberOfRiverCorner];

            //---> Détermine les coins de la rivière
            for (int i = 0; i < numberOfRiverCorner; i++)
            {
                riverYCorner[i] = rnd.Next(this.Map.Width / numberOfRiverCorner * i, this.Map.Width / numberOfRiverCorner * (i + 1));
            }

            for (int y = 0; y < Map.Height; y++)
            {
                //---> Pose la rivière sur la carte
                if (riverYCorner.Contains<int>(y))
                {
                    //---> La rivière tourne du haut vers la droite
                    this.Map.Cases[riverX, y] = new BaseCase(1680);
                    //---> La rivière toune de la gauche vers le bas
                    this.Map.Cases[++riverX, y] = new BaseCase(1272);
                }
                else
                {
                    //---> La rivière coule de haut en bas
                    this.Map.Cases[riverX, y] = new BaseCase(1080);
                }
            }
            //===

            //--- Création des relais
            int nmbRelay = 6;

            for (int idRelay = 0; idRelay < nmbRelay; idRelay++)
            {
                bool relayCasePositionFound = false;
                RelayCase relayCase = new RelayCase((RelayType)rnd.Next(1, 4), 1f);
                relayCase.TileId = TILE_ID_GRASS;

                while (!relayCasePositionFound)
                {
                    int x = rnd.Next(this.Map.Width);
                    int y = rnd.Next(this.Map.Height);

                    if (this.Map.Cases[x, y].TileId == TILE_ID_GRASS)
                    {
                        this.Map.Cases[x, y] = relayCase;
                        relayCasePositionFound = true;
                    }
                }
            }
            //---

            //--- Création des bases des joueurs
            for (int i = 0; i < ListPlayerBasePosition.Count; i++)
            {
                PlayerCase playerBaseCase = new PlayerCase();
                playerBaseCase.TileId = TILE_ID_GRASS;
                playerBaseCase.Player = i + 1;
                playerBaseCase.FlowerType = FlowerType.Blue;

                Map.Cases[ListPlayerBasePosition[i].X, ListPlayerBasePosition[i].Y] = playerBaseCase;
            }
            //---

            //--- Zone de la map intéractive avec la souris
            ClickableZone mapZone = new ClickableZone(PosMap, Map.Width * CaseSize, Map.Height * CaseSize);
            mapZone.MouseEnter += new ClickableZone.ClickZoneMouseEnterHandler(mapZone_MouseEnter);
            mapZone.MouseLeave += new ClickableZone.ClickZoneMouseLeaveHandler(mapZone_MouseLeave);
            mapZone.Clicked += new ClickableZone.ClickZoneHandler(mapZone_Clicked);

            this.AddClickableZone(mapZone);
            //---
        }

        private void CreateCardModels()
        {
            this.ListModelCard = new List<ModelCard>();

            //--- L
            ModelCard modelCard = new ModelCard();

            modelCard.Center = new Point(0, 1);

            modelCard.Cases[0, 0] = new PlayerCase() { FlowerType = FlowerType.None };
            modelCard.Cases[0, 1] = new PlayerCase() { FlowerType = FlowerType.None };
            modelCard.Cases[0, 2] = new PlayerCase() { FlowerType = FlowerType.None };
            modelCard.Cases[1, 2] = new PlayerCase() { FlowerType = FlowerType.None };
            modelCard.CalcDimensions();

            this.ListModelCard.Add(modelCard);
            //---

            //--- []
            modelCard = new ModelCard();

            modelCard.Center = new Point(0, 0);

            modelCard.Cases[0, 0] = new PlayerCase() { FlowerType = FlowerType.None };
            modelCard.Cases[0, 1] = new PlayerCase() { FlowerType = FlowerType.None };
            modelCard.Cases[1, 0] = new PlayerCase() { FlowerType = FlowerType.None };
            modelCard.Cases[1, 1] = new PlayerCase() { FlowerType = FlowerType.None };
            modelCard.CalcDimensions();

            this.ListModelCard.Add(modelCard);
            //---

            //--- I
            modelCard = new ModelCard();

            modelCard.Center = new Point(0, 1);

            modelCard.Cases[0, 0] = new PlayerCase() { FlowerType = FlowerType.None };
            modelCard.Cases[0, 1] = new PlayerCase() { FlowerType = FlowerType.None };
            modelCard.Cases[0, 2] = new PlayerCase() { FlowerType = FlowerType.None };
            modelCard.Cases[0, 3] = new PlayerCase() { FlowerType = FlowerType.None };
            modelCard.CalcDimensions();

            this.ListModelCard.Add(modelCard);
            //---

            //--- T
            modelCard = new ModelCard();

            modelCard.Center = new Point(0, 1);

            modelCard.Cases[0, 0] = new PlayerCase() { FlowerType = FlowerType.None };
            modelCard.Cases[0, 1] = new PlayerCase() { FlowerType = FlowerType.None };
            modelCard.Cases[0, 2] = new PlayerCase() { FlowerType = FlowerType.None };
            modelCard.Cases[1, 1] = new PlayerCase() { FlowerType = FlowerType.None };
            modelCard.CalcDimensions();

            this.ListModelCard.Add(modelCard);
            //---

            //--- Z
            modelCard = new ModelCard();

            modelCard.Center = new Point(0, 1);

            modelCard.Cases[0, 0] = new PlayerCase() { FlowerType = FlowerType.None };
            modelCard.Cases[0, 1] = new PlayerCase() { FlowerType = FlowerType.None };
            modelCard.Cases[1, 1] = new PlayerCase() { FlowerType = FlowerType.None };
            modelCard.Cases[1, 2] = new PlayerCase() { FlowerType = FlowerType.None };
            modelCard.CalcDimensions();

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

            for (int i = 0; i <= numberCards; i++)
            {
                PlayerCard playerCard = new PlayerCard();
                ModelCard modeCard = ListModelCard[rnd.Next(0, ListModelCard.Count)].Clone();

                playerCard.Cases = modeCard.Cases;
                playerCard.DrawingCaseValue = modeCard.DrawingCaseValue;
                playerCard.Player = numberPlayer;
                playerCard.Center = new Point(modeCard.Center.X, modeCard.Center.Y);
                playerCard.FlowerType = (FlowerType)((int)rnd.Next(1, 4));
                playerCard.CardType = CardType.None;

                ListAllPlayerCard[numberPlayer].Add(playerCard);
            }
        }

        private void CreateCardClickableZone()
        {
            Vector2 posDeck = new Vector2(PosMap.X + Map.Width * CaseSize + Marge, PosMap.Y);

            for (int numberPlayer = 1; numberPlayer <= ListAllPlayerColor.Keys.Count; numberPlayer++)
            {
                for (int numCard = 0; numCard < ListAllPlayerCard[numberPlayer].Count; numCard++)
                {
                    Vector2 posCard = posDeck + new Vector2(
                        numCard * (SmallMarge + WidthPlayerCard * CaseSize * scale),
                        (CaseSize * scale * HeightPlayerCard + Marge) * (numberPlayer - 1));

                    ClickableZone clickableCardZone = new ClickableZone(posCard, (int)(CaseSize * scale * WidthPlayerCard), (int)(CaseSize * scale * HeightPlayerCard));
                    clickableCardZone.Tag = ListAllPlayerCard[numberPlayer][numCard];

                    clickableCardZone.Clicked += new ClickableZone.ClickZoneHandler(clickableCardZone_ClickZone);

                    this.AddClickableZone(clickableCardZone);
                }
            }
        }

        private void InitCardValues(PlayerCard playerCard, CardType cardType)
        {
            playerCard.CardType = cardType;

            float maxValue = 100f;
            float maxBonus = 1.25f;
            float maxMalus = 0.75f;

            float minValue = 50f;
            float minBonus = 1.25f;
            float minMalus = 0.75f;

            for (int x = 0; x < WidthPlayerCard; x++)
            {
                for (int y = 0; y < HeightPlayerCard; y++)
                {
                    if (playerCard.Cases[x, y] != null)
                    {
                        playerCard.Cases[x, y].Player = playerCard.Player;

                        if (cardType == CardType.Defenser)
                        {
                            playerCard.Cases[x, y].Defenser = maxValue;
                            playerCard.Cases[x, y].BonusDefenser = maxBonus;
                            playerCard.Cases[x, y].MalusDefenser = maxMalus;

                            playerCard.Cases[x, y].Stricker = minValue;
                            playerCard.Cases[x, y].BonusStricker = minBonus;
                            playerCard.Cases[x, y].MalusStricker = minMalus;
                        }
                        else if (cardType == CardType.Stricker)
                        {
                            playerCard.Cases[x, y].Stricker = maxValue;
                            playerCard.Cases[x, y].BonusStricker = maxBonus;
                            playerCard.Cases[x, y].MalusStricker = maxMalus;

                            playerCard.Cases[x, y].Defenser = minValue;
                            playerCard.Cases[x, y].BonusDefenser = minBonus;
                            playerCard.Cases[x, y].MalusDefenser = minMalus;
                        }
                    }
                }
            }
        }

        private void CalcDrawingCaseValue(BaseCase[,] cases, ref int[,] casesValue)
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

        private void CalcTabRotatedCard()
        {
            CloneSelectedCard = CurrentPlayerCard.Clone();

            //Case[,] cloneSelectedCard.Cases = new Case[0,0];

            //--- Détermine la taille réelle de la carte sélectionnée
            int width = CurrentPlayerCard.Cases.GetUpperBound(0) + 1;
            int height = CurrentPlayerCard.Cases.GetUpperBound(1) + 1;

            int realWidth = 0;
            int realHeight = 0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (CurrentPlayerCard.Cases[x, y] != null)
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
                CloneSelectedCard.DrawingCaseValue = new int[realWidth, realHeight];
                CloneSelectedCard.Cases = new PlayerCase[realWidth, realHeight];

                CloneSelectedCard.Center.X = CurrentPlayerCard.Center.X;
                CloneSelectedCard.Center.Y = CurrentPlayerCard.Center.Y;

                for (int x = 0; x < realWidth; x++)
                {
                    for (int y = 0; y < realHeight; y++)
                    {
                        //cloneSelectedCard.DrawingCaseValue[x, y] = currentPlayerCard.DrawingCaseValue[x, y];
                        CloneSelectedCard.Cases[x, y] = CurrentPlayerCard.Cases[x, y];
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
                CloneSelectedCard.DrawingCaseValue = new int[realHeight, realWidth];
                CloneSelectedCard.Cases = new PlayerCase[realHeight, realWidth];

                CloneSelectedCard.Center.X = realHeight - CurrentPlayerCard.Center.Y - 1;
                CloneSelectedCard.Center.Y = CurrentPlayerCard.Center.X;

                for (int x = 0; x < realHeight; x++)
                {
                    for (int y = 0; y < realWidth; y++)
                    {
                        //cloneSelectedCard.DrawingCaseValue[x, y] = currentPlayerCard.DrawingCaseValue[y, x];
                        CloneSelectedCard.Cases[x, y] = CurrentPlayerCard.Cases[y, realHeight - 1 - x];
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
                CloneSelectedCard.DrawingCaseValue = new int[realWidth, realHeight];
                CloneSelectedCard.Cases = new PlayerCase[realWidth, realHeight];

                CloneSelectedCard.Center.X = realWidth - CurrentPlayerCard.Center.X - 1;
                CloneSelectedCard.Center.Y = realHeight - CurrentPlayerCard.Center.Y - 1;

                for (int x = 0; x < realWidth; x++)
                {
                    for (int y = 0; y < realHeight; y++)
                    {
                        //cloneSelectedCard.DrawingCaseValue[x, y] = currentPlayerCard.DrawingCaseValue[realWidth - x - 1, realHeight - y - 1];
                        CloneSelectedCard.Cases[x, y] = CurrentPlayerCard.Cases[realWidth - x - 1, realHeight - y - 1];
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
                CloneSelectedCard.DrawingCaseValue = new int[realHeight, realWidth];
                CloneSelectedCard.Cases = new PlayerCase[realHeight, realWidth];

                CloneSelectedCard.Center.X = CurrentPlayerCard.Center.Y;
                CloneSelectedCard.Center.Y = realWidth - CurrentPlayerCard.Center.X - 1;

                for (int x = 0; x < realHeight; x++)
                {
                    for (int y = 0; y < realWidth; y++)
                    {
                        //cloneSelectedCard.DrawingCaseValue[x, y] = currentPlayerCard.DrawingCaseValue[realWidth - y - 1, realHeight - x - 1];
                        CloneSelectedCard.Cases[x, y] = CurrentPlayerCard.Cases[realWidth - y - 1, x];
                    }
                }
            }

            //--- Calcul les valeurs de cases (2,4,8,16, etc...)
            width = CloneSelectedCard.DrawingCaseValue.GetUpperBound(0) + 1;
            height = CloneSelectedCard.DrawingCaseValue.GetUpperBound(1) + 1;

            int[,] drawingCaseValue = new int[width, height];
            CalcDrawingCaseValue(CloneSelectedCard.Cases, ref drawingCaseValue);
            CloneSelectedCard.DrawingCaseValue = drawingCaseValue;
            //---
        }

        public BaseCase GetCaseMapFromRotatedCard(int rotatedX, int rotatedY, int x, int y, int cardWidth, int cardHeight)
        {
            //---
            int realWidth = Map.Width - 1;
            int realHeight = Map.Height - 1;
            //---

            BaseCase caseMap = null;

            //--- Angle 0
            //  1 _ 2
            //  3|_|4
            //---
            if (cycleRotationCard == 0)
            {
                caseMap = Map.Cases[rotatedX + x, rotatedY + y];
            }

            //--- Angle Pi/2
            //  3 _ 1
            //  4|_|2
            //---
            if (cycleRotationCard == 1)
            {
                caseMap = Map.Cases[rotatedX + cardWidth - y - 1, rotatedY + x];
            }

            //--- Angle Pi
            //  4 _ 3
            //  2|_|1
            //---
            if (cycleRotationCard == 2)
            {
                caseMap = Map.Cases[rotatedX + cardWidth - x - 1, rotatedY + cardHeight - y - 1];
            }

            //--- Angle 3 Pi/2
            //  2 _ 4
            //  1|_|3
            //---
            if (cycleRotationCard == 3)
            {
                caseMap = Map.Cases[rotatedX + y, rotatedY + cardHeight - x - 1];
            }

            return caseMap;
        }

        private void CalcCardPositionOverMap(MouseState mouseState, bool withAnimation)
        {
            CalcCardPositionOverMap(mouseState, withAnimation, false);
        }

        private void CalcCardPositionOverMap(MouseState mouseState, bool withAnimation, bool forcePosition)
        {
            Point position = Point.Zero;

            //--- Dimension de la carte
            int width = CloneSelectedCard.DrawingCaseValue.GetUpperBound(0) + 1;
            int height = CloneSelectedCard.DrawingCaseValue.GetUpperBound(1) + 1;
            //---

            //--- Case située au centre de la carte
            int cx = CloneSelectedCard.Center.X;
            int cy = CloneSelectedCard.Center.Y;
            //---

            //--- Marge sur les côtés par rapport au centre de la carte
            int dxL = cx;
            int dxR = width - cx - 1;
            int dyL = cy;
            int dyR = height - cy - 1;
            //---

            //--- Calcul de la case sélectionnée sur la map par le curseur
            position.X = (mouseState.X - (int)PosMap.X) / CaseSize;
            position.Y = (mouseState.Y - (int)PosMap.Y) / CaseSize;
            //---

            if (position.X - dxL < 0)
                position.X = 0;
            else if (position.X + dxR >= Map.Width)
                position.X = Map.Width - width;
            else
                position.X -= cx;

            if (position.Y - dyL < 0)
                position.Y = 0;
            else if (position.Y + dyR >= Map.Height)
                position.Y = Map.Height - height;
            else
                position.Y -= cy;

            //---
            //int oldWidth = oldCloneSelectedCard.DrawingCaseValue.GetUpperBound(0) + 1;
            //int oldHeight = oldCloneSelectedCard.DrawingCaseValue.GetUpperBound(1) + 1;

            //Vector2 vecDeltascaleCardOverMap = new Vector2((oldWidth-width) * caseSize, (oldHeight-height) * caseSize) * (scaleMap) / 2f;
            //---

            //---> Déclenchement de l'animation de translation
            if (forcePosition ||
                position.X != PosSelectedCardOnMap.X ||
                position.Y != PosSelectedCardOnMap.Y)
            {
                if (withAnimation)
                {
                    Vector2 deltaCenterRotation = Vector2.Zero;
                    deltaCenterRotation.X = (oldCloneSelectedCard.Center.X - CloneSelectedCard.Center.X);
                    deltaCenterRotation.Y = (oldCloneSelectedCard.Center.Y - CloneSelectedCard.Center.Y);

                    VecPosSelectedCardOnMap += (deltaCenterRotation * CaseSize * ScaleMap);
                    //if (forcePosition)
                    //    vecPosSelectedCardOnMap -= vecDeltascaleCardOverMap;

                    lerpCardTranslationX = new Lerp(VecPosSelectedCardOnMap.X, position.X * CaseSize * ScaleMap, false, false, durationCardAnimation, -1);
                    lerpCardTranslationY = new Lerp(VecPosSelectedCardOnMap.Y, position.Y * CaseSize * ScaleMap, false, false, durationCardAnimation, -1);
                }
                else
                {
                    VecPosSelectedCardOnMap.X = position.X * CaseSize * ScaleMap;
                    VecPosSelectedCardOnMap.Y = position.Y * CaseSize * ScaleMap;
                }
            }

            PosSelectedCardOnMap = position;
        }

        public Boolean AreCasesEqual(BaseCase initialCase, BaseCase[,] cases, int offsetX, int offsetY)
        {
            bool casesAreEqual = false;

            int width = cases.GetUpperBound(0) + 1;
            int height = cases.GetUpperBound(1) + 1;

            if (offsetX >= 0 && offsetX < width && offsetY >= 0 && offsetY < height)
            {
                BaseCase offsetCase = cases[offsetX, offsetY];

                if (initialCase is PlayerCase && offsetCase is PlayerCase)
                {
                    PlayerCase pInitialiseCase = (PlayerCase)initialCase;
                    PlayerCase pOffsetCase = (PlayerCase)offsetCase;

                    if (offsetCase != null &&
                        pInitialiseCase.Player ==pOffsetCase.Player &&
                        !(pOffsetCase.NewCase && !pOffsetCase.GrowingCase)
                        )
                        //((PlayerCase)initialCase).FlowerType == ((PlayerCase)offsetCase).FlowerType)
                        casesAreEqual = true;
                }
            }

            return casesAreEqual;
        }

        private void PutDownSelectedCard()
        {
            //--- Dimension de la carte
            //int width = cloneSelectedCard.DrawingCaseValue.GetUpperBound(0) + 1;
            //int height = cloneSelectedCard.DrawingCaseValue.GetUpperBound(1) + 1;
            //---

            //--- Change le contexte
            ContextType = ContextType.PutDownCard;
            //---

            ScaleCardPuttingDown = ScaleCardOverMap;
            lerpScaleCardPuttingDown = new Lerp(ScaleCardOverMap, ScaleMap, false, false, durationCardPuttingDown, -1);

            //---> Initialise la case courante à -1
            //currentCasePuttingDown = -1;
        }

        private void CalcMapAfterCardPuttingDown()
        {
            //--- Dimension de la carte
            int width = CloneSelectedCard.DrawingCaseValue.GetUpperBound(0) + 1;
            int height = CloneSelectedCard.DrawingCaseValue.GetUpperBound(1) + 1;
            //---

            //--- Position de la première case de la carte à faire grossir
            //Point firstCaseCard = new Point(-1, -1);
            //---

            //--- Calcul de la position de la case
            //int y = (currentCasePuttingDown) / width;
            //int x = (currentCasePuttingDown) - y * width;
            //---

            PlayerCase caseMapPlayer = null;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //--- Détermine les cases de la map et de la carte en cours de pose
                    BaseCase caseMap = Map.Cases[x + PosSelectedCardOnMap.X, y + PosSelectedCardOnMap.Y];
                    PlayerCase caseCard = CloneSelectedCard.Cases[x, y];
                    //---

                    float finalCardValue = 0f;
                    int finalPlayer = 0;
                    float startGrowingValue = 0f;

                    FlowerType finalFlowerType = FlowerType.None;

                    if (caseCard != null)
                    {
                        if (caseMap is PlayerCase)
                        {
                            caseMapPlayer = (PlayerCase)caseMap;

                            int caseValue = CloneSelectedCard.DrawingCaseValue[x, y];

                            //---> Conflit
                            if (caseCard != null && caseMap != null && caseMapPlayer.FlowerType != FlowerType.None && caseMapPlayer.Player != CurrentPlayer)
                            {
                                float defenserValue = GetDefenserValue(caseMapPlayer, caseCard);
                                float strikerValue = GetStrikerValue(caseMapPlayer, caseCard);
                                float ratioSurviver = 1f;

                                //---> L'attaquant gagne
                                if (defenserValue < strikerValue)
                                {
                                    ratioSurviver = (strikerValue - defenserValue) / strikerValue;

                                    finalCardValue = caseCard.Defenser * ratioSurviver;
                                    finalPlayer = caseCard.Player;
                                    finalFlowerType = caseCard.FlowerType;
                                }
                                //---> Le défenseur gagne
                                else if (defenserValue > strikerValue)
                                {
                                    finalCardValue = defenserValue - strikerValue;
                                    finalPlayer = caseMapPlayer.Player;
                                    finalFlowerType = caseMapPlayer.FlowerType;
                                }

                                startGrowingValue = finalCardValue;
                            }
                            //---> Renfort
                            else if (caseCard != null && caseMap != null && caseMapPlayer.FlowerType != FlowerType.None && caseMapPlayer.Player == CurrentPlayer)
                            {
                                startGrowingValue = caseMapPlayer.Defenser;

                                finalCardValue = caseMapPlayer.Defenser + caseCard.Defenser;
                                finalPlayer = caseMapPlayer.Player;
                                finalFlowerType = caseMapPlayer.FlowerType;
                            }
                        }
                        //---> Nouvelle installation
                        else
                        {
                            finalCardValue = caseCard.Defenser;
                            finalPlayer = caseCard.Player;
                            finalFlowerType = caseCard.FlowerType;

                            //---
                            caseMapPlayer = new PlayerCase();

                            caseMapPlayer.BonusDefenser = caseCard.BonusDefenser;
                            caseMapPlayer.BonusStricker = caseCard.BonusStricker;
                            caseMapPlayer.Defenser = caseCard.Defenser;
                            caseMapPlayer.FlowerType = CloneSelectedCard.FlowerType;
                            caseMapPlayer.MalusDefenser = caseCard.MalusDefenser;
                            caseMapPlayer.MalusStricker = caseCard.MalusStricker;
                            caseMapPlayer.NumberFlower = caseCard.NumberFlower;
                            caseMapPlayer.NumberFlowerAdjacent = caseCard.NumberFlowerAdjacent;
                            caseMapPlayer.Player = CurrentPlayer;
                            caseMapPlayer.Stricker = caseCard.Stricker;
                            //---

                            startGrowingValue = 0f;
                        }

                        //--- Affecte les nouvelles valeurs de la carte
                        this.Map.Cases[x + PosSelectedCardOnMap.X, y + PosSelectedCardOnMap.Y] = caseMapPlayer;

                        caseMapPlayer.StartValueGrowingCase = startGrowingValue / 100f;

                        caseMapPlayer.TileId = caseMap.TileId;
                        caseMapPlayer.Defenser = finalCardValue;
                        caseMapPlayer.FlowerType = finalFlowerType;
                        caseMapPlayer.Player = finalPlayer;

                        caseMapPlayer.EndValueGrowingCase = caseMapPlayer.Defenser/100f;
                        caseMapPlayer.PercentageGrowingCase = 0f;
                        caseMapPlayer.NumberGrowingCase = -1;
                        caseMapPlayer.NewCase = true;
                        caseMapPlayer.GrowingStartTime = TimeSpan.Zero;

                        //TODO : mettrer les number flower
                        //--- 

                        //---
                        //if (firstCaseCard.X == -1 && firstCaseCard.Y == -1)
                        //{
                        //    firstCaseCard = new Point(x, y);
                        //}
                        //---
                    }
                }

                //--- Calcul la valeur de la bordure
                //int[,] drawingCaseValue = Map.DrawingCaseValue;
                //CalcDrawingCaseValue(Map.Cases, ref drawingCaseValue);
                //Map.DrawingCaseValue = drawingCaseValue;

                //---
            }

            //--- Calcul l'ordre de croissance des cases
            CalcMapGrowingCase(CloneSelectedCard.Player);
            //---
        }

        private void CalcMapGrowingCase(int player)
        {
            //--- Position de la base du joueur
            Point basePlayer = ListPlayerBasePosition[player - 1];
            //---

            //--- Réinitialise l'ordre de croissance du joueur
            List<PlayerCase> listPlayerCase = Map.Cases.OfType<PlayerCase>().Where(c => c.Player == player).ToList();

            foreach (PlayerCase playerCase in listPlayerCase)
            {
                playerCase.NumberGrowingCase = -1;
            }

            //---> La base du joueur a une valeur de croissance de 0
            ((PlayerCase)Map.Cases[basePlayer.X, basePlayer.Y]).NumberGrowingCase = 0;
            //---

            //--- Calcul de l'ordre de croissance des cases du joueur
            CalcNeighbourGrowingCaseOrder(player, basePlayer);
            //---

            //--- Initialise la croissance des cases
            curNumberCaseGrowing = -1;
            //---

            //--- Calcul de la prochaine case qui va croître
            CalcNextCaseGrowing();
            //---

            //--- Change l'état du contexte
            ContextType = ContextType.GrowingCase;
            //---
        }

        private void CalcNeighbourGrowingCaseOrder(int player, Point parentPoint)
        {
            //--- Récupère la valeur de la case courante
            int parentNumberGrowingCase = ((PlayerCase)Map.Cases[parentPoint.X, parentPoint.Y]).NumberGrowingCase;
            //---

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if ((x == 0 || y == 0) &&
                        parentPoint.X + x >= 0 && parentPoint.X + x < Map.Width &&
                        parentPoint.Y + y >= 0 && parentPoint.Y + y < Map.Height)
                    {
                        Point curPoint = new Point(parentPoint.X + x, parentPoint.Y + y);

                        if (Map.Cases[curPoint.X, curPoint.Y] is PlayerCase &&
                            ((PlayerCase)Map.Cases[curPoint.X, curPoint.Y]).Player == player &&
                            ((PlayerCase)Map.Cases[curPoint.X, curPoint.Y]).NumberGrowingCase == -1)
                        {
                            ((PlayerCase)Map.Cases[curPoint.X, curPoint.Y]).NumberGrowingCase = parentNumberGrowingCase + 1;
                        }
                    }
                }
            }

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if ((x == 0 || y == 0) &&
                        parentPoint.X + x >= 0 && parentPoint.X + x < Map.Width &&
                        parentPoint.Y + y >= 0 && parentPoint.Y + y < Map.Height)
                    {
                        Point curPoint = new Point(parentPoint.X + x, parentPoint.Y + y);

                        if (Map.Cases[curPoint.X, curPoint.Y] is PlayerCase &&
                            ((PlayerCase)Map.Cases[curPoint.X, curPoint.Y]).Player == player &&
                            ((PlayerCase)Map.Cases[curPoint.X, curPoint.Y]).NumberGrowingCase == parentNumberGrowingCase + 1)
                        {
                            //---> incrémente les valeurs de toutes les cases voisines
                            CalcNeighbourGrowingCaseOrder(player, curPoint);
                        }
                    }
                }
            }
        }

        private void CalcNextCaseGrowing()
        {
            int minimumDrawingCaseNumber = int.MaxValue;

            for (int x = 0; x < Map.Width; x++)
            {
                for (int y = 0; y < Map.Height; y++)
                {
                    if (Map.Cases[x, y] is PlayerCase &&
                        ((PlayerCase)Map.Cases[x, y]).NewCase &&
                        ((PlayerCase)Map.Cases[x, y]).NumberGrowingCase < minimumDrawingCaseNumber)
                    {
                        minimumDrawingCaseNumber = ((PlayerCase)Map.Cases[x, y]).NumberGrowingCase;
                    }
                }
            }

            for (int x = 0; x < Map.Width; x++)
            {
                for (int y = 0; y < Map.Height; y++)
                {
                    if (Map.Cases[x, y] is PlayerCase &&
                        ((PlayerCase)Map.Cases[x, y]).NewCase &&
                        ((PlayerCase)Map.Cases[x, y]).NumberGrowingCase == minimumDrawingCaseNumber)
                    {
                        ((PlayerCase)Map.Cases[x, y]).GrowingCase = true;
                    }

                    //((PlayerCase)Map.Cases[x, y]).NewCase = false;
                }
            }
        }

        private void NextPlayerToPlay()
        {
            ContextType = ContextType.None;

            if (CurrentPlayer == 0)
                CurrentPlayer = 1;
            else if (CurrentPlayer < ListAllPlayerColor.Count)
                CurrentPlayer++;
            else
                CurrentPlayer = 1;
        }

        public float GetDefenserValue(PlayerCase caseDefenser, PlayerCase caseStriker)
        {
            float defenserValue = 0f;
            int defenserCoeff = battleCoeff[(int)caseDefenser.FlowerType, (int)caseStriker.FlowerType];

            if (defenserCoeff == -1)
                defenserValue = caseDefenser.Defenser * caseDefenser.MalusDefenser;
            if (defenserCoeff == 0)
                defenserValue = caseDefenser.Defenser;
            if (defenserCoeff == 1)
                defenserValue = caseDefenser.Defenser * caseDefenser.BonusDefenser;

            return defenserValue;
        }

        public float GetStrikerValue(PlayerCase caseDefenser, PlayerCase caseStriker)
        {
            float strikerValue = 0f;
            int defenserCoeff = battleCoeff[(int)caseDefenser.FlowerType, (int)caseStriker.FlowerType];

            if (defenserCoeff == -1)
                strikerValue = caseStriker.Defenser * caseStriker.BonusDefenser;
            if (defenserCoeff == 0)
                strikerValue = caseStriker.Defenser;
            if (defenserCoeff == 1)
                strikerValue = caseStriker.Defenser * caseStriker.MalusDefenser;

            return strikerValue;
        }

        #region Events
        void clickableCardZone_ClickZone(ClickableZone zone, MouseState mouseState, GameTime gameTime)
        {
            //---> La carte peut être sélectionnée
            PlayerCard playerCard = (PlayerCard)zone.Tag;
            if (
                    (ContextType == ContextType.None && playerCard.Player == CurrentPlayer) ||
                    (ContextType == ContextType.CardSelected && playerCard.Player == CurrentPlayer && CurrentPlayerCard != playerCard)
                )
            {
                CurrentPlayerCard = playerCard;
                oldCloneSelectedCard = CurrentPlayerCard.Clone();

                ContextType = ContextType.CardSelected;
                InitCardValues(CurrentPlayerCard, CardType.Defenser);

                CalcTabRotatedCard();

                return;
            }

            //---> Déselectionnne la carte si elle est de nouveau cliquée
            if (ContextType == ContextType.CardSelected && playerCard.Player == CurrentPlayer && CurrentPlayerCard == playerCard)
            {
                CurrentPlayerCard = null;
                ContextType = ContextType.None;
                return;
            }
        }

        void mapZone_MouseEnter(ClickableZone zone, MouseState mouseState, GameTime gameTime)
        {
            bool triggerAnimation = true;

            //---> Si le pointeur est au dessus de la Map alors le context change
            if (ContextType == ContextType.CardSelected)
            {
                ContextType = ContextType.CardOverMap;
                triggerAnimation = false;
            }

            //---> Si le pointeur est au dessus de la Map et la carte est en train de tourner
            //	   alors le context change et la rotation prend fin
            if (ContextType == ContextType.CardRotated)
            {
                ContextType = ContextType.CardRotatedOverMap;
                triggerAnimation = false;
            }

            //---> Si la carte est au dessus de la map
            //	   calcul de la position de la carte sur la map
            if (ContextType == ContextType.CardOverMap ||
                ContextType == ContextType.CardRotatedOverMap)
            {
                //oldCenterRotated = cloneSelectedCard.Center;
                oldCloneSelectedCard = CloneSelectedCard.Clone();

                CalcCardPositionOverMap(mouseState, triggerAnimation);
            }
        }

        void mapZone_MouseLeave(ClickableZone zone, MouseState mouseState, GameTime gameTime)
        {
            //---> Le joueur a la carte en main et quitte la map
            if (ContextType == ContextType.CardOverMap)
                ContextType = ContextType.CardSelected;

            if (ContextType == ContextType.CardRotatedOverMap)
                ContextType = ContextType.CardRotated;
        }

        void mapZone_Clicked(ClickableZone zone, MouseState mouseState, GameTime gameTime)
        {
            //---> Le joueur pose la carte sur la map
            if (ContextType == ContextType.CardOverMap)
            {
                PutDownSelectedCard();
            }
        }
        #endregion

        public override void Draw(GameTime gameTime)
        {
            renderLogic.Draw(gameTime);

            //---
            SpriteBatch.Begin();

            base.Draw(gameTime);

            SpriteBatch.End();
            //---
        }
    }
}
