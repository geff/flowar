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
        #region Fields
        private SpriteFont fontCase;

        private Texture2D tex2DGround;

        /// <summary>
        /// Joueur actif
        /// </summary>
        private int currentPlayer = 0;

        /// <summary>
        /// Carte sélectionnée par le joueur actif
        /// </summary>
        private PlayerCard currentPlayerCard = null;

        /// <summary>
        /// Rotation de la carte sélectionnée
        /// </summary>
        private float currentCardRotation;

        /// <summary>
        /// Clone de la carte sélectionnée (pour les rotations)
        /// </summary>
        private PlayerCard cloneSelectedCard = null;

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
        private Point posSelectedCardOnMap;

        /// <summary>
        /// précédente valeur de la  roulette, pour tourner la carte
        /// </summary>
        private int prevMouseWheel = 0;

        /// <summary>
        /// Contexte utilisateur
        /// </summary>
        private ContextType contextType = ContextType.None;

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
        private Vector2 posMap = new Vector2(50, 120);

        /// <summary>
        /// Taille en pixel d'une case
        /// </summary>
        int caseSize = 64;

        /// <summary>
        /// Marge entrer la map et les cartes
        /// </summary>
        int marge = 10;

        /// <summary>
        /// Marge entre les cartes
        /// </summary>
        int smallMarge = 10;

        /// <summary>
        /// Largeur d'une carte standard
        /// </summary>
        int widthPlayerCard = 2;

        /// <summary>
        /// Hauteur d'une carte standard
        /// </summary>
        int heightPlayerCard = 4;

        /// <summary>
        /// Echelle des cartes
        /// </summary>
        float scale = 0.5f;

        /// <summary>
        /// Echelle de la carte sélectionnée (animation)
        /// </summary>
        float scaleSelectedCard = 0.5f;

        /// <summary>
        /// Echelle de la carte lorsque'elle est posée sur la map
        /// </summary>
        float scaleCardPuttingDown = 0f;

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

        Vector2 vecPosSelectedCardOnMap = Vector2.Zero;

        int durationCardAnimation = 100;
        int durationCardPuttingDown = 175;

        float scaleMap = 1f;
        float scaleCardOverMap = 1.15f;

        /// <summary>
        /// Carte clonée avant animation
        /// </summary>
        PlayerCard oldCloneSelectedCard;

        public Dictionary<int, MapTile> ListMapTile { get; set; }
        #endregion

        #region Constructeur / Init
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

            //--- chargement des polices de caractères
            fontCase = ContentManager.Load<SpriteFont>(@"Content\Font\FontCase1");
            //---

            //--- Chargement des images de la carte
            ListMapTile = new Dictionary<int, MapTile>();

            ListMapTile.Add(1000, new MapTile(1000, "Herbe", 0f, new Vector2(0f, -28f)));
            ListMapTile.Add(1260, new MapTile(1260, "Eau_H", 0f, Vector2.Zero));
            ListMapTile.Add(1080, new MapTile(1080, "Eau_H", MathHelper.PiOver2, Vector2.Zero));
            ListMapTile.Add(1320, new MapTile(1320, "Eau_C", 0f, new Vector2(-16, -16f)));
            ListMapTile.Add(1272, new MapTile(1272, "Eau_C", MathHelper.PiOver2, new Vector2(16f, -16f)));
            ListMapTile.Add(1020, new MapTile(1020, "Eau_C", MathHelper.Pi, new Vector2(-16, -16f)));
            ListMapTile.Add(1680, new MapTile(1680, "Eau_C", -MathHelper.PiOver2, new Vector2(-16, 16f)));
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
            if (contextType == ContextType.CardSelected ||
                contextType == ContextType.CardOverMap)
            {
                int sens = prevMouseWheel >= mouseState.ScrollWheelValue ? 1 : -1;

                lerpRotation = new Lerp(currentCardRotation, currentCardRotation + MathHelper.PiOver2 * sens, false, false, durationCardAnimation, -1);

                cycleRotationCard = (cycleRotationCard + sens) % 4;
                if (cycleRotationCard < 0)
                    cycleRotationCard += 4;

                if (contextType == ContextType.CardSelected)
                    contextType = ContextType.CardRotated;

                if (contextType == ContextType.CardOverMap)
                    contextType = ContextType.CardRotatedOverMap;

                //oldCenterRotated = cloneSelectedCard.Center;
                oldCloneSelectedCard = cloneSelectedCard.Clone();

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
        }
        #endregion

        public override void Update(GameTime gameTime)
        {

            //---> Grossissement de la carte lorsqu'elle est sélectionnée
            if (contextType == ContextType.CardRotated ||
                contextType == ContextType.CardSelected)
            {
                scaleSelectedCard = lerpScale.Eval(gameTime);
            }

            //---> Animation de rotation de la carte
            if (contextType == ContextType.CardRotated)
            {
                currentCardRotation = lerpRotation.Eval(gameTime);

                if (lerpRotation.IsFinished(gameTime))
                    contextType = ContextType.CardSelected;
            }

            //---> Animation de rotation de la carte sur la map
            if (contextType == ContextType.CardRotatedOverMap)
            {
                currentCardRotation = lerpRotation.Eval(gameTime);

                if (lerpRotation.IsFinished(gameTime))
                {
                    contextType = ContextType.CardOverMap;
                    //TODO : voir si cette ligne de code est utile par la suite
                    oldCloneSelectedCard = cloneSelectedCard;
                }
            }

            //---> Animation de translation sur la map
            if (lerpCardTranslationX != null)
            {
                vecPosSelectedCardOnMap.X = (int)lerpCardTranslationX.Eval(gameTime);
                vecPosSelectedCardOnMap.Y = (int)lerpCardTranslationY.Eval(gameTime);

                if (lerpCardTranslationX.IsFinished(gameTime) &&
                    lerpCardTranslationY.IsFinished(gameTime))
                {
                    lerpCardTranslationX = null;
                    lerpCardTranslationY = null;
                }
            }

            //---> Animation des cases lorsque la carte est posée sur la map
            if (contextType == ContextType.PutDownCard)
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
                scaleCardPuttingDown = lerpScaleCardPuttingDown.Eval(gameTime);

                if (lerpScaleCardPuttingDown.IsFinished(gameTime))
                {
                    CalcMapAfterCardPuttingDown();

                    NextPlayerToPlay();
                }
            }

            base.Update(gameTime);
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

        private void CreateMap()
        {
            int width = 10;
            int height = 8;

            this.Map = new Map(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //---> Recouvre le terrain d'herbe
                    this.Map.Cases[x, y] = new BaseCase(1000);
                }
            }

            //--- Calcul de la position de la rivière

            //---> Le début de la rivière est dans le deuxième cart de la carte
            int riverStart = rnd.Next((int)((float)this.Map.Width * 0.25f), this.Map.Width / 2 + 1);

            //---> La fin de la rivier est dans le troisième quart de la carte
            int riverEnd = rnd.Next(this.Map.Width / 2, (int)((float)this.Map.Width * 0.75f) + 1);

            int numberOfRiverCorner = riverEnd - riverStart;
            int riverX = riverStart;
            int[] riverYCorner = new int[numberOfRiverCorner];

            //---> Détermine les coins de la rivière
            for (int i = 0; i < numberOfRiverCorner; i++)
            {
                riverYCorner[i] = rnd.Next(this.Map.Width / numberOfRiverCorner * i, this.Map.Width / numberOfRiverCorner * (i + 1));
            }

            for (int y = 0; y < height; y++)
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

            //---

            //--- Zone de la map intéractive avec la souris
            ClickableZone mapZone = new ClickableZone(posMap, Map.Width * caseSize, Map.Height * caseSize);
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

        private void InitCardValues(PlayerCard playerCard, CardType cardType)
        {
            playerCard.CardType = cardType;

            float maxValue = 100f;
            float maxBonus = 1.25f;
            float maxMalus = 0.75f;

            float minValue = 50f;
            float minBonus = 1.25f;
            float minMalus = 0.75f;

            for (int x = 0; x < widthPlayerCard; x++)
            {
                for (int y = 0; y < heightPlayerCard; y++)
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
            cloneSelectedCard = currentPlayerCard.Clone();

            //Case[,] cloneSelectedCard.Cases = new Case[0,0];

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
                cloneSelectedCard.DrawingCaseValue = new int[realWidth, realHeight];
                cloneSelectedCard.Cases = new PlayerCase[realWidth, realHeight];

                cloneSelectedCard.Center.X = currentPlayerCard.Center.X;
                cloneSelectedCard.Center.Y = currentPlayerCard.Center.Y;

                for (int x = 0; x < realWidth; x++)
                {
                    for (int y = 0; y < realHeight; y++)
                    {
                        //cloneSelectedCard.DrawingCaseValue[x, y] = currentPlayerCard.DrawingCaseValue[x, y];
                        cloneSelectedCard.Cases[x, y] = currentPlayerCard.Cases[x, y];
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
                cloneSelectedCard.DrawingCaseValue = new int[realHeight, realWidth];
                cloneSelectedCard.Cases = new PlayerCase[realHeight, realWidth];

                cloneSelectedCard.Center.X = realHeight - currentPlayerCard.Center.Y - 1;
                cloneSelectedCard.Center.Y = currentPlayerCard.Center.X;

                for (int x = 0; x < realHeight; x++)
                {
                    for (int y = 0; y < realWidth; y++)
                    {
                        //cloneSelectedCard.DrawingCaseValue[x, y] = currentPlayerCard.DrawingCaseValue[y, x];
                        cloneSelectedCard.Cases[x, y] = currentPlayerCard.Cases[y, realHeight - 1 - x];
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
                cloneSelectedCard.DrawingCaseValue = new int[realWidth, realHeight];
                cloneSelectedCard.Cases = new PlayerCase[realWidth, realHeight];

                cloneSelectedCard.Center.X = realWidth - currentPlayerCard.Center.X - 1;
                cloneSelectedCard.Center.Y = realHeight - currentPlayerCard.Center.Y - 1;

                for (int x = 0; x < realWidth; x++)
                {
                    for (int y = 0; y < realHeight; y++)
                    {
                        //cloneSelectedCard.DrawingCaseValue[x, y] = currentPlayerCard.DrawingCaseValue[realWidth - x - 1, realHeight - y - 1];
                        cloneSelectedCard.Cases[x, y] = currentPlayerCard.Cases[realWidth - x - 1, realHeight - y - 1];
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
                cloneSelectedCard.DrawingCaseValue = new int[realHeight, realWidth];
                cloneSelectedCard.Cases = new PlayerCase[realHeight, realWidth];

                cloneSelectedCard.Center.X = currentPlayerCard.Center.Y;
                cloneSelectedCard.Center.Y = realWidth - currentPlayerCard.Center.X - 1;

                for (int x = 0; x < realHeight; x++)
                {
                    for (int y = 0; y < realWidth; y++)
                    {
                        //cloneSelectedCard.DrawingCaseValue[x, y] = currentPlayerCard.DrawingCaseValue[realWidth - y - 1, realHeight - x - 1];
                        cloneSelectedCard.Cases[x, y] = currentPlayerCard.Cases[realWidth - y - 1, x];
                    }
                }
            }

            //--- Calcul les valeurs de cases (2,4,8,16, etc...)
            width = cloneSelectedCard.DrawingCaseValue.GetUpperBound(0) + 1;
            height = cloneSelectedCard.DrawingCaseValue.GetUpperBound(1) + 1;

            int[,] drawingCaseValue = new int[width, height];
            CalcDrawingCaseValue(cloneSelectedCard.Cases, ref drawingCaseValue);
            cloneSelectedCard.DrawingCaseValue = drawingCaseValue;
            //---
        }

        private BaseCase GetCaseMapFromRotatedCard(int rotatedX, int rotatedY, int x, int y, int cardWidth, int cardHeight)
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
            int width = cloneSelectedCard.DrawingCaseValue.GetUpperBound(0) + 1;
            int height = cloneSelectedCard.DrawingCaseValue.GetUpperBound(1) + 1;
            //---

            //--- Case située au centre de la carte
            int cx = cloneSelectedCard.Center.X;
            int cy = cloneSelectedCard.Center.Y;
            //---

            //--- Marge sur les côtés par rapport au centre de la carte
            int dxL = cx;
            int dxR = width - cx - 1;
            int dyL = cy;
            int dyR = height - cy - 1;
            //---

            //--- Calcul de la case sélectionnée sur la map par le curseur
            position.X = (mouseState.X - (int)posMap.X) / caseSize;
            position.Y = (mouseState.Y - (int)posMap.Y) / caseSize;
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
                position.X != posSelectedCardOnMap.X ||
                position.Y != posSelectedCardOnMap.Y)
            {
                if (withAnimation)
                {
                    Vector2 deltaCenterRotation = Vector2.Zero;
                    deltaCenterRotation.X = (oldCloneSelectedCard.Center.X - cloneSelectedCard.Center.X);
                    deltaCenterRotation.Y = (oldCloneSelectedCard.Center.Y - cloneSelectedCard.Center.Y);

                    vecPosSelectedCardOnMap += (deltaCenterRotation * caseSize * scaleMap);
                    //if (forcePosition)
                    //    vecPosSelectedCardOnMap -= vecDeltascaleCardOverMap;

                    lerpCardTranslationX = new Lerp(vecPosSelectedCardOnMap.X, position.X * caseSize * scaleMap, false, false, durationCardAnimation, -1);
                    lerpCardTranslationY = new Lerp(vecPosSelectedCardOnMap.Y, position.Y * caseSize * scaleMap, false, false, durationCardAnimation, -1);
                }
                else
                {
                    vecPosSelectedCardOnMap.X = position.X * caseSize * scaleMap;
                    vecPosSelectedCardOnMap.Y = position.Y * caseSize * scaleMap;
                }
            }

            posSelectedCardOnMap = position;
        }

        private Boolean AreCasesEqual(BaseCase initialCase, BaseCase[,] cases, int offsetX, int offsetY)
        {
            bool casesAreEqual = false;

            int width = cases.GetUpperBound(0) + 1;
            int height = cases.GetUpperBound(1) + 1;

            if (offsetX >= 0 && offsetX < width && offsetY >= 0 && offsetY < height)
            {
                BaseCase offsetCase = cases[offsetX, offsetY];

                if (initialCase is PlayerCase && offsetCase is PlayerCase)
                {
                    if (offsetCase != null &&
                        ((PlayerCase)initialCase).Player == ((PlayerCase)offsetCase).Player &&
                        ((PlayerCase)initialCase).FlowerType == ((PlayerCase)offsetCase).FlowerType)
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
            contextType = ContextType.PutDownCard;
            //---

            scaleCardPuttingDown = scaleCardOverMap;
            lerpScaleCardPuttingDown = new Lerp(scaleCardOverMap, scaleMap, false, false, durationCardPuttingDown, -1);

            //---> Initialise la case courante à -1
            //currentCasePuttingDown = -1;
        }

        private void CalcMapAfterCardPuttingDown()
        {
            //--- Dimension de la carte
            int width = cloneSelectedCard.DrawingCaseValue.GetUpperBound(0) + 1;
            int height = cloneSelectedCard.DrawingCaseValue.GetUpperBound(1) + 1;
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
                    BaseCase caseMap = Map.Cases[x + posSelectedCardOnMap.X, y + posSelectedCardOnMap.Y];
                    PlayerCase caseCard = cloneSelectedCard.Cases[x, y];
                    //---

                    float finalCardValue = 0f;
                    int finalPlayer = 0;
                    FlowerType finalFlowerType = FlowerType.None;

                    if (caseCard != null)
                    {
                        if (caseMap is PlayerCase)
                        {
                            caseMapPlayer = (PlayerCase)caseMap;

                            int caseValue = cloneSelectedCard.DrawingCaseValue[x, y];

                            //---> Conflit
                            if (caseCard != null && caseMap != null && caseMapPlayer.FlowerType != FlowerType.None && caseMapPlayer.Player != currentPlayer)
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
                            }
                            //---> Renfort
                            else if (caseCard != null && caseMap != null && caseMapPlayer.FlowerType != FlowerType.None && caseMapPlayer.Player == currentPlayer)
                            {
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
                            caseMapPlayer.FlowerType = cloneSelectedCard.FlowerType;
                            caseMapPlayer.MalusDefenser = caseCard.MalusDefenser;
                            caseMapPlayer.MalusStricker = caseCard.MalusStricker;
                            caseMapPlayer.NumberFlower = caseCard.NumberFlower;
                            caseMapPlayer.NumberFlowerAdjacent = caseCard.NumberFlowerAdjacent;
                            caseMapPlayer.Player = currentPlayer;
                            caseMapPlayer.Stricker = caseCard.Stricker;
                            //---
                        }

                        //--- Affecte les nouvelles valeurs de la carte
                        this.Map.Cases[x + posSelectedCardOnMap.X, y + posSelectedCardOnMap.Y] = caseMapPlayer;

                        caseMapPlayer.TileId = caseMap.TileId;
                        caseMapPlayer.Defenser = finalCardValue;
                        caseMapPlayer.FlowerType = finalFlowerType;
                        caseMapPlayer.Player = finalPlayer;
                        //TODO : mettrer les number flower
                        //--- 
                    }
                }


                //--- Calcul la valeur de la bordure
                int[,] drawingCaseValue = Map.DrawingCaseValue;
                CalcDrawingCaseValue(Map.Cases, ref drawingCaseValue);
                Map.DrawingCaseValue = drawingCaseValue;


                //---
            }
        }

        private void NextPlayerToPlay()
        {
            contextType = ContextType.None;

            if (currentPlayer == 0)
                currentPlayer = 1;
            else if (currentPlayer < ListAllPlayerColor.Count)
                currentPlayer++;
            else
                currentPlayer = 1;
        }

        private float GetDefenserValue(PlayerCase caseDefenser, PlayerCase caseStriker)
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

        private float GetStrikerValue(PlayerCase caseDefenser, PlayerCase caseStriker)
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

        #region Drawing
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(20, 20, 20));

            //---
            DrawMap();

            if (contextType == ContextType.CardOverMap ||
                contextType == ContextType.CardRotatedOverMap)
                DrawSelectedCardOverMap();

            if (contextType == ContextType.PutDownCard)
                DrawCardPuttingDown();

            SpriteBatch.Begin();

            for (int i = 1; i <= ListAllPlayerColor.Keys.Count; i++)
            {
                DrawPlayerCards(i);
            }

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

        private void DrawMap()
        {
            Vector2 centerCase = new Vector2(caseSize / 2f);

            SpriteBatch.Begin();
            for (int x = 0; x < Map.Width; x++)
            {
                for (int y = 0; y < Map.Height; y++)
                {
                    //if (!(Map.Cases[x, y] != null && Map.Cases[x, y].Player > 0))
                    {
                        BaseCase baseCase = this.Map.Cases[x, y];

                        Vector2 posCard = new Vector2(x * caseSize * scaleMap, y * caseSize * scaleMap);
                        MapTile mapTile = ListMapTile[baseCase.TileId];
                        Texture2D texCase = ContentManager.Load<Texture2D>(String.Format(@"Content\Pic\{0}", mapTile.ContentName));

                        //--- Affiche le fond
                        SpriteBatch.Draw(
                            texCase,
                            posMap + posCard + new Vector2(caseSize) * (scaleMap + 0.1f) / 2f + mapTile.OffsetPosition,
                            null,
                            Color.White,
                            mapTile.Rotation, new Vector2(caseSize) * (scaleMap) / 2f, scaleMap
                            //* (scale + 0.1f) / 2f, scale + 0.1f
                            , SpriteEffects.None, 0f
                            );
                        //---

                        ////--- Affiche le fond
                        //SpriteBatch.Draw(
                        //    tex2DGround,
                        //    posMap + posCard,
                        //    null,
                        //    Color.SaddleBrown,
                        //    0f, Vector2.Zero, scale, SpriteEffects.None, 0f
                        //    );
                        ////---
                    }
                }
            }

            SpriteBatch.End();

            for (int x = 0; x < Map.Width; x++)
            {
                for (int y = 0; y < Map.Height; y++)
                {
                    if (Map.Cases[x, y] != null && Map.Cases[x, y] is PlayerCase)
                    {
                        PlayerCase playerCase = (PlayerCase)Map.Cases[x, y];
                        int caseValue = Map.DrawingCaseValue[x, y];

                        if (caseValue >= 0)
                        {
                            Vector2 posCard = posMap;
                            Vector2 posCase = new Vector2(x * caseSize * scaleMap, y * caseSize * scaleMap);

                            //--- Affiche la case
                            DrawCase(playerCase, caseValue, GetColorFlower(playerCase.FlowerType), ListAllPlayerColor[playerCase.Player], posCard, posCase, scaleMap);

                            //---> Affiche les valeurs
                            DrawCaseValuesMap(playerCase, posMap + posCard, posCase, scaleMap, 0f, centerCase);
                        }
                    }
                }
            }

        }

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
                                ListAllPlayerColor[numberPlayer],
                                //GetColorFlower(ListAllPlayerCard[numberPlayer][numCard].FlowerType),
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
                                GetColorFlower(ListAllPlayerCard[numberPlayer][numCard].FlowerType),
                                //ListAllPlayerColor[numberPlayer],
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
                        Vector2 posCard = new Vector2((float)(x + 0.5f) * (float)caseSize * scaleSelectedCard, (float)(y + 0.5f) * (float)caseSize * scaleSelectedCard);
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
                            ListAllPlayerColor[currentPlayerCard.Player],
                            //GetColorFlower(currentPlayerCard.FlowerType),
                            0, Vector2.Zero,
                            scaleSelectedCard, SpriteEffects.None, 0f
                            );
                        //---

                        //--- Affiche le contour de la case
                        Texture2D texCase = ContentManager.Load<Texture2D>(String.Format(@"Content\Pic\{0}", caseValue));

                        SpriteBatch.Draw(
                            texCase,
                            posCard,
                            null,
                            GetColorFlower(currentPlayerCard.FlowerType),
                            //ListAllPlayerColor[currentPlayerCard.Player],
                            0, Vector2.Zero,
                            scaleSelectedCard, SpriteEffects.None, 0f);
                        //---

                        SpriteBatch.End();
                    }
                }
            }
        }

        private void DrawSelectedCardOverMap()
        {
            currentPlayerCard.CalcDimensions();

            int width = currentPlayerCard.Width;
            int height = currentPlayerCard.Height;

            int realWidth = cloneSelectedCard.DrawingCaseValue.GetUpperBound(0) + 1;
            int realHeight = cloneSelectedCard.DrawingCaseValue.GetUpperBound(1) + 1;

            Vector2 vecDeltascaleCardOverMap = new Vector2(realWidth * caseSize, realHeight * caseSize) * (scaleCardOverMap - scaleMap) / 2f;

            Vector2 centerCard = new Vector2((currentPlayerCard.Center.X + 0.5f) * caseSize * scaleCardOverMap, (currentPlayerCard.Center.Y + 0.5f) * caseSize * scaleCardOverMap);
            Vector2 centerRotatedCard = new Vector2((cloneSelectedCard.Center.X + 0.5f) * caseSize * scaleCardOverMap, (cloneSelectedCard.Center.Y + 0.5f) * caseSize * scaleCardOverMap);

            Vector2 posCard = posMap + vecPosSelectedCardOnMap + centerRotatedCard - vecDeltascaleCardOverMap;

            this.Game.Window.Title = posCard.ToString();

            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    Vector2 posCase = new Vector2(x * caseSize * scaleCardOverMap, y * caseSize * scaleCardOverMap);

                    PlayerCase caseCard = currentPlayerCard.Cases[x, y];

                    BaseCase caseMap = GetCaseMapFromRotatedCard(posSelectedCardOnMap.X, posSelectedCardOnMap.Y, x, y, realWidth, realHeight);

                    if (caseCard != null)
                    {
                        int caseValue = currentPlayerCard.DrawingCaseValue[x, y];

                        if (contextType == ContextType.CardRotatedOverMap)
                        {
                            //--- Affiche la case
                            DrawCase(caseCard, caseValue, GetColorFlower(cloneSelectedCard.FlowerType), ListAllPlayerColor[currentPlayer], posCard, posCase, scaleCardOverMap, currentCardRotation, centerCard);

                            //---> Affiche les valeurs
                            DrawCaseValuesMap(caseCard, posCard, posCase, scaleCardOverMap, currentCardRotation, centerCard);
                        }
                        else if (caseMap is PlayerCase)
                        {
                            PlayerCase caseMapPlayer = (PlayerCase)caseMap;

                            //---> Conflit
                            if (caseCard != null && caseMap != null && caseMapPlayer.FlowerType != FlowerType.None && caseMapPlayer.Player != currentPlayer)
                            {
                                //--- Affiche la case
                                DrawCase(caseCard, caseValue, GetColorFlower(cloneSelectedCard.FlowerType), Color.Gray, posCard, posCase, scaleCardOverMap, currentCardRotation, centerCard);

                                //---> Affiche les valeurs
                                DrawCaseValuesConflict(caseMapPlayer, caseCard, posCase, scaleCardOverMap);
                            }
                            //---> Renfort
                            else if (caseCard != null && caseMap != null && caseMapPlayer.FlowerType != FlowerType.None && caseMapPlayer.Player == currentPlayer)
                            {
                                //--- Affiche la case
                                DrawCase(caseCard, caseValue, GetColorFlower(cloneSelectedCard.FlowerType), ListAllPlayerColor[currentPlayer], posCard, posCase, scaleCardOverMap, currentCardRotation, centerCard);

                                //---> Affiche les valeurs
                                DrawCaseValuesRenfort(caseMapPlayer, caseCard, posCase, scaleCardOverMap);
                            }
                        }
                        else
                        {

                            //---> Nouvelle installation
                            //else
                            {
                                //--- Affiche la case
                                DrawCase(caseCard, caseValue, GetColorFlower(cloneSelectedCard.FlowerType), ListAllPlayerColor[currentPlayer], posCard, posCase, scaleCardOverMap, currentCardRotation, centerCard);

                                //---> Affiche les valeurs
                                DrawCaseValuesMap(caseCard, posCard, posCase, scaleCardOverMap, currentCardRotation, centerCard);
                            }
                        }
                    }
                }
            }
        }

        private void DrawCardPuttingDown()
        {
            int width = currentPlayerCard.DrawingCaseValue.GetUpperBound(0) + 1;
            int height = currentPlayerCard.DrawingCaseValue.GetUpperBound(1) + 1;

            int realWidth = cloneSelectedCard.DrawingCaseValue.GetUpperBound(0) + 1;
            int realHeight = cloneSelectedCard.DrawingCaseValue.GetUpperBound(1) + 1;

            Vector2 vecDeltascaleCardPuttingDown = new Vector2(realWidth * caseSize, realHeight * caseSize) * (scaleCardPuttingDown - scaleMap) / 2f;

            Color colorBorder = ListAllPlayerColor[currentPlayer];
            Color colorBackground = GetColorFlower(currentPlayerCard.FlowerType);

            Vector2 centerCard = new Vector2((currentPlayerCard.Center.X + 0.5f) * caseSize * scaleCardPuttingDown, (currentPlayerCard.Center.Y + 0.5f) * caseSize * scaleCardPuttingDown);
            Vector2 centerRotatedCard = new Vector2((cloneSelectedCard.Center.X + 0.5f) * caseSize * scaleCardPuttingDown, (cloneSelectedCard.Center.Y + 0.5f) * caseSize * scaleCardPuttingDown);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2 posCard = posMap + vecPosSelectedCardOnMap + centerRotatedCard - vecDeltascaleCardPuttingDown;
                    Vector2 posCase = new Vector2(x * caseSize * scaleCardPuttingDown, y * caseSize * scaleCardPuttingDown);

                    PlayerCase caseCard = currentPlayerCard.Cases[x, y];

                    //Case caseMap = GetCaseMapFromRotatedCard(x + posSelectedCardOnMap.X, y + posSelectedCardOnMap.Y);

                    if (caseCard != null)
                    {
                        int caseValue = currentPlayerCard.DrawingCaseValue[x, y];

                        //--- Affiche la case
                        DrawCase(caseCard, caseValue, colorBackground, colorBorder, posCard, posCase, scaleCardPuttingDown, currentCardRotation, centerCard);

                        //---> Affiche les valeurs
                        DrawCaseValuesMap(caseCard, posCard, posCase, scaleCardPuttingDown, currentCardRotation, centerCard);
                    }
                }
            }
        }

        private void DrawCase(BaseCase caseToDraw, int caseValue, Color colorPlayer, Color colorFLowerType, Vector2 posCard, Vector2 posCase, float scale)
        {
            DrawCase(caseToDraw, caseValue, colorPlayer, colorFLowerType, posCard, posCase, scale, 0f, Vector2.Zero);
        }

        private void DrawCase(BaseCase caseToDraw, int caseValue, Color colorPlayer, Color colorFLowerType, Vector2 posCard, Vector2 posCase, float scale, float rotation, Vector2 center)
        {
            Matrix mtxTransform =
                Matrix.CreateTranslation(new Vector3(-center, 0f)) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateTranslation(new Vector3(posCard, 0f));

            SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState, mtxTransform);

            Vector2 centerCase = new Vector2(caseSize / 2f);

            //---> posCard = centre absolu de la carte (position de la première case + position du centre)
            //---> posCase = position relative de la carte

            //--- Affiche le fond
            SpriteBatch.Draw(
                tex2DGround,
                posCase,
                null,
                colorFLowerType,
                0f,
                Vector2.Zero,
                scale, SpriteEffects.None, 0f
                );
            //---

            //--- Affiche le contour de la case
            Texture2D texCase = ContentManager.Load<Texture2D>(String.Format(@"Content\Pic\{0}", caseValue));

            SpriteBatch.Draw(
                texCase,
                posCase,
                null,
                colorPlayer,
                0f,
                Vector2.Zero,
                scale, SpriteEffects.None, 0f);
            //---

            SpriteBatch.End();

        }

        private void DrawCaseValuesMap(PlayerCase caseCard, Vector2 posCard, Vector2 posCase, float scale, float rotation, Vector2 center)
        {
            int width = (int)((float)caseSize * scale);
            int height = (int)((float)caseSize * scale);

            Vector2 centerCase = new Vector2(width, height) / 2f;

            //rotation = DateTime.Now.Millisecond/1000f;

            Matrix mtxTransform =
                Matrix.CreateTranslation(new Vector3(-centerCase, 0f)) *
                Matrix.CreateRotationZ(-rotation) *
                Matrix.CreateTranslation(new Vector3(posCase - center + centerCase, 0f)) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateTranslation(new Vector3(posCard, 0f));

            SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState, mtxTransform);


            //Matrix mtxTransform2 =
            //    Matrix.CreateTranslation(new Vector3(-centerCase, 0f)) *
            //    Matrix.CreateRotationZ(-rotation);

            Vector2 vec1 = new Vector2((float)caseSize * scale * 0.15f, (float)caseSize * scale * 0.75f);
            Vector2 vec2 = new Vector2((float)caseSize * scale * 0.4f, (float)caseSize * scale * 0.75f);
            Vector2 vec3 = new Vector2((float)caseSize * scale * 0.75f, (float)caseSize * scale * 0.75f);


            //vec1 = Vector2.Transform(vec1, mtxTransform2);
            //vec2 = Vector2.Transform(vec2, Matrix.CreateRotationZ(-rotation))-center*0;
            //vec3 = Vector2.Transform(vec3, Matrix.CreateRotationZ(-rotation))-center*0;

            float r = 0f; ;
            //Vector2 c = centerCase;
            Vector2 c = Vector2.Zero; ;

            SpriteBatch.DrawString(fontCase, Math.Round((double)(caseCard.BonusDefenser * caseCard.Defenser), 0, MidpointRounding.AwayFromZero).ToString(), vec1, Color.LightGreen,
                r, c, Vector2.One, SpriteEffects.None, 0f);


            SpriteBatch.DrawString(fontCase, caseCard.Defenser.ToString(), vec2, Color.LightGreen,
                r, c, Vector2.One, SpriteEffects.None, 0f);


            SpriteBatch.DrawString(fontCase, Math.Round((double)(caseCard.MalusStricker * caseCard.Defenser), 0, MidpointRounding.AwayFromZero).ToString(), vec3, Color.LightGreen,
                r, c, Vector2.One, SpriteEffects.None, 0f);


            SpriteBatch.End();
        }

        private void DrawCaseValuesConflict(PlayerCase caseMap, PlayerCase caseCard, Vector2 pos, float scale)
        {
            int width = (int)((float)caseSize * scale);
            int height = (int)((float)caseSize * scale);

            float defenserValue = GetDefenserValue(caseMap, caseCard);
            float strikerValue = GetStrikerValue(caseMap, caseCard);

            SpriteBatch.Begin();

            //---> Défenseur
            SpriteBatch.DrawString(fontCase, Math.Round(defenserValue, 0, MidpointRounding.AwayFromZero).ToString(), pos + new Vector2((float)caseSize * scale * 0.15f, (float)caseSize * scale * 0.7f), Color.White);

            //---> Attaquant
            SpriteBatch.DrawString(fontCase, Math.Round(strikerValue, 0, MidpointRounding.AwayFromZero).ToString(), pos + new Vector2((float)caseSize * scale * 0.65f, (float)caseSize * scale * 0.7f), Color.White);

            SpriteBatch.End();
        }

        private void DrawCaseValuesRenfort(PlayerCase caseMap, PlayerCase caseCard, Vector2 pos, float scale)
        {
            int width = (int)((float)caseSize * scale);
            int height = (int)((float)caseSize * scale);

            SpriteBatch.Begin();

            SpriteBatch.DrawString(fontCase, Math.Round((double)(caseMap.BonusDefenser * (caseMap.Defenser + caseCard.Defenser)), 0, MidpointRounding.AwayFromZero).ToString(), pos + new Vector2((float)caseSize * scale * 0.15f, (float)caseSize * scale * 0.7f), Color.Black);
            SpriteBatch.DrawString(fontCase, (caseMap.Defenser + caseCard.Defenser).ToString(), pos + new Vector2((float)caseSize * scale * 0.4f, (float)caseSize * scale * 0.7f), Color.Black);
            SpriteBatch.DrawString(fontCase, Math.Round((double)(caseMap.MalusStricker * (caseMap.Defenser + caseCard.Defenser)), 0, MidpointRounding.AwayFromZero).ToString(), pos + new Vector2((float)caseSize * scale * 0.65f, (float)caseSize * scale * 0.7f), Color.Black);

            SpriteBatch.End();
        }

        private void DrawCaseValuesOnHand(PlayerCase caseCard)
        {
        }

        private Color GetColorFlower(FlowerType flowerType)
        {
            Color color = Color.White;

            switch (flowerType)
            {
                case FlowerType.None:
                    break;
                case FlowerType.Red:
                    color = Color.LightCoral;
                    break;
                case FlowerType.Green:
                    color = Color.LightGreen;
                    break;
                case FlowerType.Blue:
                    color = Color.LightSkyBlue;
                    break;
                default:
                    break;
            }

            return color;
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
                oldCloneSelectedCard = currentPlayerCard.Clone();

                contextType = ContextType.CardSelected;
                InitCardValues(currentPlayerCard, CardType.Defenser);

                CalcTabRotatedCard();

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
            bool triggerAnimation = true;

            //---> Si le pointeur est au dessus de la Map alors le context change
            if (contextType == ContextType.CardSelected)
            {
                contextType = ContextType.CardOverMap;
                triggerAnimation = false;
            }

            //---> Si le pointeur est au dessus de la Map et la carte est en train de tourner
            //	   alors le context change et la rotation prend fin
            if (contextType == ContextType.CardRotated)
            {
                contextType = ContextType.CardRotatedOverMap;
                triggerAnimation = false;
            }

            //---> Si la carte est au dessus de la map
            //	   calcul de la position de la carte sur la map
            if (contextType == ContextType.CardOverMap ||
                contextType == ContextType.CardRotatedOverMap)
            {
                //oldCenterRotated = cloneSelectedCard.Center;
                oldCloneSelectedCard = cloneSelectedCard.Clone();

                CalcCardPositionOverMap(mouseState, triggerAnimation);
            }
        }

        void mapZone_MouseLeave(ClickableZone zone, MouseState mouseState, GameTime gameTime)
        {
            //---> Le joueur a la carte en main et quitte la map
            if (contextType == ContextType.CardOverMap)
                contextType = ContextType.CardSelected;

            if (contextType == ContextType.CardRotatedOverMap)
                contextType = ContextType.CardRotated;
        }

        void mapZone_Clicked(ClickableZone zone, MouseState mouseState, GameTime gameTime)
        {
            //---> Le joueur pose la carte sur la map
            if (contextType == ContextType.CardOverMap)
            {
                PutDownSelectedCard();
            }
        }
        #endregion
    }
}
