using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using NewFlowar.Tools;
using Microsoft.Xna.Framework.Graphics;
using NewFlowar.Model.Enum;

namespace NewFlowar.Model
{
    public class Contexte
    {
        #region Fields
        public int NumberCardsPerPlayer { get; set; }

        /// <summary>
        /// Joueur actif
        /// </summary>
        public Player CurrentPlayer = null;

        /// <summary>
        /// Carte sélectionnée par le joueur actif
        /// </summary>
        public PlayerCard CurrentPlayerCard = null;

        /// <summary>
        /// Rotation de la carte sélectionnée
        /// </summary>
        //public float CurrentCardRotation;

        /// <summary>
        /// Clone de la carte sélectionnée (pour les rotations)
        /// </summary>
        //public PlayerCard CloneSelectedCard = null;

        /// <summary>
        /// Cycle de rotation de la carte
        /// </summary>
        //private int cycleRotationCard;

        /// <summary>
        /// Case de la carte sélectionnée en cours de pose sur la map
        /// </summary>
        //private int currentCasePuttingDown = -1;

        /// <summary>
        /// Position de la carte sélectionnée sur la map
        /// </summary>
        //public Point PosSelectedCardOnMap;

        /// <summary>
        /// précédente valeur de la  roulette, pour tourner la carte
        /// </summary>
        //private int prevMouseWheel = 0;

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
        //public Dictionary<int, List<PlayerCard>> ListAllPlayerCard { get; set; }

        /// <summary>
        /// Liste des couleurs pour les joueurs (détermine également le nombre de joueurs)
        /// </summary>
        public Dictionary<int, Color> ListAllPlayerColor { get; set; }

        public int[,] battleCoeff;


        ///// <summary>
        ///// Largeur d'une carte standard
        ///// </summary>
        //public int WidthPlayerCard = 2;

        ///// <summary>
        ///// Hauteur d'une carte standard
        ///// </summary>
        //public int HeightPlayerCard = 4;



        /// <summary>
        /// Générateur de nombres aléatoires
        /// </summary>
        Random rnd = new Random();

        //public Vector2 VecPosSelectedCardOnMap = Vector2.Zero;


        /// <summary>
        /// Carte clonée avant animation
        /// </summary>
        //PlayerCard oldCloneSelectedCard;

        //public bool DoScreenShot = false;

        //public Dictionary<int, MapTile> ListMapTile { get; set; }

        //private const int TILE_ID_GRASS = 1000;

        //public List<Point> ListPlayerBasePosition { get; set; }

        //private int mapWidth;
        //private int mapHeight;

        //private int curNumberCaseGrowing;
        #endregion

        #region Rendering
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
        #endregion

        #region Animation

        public int durationCardAnimation = 100;
        public int durationCardPuttingDown = 175;

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
        #endregion

        public Contexte()
        {
            Init();
        }

        private void Init()
        {
            this.NumberCardsPerPlayer = 5;
        }
    }
}
