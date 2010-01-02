using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Flowar.Model;
using Flowar.Model.Enum;
using Microsoft.Xna.Framework.Content;

namespace Flowar.Render
{
    public class RenderLogic
    {
        public RenderLogic(GameFlowar gameBase)
        {
            this.gameEngine = gameBase;

            Init();
        }

        GameFlowar gameEngine;

        private SpriteFont fontCase;
        private Texture2D tex2DGround;

        Texture2D sceneTex;
        Texture2D colorTex;
        Texture2D normalTex;
        //Texture2D playerTex;
        //Texture2D unitTex;

        RenderTarget2D sceneTarget;
        RenderTarget2D colorTarget;
        RenderTarget2D normalTarget;
        //RenderTarget2D playerTarget;
        //RenderTarget2D unitTarget;

        Texture2D gaussianTex;
        Texture2D gaussianUnitTex;

        public Texture2D texBlank;

        Effect blobEffect, blob2Effect, blobColorEffect;
        int GAUSSIANSIZE = 64;
        float GAUSSIANDEVIATION = 0.125f;

        VertexBlob[] vertexBlobArray;
        //VertexPositionColor[] vertexBlobArray;
        //VertexPositionColor[] vertexBlobArrayTest;

        public VertexDeclaration vertexPosColDecl;

        public Matrix View, Projection, ViewProjection;

        public Matrix ViewUI, ProjectionUI, ViewProjectionUI;

        TextureCube cubeTex;
        private QuadRenderComponent quad;


        public SpriteBatch SpriteBatch
        {
            get
            {
                return this.gameEngine.SpriteBatch;
            }
        }

        public ContentManager ContentManager
        {
            get
            {
                return this.gameEngine.ContentManager;
            }
        }

        private void Init()
        {
            //--- Chargement des textures
            tex2DGround = ContentManager.Load<Texture2D>(@"Content\Pic\Fond");
            //fleur0Tx = ContentManager.Load<Texture2D>(@"Content\Pic\Fleur1");
            //---

            //--- chargement des polices de caractères
            fontCase = ContentManager.Load<SpriteFont>(@"Content\Font\FontCase1");
            //---

            GenerateGaussianTexture(ref gaussianTex, GAUSSIANDEVIATION);
            GenerateGaussianTexture(ref gaussianUnitTex, GAUSSIANDEVIATION / 7f);

            //-------------------------------

            //--- Effects
            blobEffect = gameEngine.Game.Content.Load<Effect>(@"Content\Shader\Blobs2");
            blobColorEffect = gameEngine.Game.Content.Load<Effect>(@"Content\Shader\BlobFinalPass");
            //---

            quad = new QuadRenderComponent(this.gameEngine.Game);
            this.gameEngine.Game.Components.Add(quad);

            //--- Render target
            sceneTarget = CreateRenderTarget();
            colorTarget = CreateRenderTarget();
            normalTarget = CreateRenderTarget();
            //playerTarget = CreateRenderTarget();
            //unitTarget = CreateRenderTarget();
            //infoTarget = CreateRenderTarget();

            //UITarget = CreateRenderTarget();
            //---

            //spriteArray = new VertexBlob[20];
            cubeTex = gameEngine.Game.Content.Load<TextureCube>(@"Content\Pic\LobbyCube");

            vertexPosColDecl = new VertexDeclaration(gameEngine.Game.GraphicsDevice,
                VertexBlob.VertexElements);

            //vertexPosColDecl = new VertexDeclaration(gameEngine.Game.GraphicsDevice,
            //    VertexPositionColor.VertexElements);

            //--- Matrix
            //Projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f,
            //    (float)this.gameEngine.Game.GraphicsDevice.Viewport.Width / (float)this.gameEngine.Game.GraphicsDevice.Viewport.Height, 0.01f, 5000.0f);

            Projection = Matrix.CreateOrthographic((float)this.gameEngine.Game.GraphicsDevice.Viewport.Width, (float)this.gameEngine.Game.GraphicsDevice.Viewport.Height, 0.01f, 5000.0f);
            //Projection = Matrix.CreateOrthographic(100,100, 0.01f, 5000.0f);

            View = Matrix.CreateTranslation(
                -(float)this.gameEngine.Game.GraphicsDevice.Viewport.Width / 2 + 2 * gameEngine.PosMap.X - 20f,
                (float)this.gameEngine.Game.GraphicsDevice.Viewport.Height / 2 - gameEngine.PosMap.Y - 28f,
                -5f);
            ViewProjection = View * Projection;
            //---

            //this.vertexBlobArray = new VertexPositionColor[2];
            //this.vertexBlobArray = new VertexBlob[2];

            //this.vertexBlobArray[0].Color = Color.Orange;
            //this.vertexBlobArray[0].PlayerColor = Color.Yellow;

            //this.vertexBlobArray[0].Size = 1f;

            //this.vertexBlobArray[1].Color = Color.Blue;
            //this.vertexBlobArray[1].PlayerColor = Color.Yellow;
            //this.vertexBlobArray[1].Position.X = 1;// gameEngine.PosMap.X + gameEngine.CaseSize * 1.5f;
            //this.vertexBlobArray[1].Position.Y = 5;// gameEngine.PosMap.Y + gameEngine.CaseSize / 2;
            //this.vertexBlobArray[1].Size = 0.75f;

            //ProjectionUI = Projection;
            //ViewUI = Matrix.CreateTranslation(new Vector3(-vecTranslation.X, vecTranslation.Y, zoom*10));
            //ViewProjectionUI = ViewUI * ProjectionUI;
        }

        private RenderTarget2D CreateRenderTarget()
        {
            return new RenderTarget2D(gameEngine.Game.GraphicsDevice, this.gameEngine.Game.GraphicsDevice.Viewport.Width,
                this.gameEngine.Game.GraphicsDevice.Viewport.Height, 0, SurfaceFormat.HalfVector4);//, gameEngine.Game.GraphicsDevice.PresentationParameters.MultiSampleType, gameEngine.Game.GraphicsDevice.PresentationParameters.MultiSampleQuality);
        }

        #region Drawing
        public void Draw(GameTime gameTime)
        {
            gameEngine.GraphicsDevice.SetRenderTarget(0, sceneTarget);

            gameEngine.GraphicsDevice.Clear(new Color(20, 20, 20));

            //---
            DrawMap();

            if (gameEngine.ContextType == ContextType.CardOverMap ||
                gameEngine.ContextType == ContextType.CardRotatedOverMap)
                DrawSelectedCardOverMap();

            if (gameEngine.ContextType == ContextType.PutDownCard)
                DrawCardPuttingDown();

            SpriteBatch.Begin();

            for (int i = 1; i <= gameEngine.ListAllPlayerColor.Keys.Count; i++)
            {
                DrawPlayerCards(i);
            }

            SpriteBatch.End();
            //---

            //---
            if (
                    gameEngine.CurrentPlayerCard != null &&
                    (gameEngine.ContextType == ContextType.CardSelected || gameEngine.ContextType == ContextType.CardRotated)
                )
            {
                DrawSelectedCard();
            }
            //---

            gameEngine.Game.GraphicsDevice.SetRenderTarget(0, null);


            DrawBlob(gameTime);

            for (int y = 0; y < gameEngine.Map.Height; y++)
            {
                for (int x = 0; x < gameEngine.Map.Width; x++)
                {
                    //if (!(gameEngine.Map.Cases[x, y] != null && gameEngine.Map.Cases[x, y].Player > 0))
                    {
                        BaseCase baseCase = this.gameEngine.Map.Cases[x, y];

                        Vector2 posCard = new Vector2(x * gameEngine.CaseSize * gameEngine.ScaleMap, y * gameEngine.CaseSize * gameEngine.ScaleMap);

                        SpriteBatch.Begin();

                        if (baseCase is PlayerCase)
                        {
                            PlayerCase playerCase = (PlayerCase)baseCase;
                            int caseValue = gameEngine.Map.DrawingCaseValue[x, y];

                            if (caseValue >= 0)
                            {
                                //Vector2 posCard = gameEngine.posMap;
                                Vector2 posCase = new Vector2(x * gameEngine.CaseSize * gameEngine.ScaleMap, y * gameEngine.CaseSize * gameEngine.ScaleMap);

                                SpriteBatch.DrawString(fontCase, playerCase.NumberGrowingCase.ToString(), gameEngine.PosMap + posCase + new Vector2(gameEngine.CaseSize / 2), Color.Black);
                            }
                        }
                        SpriteBatch.End();
                    }
                }
            }
        }

        private void DrawMap()
        {
            Vector2 centerCase = new Vector2(gameEngine.CaseSize / 2f);

            for (int y = 0; y < gameEngine.Map.Height; y++)
            {
                for (int x = 0; x < gameEngine.Map.Width; x++)
                {
                    //if (!(gameEngine.Map.Cases[x, y] != null && gameEngine.Map.Cases[x, y].Player > 0))
                    {
                        BaseCase baseCase = this.gameEngine.Map.Cases[x, y];

                        Vector2 posCard = new Vector2(x * gameEngine.CaseSize * gameEngine.ScaleMap, y * gameEngine.CaseSize * gameEngine.ScaleMap);
                        MapTile mapTile = gameEngine.ListMapTile[baseCase.TileId];
                        Texture2D texCase = ContentManager.Load<Texture2D>(String.Format(@"Content\Pic\{0}", mapTile.ContentName));

                        SpriteBatch.Begin();

                        //--- Affiche le fond
                        SpriteBatch.Draw(
                            texCase,
                            gameEngine.PosMap + posCard + new Vector2(gameEngine.CaseSize) * (gameEngine.ScaleMap + 0.1f) / 2f + mapTile.OffsetPosition,
                            null,
                            Color.White,
                            mapTile.Rotation, new Vector2(gameEngine.CaseSize) * (gameEngine.ScaleMap) / 2f, gameEngine.ScaleMap
                            //* (gameEngine.scale + 0.1f) / 2f, gameEngine.scale + 0.1f
                            , SpriteEffects.None, 0f
                            );
                        //---

                        SpriteBatch.End();


                        //if (baseCase is PlayerCase)
                        //{
                        //    PlayerCase playerCase = (PlayerCase)baseCase;
                        //    int caseValue = gameEngine.Map.DrawingCaseValue[x, y];

                        //    if (caseValue >= 0)
                        //    {
                        //        //Vector2 posCard = gameEngine.posMap;
                        //        Vector2 posCase = new Vector2(x * gameEngine.CaseSize * gameEngine.ScaleMap, y * gameEngine.CaseSize * gameEngine.ScaleMap);

                        //        //--- Affiche la case
                        //        DrawCase(playerCase, caseValue, GetColorFlower(playerCase.FlowerType), gameEngine.ListAllPlayerColor[playerCase.Player], gameEngine.PosMap, posCase, gameEngine.ScaleMap);

                        //        //---> Affiche les valeurs
                        //        DrawCaseValuesMap(playerCase, gameEngine.PosMap + gameEngine.PosMap, posCase, gameEngine.ScaleMap, 0f, centerCase);
                        //    }
                        //}

                        if (baseCase is RelayCase)
                        {
                            RelayCase relayCase = (RelayCase)baseCase;
                            texCase = ContentManager.Load<Texture2D>(String.Format(@"Content\Pic\{0}", "FleurRelais"));

                            SpriteBatch.Begin();

                            //--- Affiche le relais
                            SpriteBatch.Draw(
                                texCase,
                                gameEngine.PosMap + posCard + new Vector2(gameEngine.CaseSize) * (gameEngine.ScaleMap + 0.1f) / 2f + new Vector2(0, -16f),
                                null,
                                GetColorRelay(relayCase.RelayType),
                                0f, new Vector2(gameEngine.CaseSize) * (gameEngine.ScaleMap) / 2f, gameEngine.ScaleMap
                                //* (gameEngine.scale + 0.1f) / 2f, gameEngine.scale + 0.1f
                                , SpriteEffects.None, 0f
                                );
                            //---

                            SpriteBatch.End();
                        }

                        ////--- Affiche le fond
                        //SpriteBatch.Draw(
                        //    tex2DGround,
                        //    gameEngine.posMap + posCard,
                        //    null,
                        //    Color.SaddleBrown,
                        //    0f, Vector2.Zero, gameEngine.scale, SpriteEffects.None, 0f
                        //    );
                        ////---
                    }
                }
            }

            /*
            for (int x = 0; x < gameEngine.Map.Width; x++)
            {
                for (int y = 0; y < gameEngine.Map.Height; y++)
                {
                    if (gameEngine.Map.Cases[x, y] != null && gameEngine.Map.Cases[x, y] is PlayerCase)
                    {
                        PlayerCase playerCase = (PlayerCase)gameEngine.Map.Cases[x, y];
                        int caseValue = gameEngine.Map.DrawingCaseValue[x, y];

                        if (caseValue >= 0)
                        {
                            Vector2 posCard = gameEngine.posMap;
                            Vector2 posCase = new Vector2(x * gameEngine.caseSize * gameEngine.ScaleMap, y * gameEngine.caseSize * gameEngine.ScaleMap);

                            //--- Affiche la case
                            DrawCase(playerCase, caseValue, GetColorFlower(playerCase.FlowerType), gameEngine.ListAllPlayerColor[playerCase.Player], posCard, posCase, gameEngine.ScaleMap);

                            //---> Affiche les valeurs
                            DrawCaseValuesMap(playerCase, gameEngine.posMap + posCard, posCase, gameEngine.ScaleMap, 0f, centerCase);
                        }
                    }
                }
            }
            */
        }

        private void DrawPlayerCards(int numberPlayer)
        {
            Vector2 posDeck = new Vector2(gameEngine.PosMap.X + gameEngine.Map.Width * gameEngine.CaseSize + gameEngine.Marge, gameEngine.PosMap.Y);

            for (int numCard = 0; numCard < gameEngine.ListAllPlayerCard[numberPlayer].Count; numCard++)
            {
                for (int x = 0; x < gameEngine.WidthPlayerCard; x++)
                {
                    for (int y = 0; y < gameEngine.HeightPlayerCard; y++)
                    {
                        int caseValue = gameEngine.ListAllPlayerCard[numberPlayer][numCard].DrawingCaseValue[x, y];

                        if (caseValue >= 0)
                        {
                            Vector2 posCard = new Vector2(numCard * (gameEngine.SmallMarge + gameEngine.WidthPlayerCard * gameEngine.CaseSize * gameEngine.scale) + x * gameEngine.CaseSize * gameEngine.scale, y * gameEngine.CaseSize * gameEngine.scale + (gameEngine.CaseSize * gameEngine.scale * gameEngine.HeightPlayerCard + gameEngine.Marge) * (numberPlayer - 1));

                            //--- Affiche le fond
                            SpriteBatch.Draw(
                                tex2DGround,
                                posDeck + posCard,
                                null,
                                gameEngine.ListAllPlayerColor[numberPlayer],
                                //GetColorFlower(gameEngine.ListAllPlayerCard[numberPlayer][numCard].FlowerType),
                                0f,
                                Vector2.Zero, gameEngine.scale, SpriteEffects.None, 0f
                                );
                            //---

                            //--- Affiche le contour de la case
                            Texture2D texCase = ContentManager.Load<Texture2D>(String.Format(@"Content\Pic\{0}", caseValue));

                            SpriteBatch.Draw(
                                texCase,
                                posDeck + posCard,
                                null,
                                GetColorFlower(gameEngine.ListAllPlayerCard[numberPlayer][numCard].FlowerType),
                                //gameEngine.ListAllPlayerColor[numberPlayer],
                                0f,
                                Vector2.Zero, gameEngine.scale, SpriteEffects.None, 0f);
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
            for (int x = 0; x < gameEngine.WidthPlayerCard; x++)
            {
                for (int y = 0; y < gameEngine.HeightPlayerCard; y++)
                {
                    int caseValue = gameEngine.CurrentPlayerCard.DrawingCaseValue[x, y];

                    if (caseValue >= 0)
                    {
                        Vector2 posCard = new Vector2((float)(x + 0.5f) * (float)gameEngine.CaseSize * gameEngine.ScaleSelectedCard, (float)(y + 0.5f) * (float)gameEngine.CaseSize * gameEngine.ScaleSelectedCard);
                        center += posCard;
                    }
                }
            }

            center /= 4f;
            //---

            for (int x = 0; x < gameEngine.WidthPlayerCard; x++)
            {
                for (int y = 0; y < gameEngine.HeightPlayerCard; y++)
                {
                    int caseValue = gameEngine.CurrentPlayerCard.DrawingCaseValue[x, y];

                    if (caseValue >= 0)
                    {
                        Vector2 posCard = new Vector2((float)x * (float)gameEngine.CaseSize * gameEngine.scale, (float)y * (float)gameEngine.CaseSize * gameEngine.scale);

                        Matrix mtxTransform =
                            Matrix.CreateTranslation(new Vector3(-center, 0f)) *
                            Matrix.CreateRotationZ(gameEngine.CurrentCardRotation) *
                            Matrix.CreateTranslation(gameEngine.MouseState.X, gameEngine.MouseState.Y, 0f);

                        SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState, mtxTransform);

                        //--- Affiche le fond
                        SpriteBatch.Draw(
                            tex2DGround,
                            posCard,
                            null,
                            gameEngine.ListAllPlayerColor[gameEngine.CurrentPlayerCard.Player],
                            //GetColorFlower(gameEngine.CurrentPlayerCard.FlowerType),
                            0, Vector2.Zero,
                            gameEngine.ScaleSelectedCard, SpriteEffects.None, 0f
                            );
                        //---

                        //--- Affiche le contour de la case
                        Texture2D texCase = ContentManager.Load<Texture2D>(String.Format(@"Content\Pic\{0}", caseValue));

                        SpriteBatch.Draw(
                            texCase,
                            posCard,
                            null,
                            GetColorFlower(gameEngine.CurrentPlayerCard.FlowerType),
                            //gameEngine.ListAllPlayerColor[gameEngine.CurrentPlayerCard.Player],
                            0, Vector2.Zero,
                            gameEngine.ScaleSelectedCard, SpriteEffects.None, 0f);
                        //---

                        SpriteBatch.End();
                    }
                }
            }
        }

        private void DrawSelectedCardOverMap()
        {
            gameEngine.CurrentPlayerCard.CalcDimensions();

            int width = gameEngine.CurrentPlayerCard.Width;
            int height = gameEngine.CurrentPlayerCard.Height;

            int realWidth = gameEngine.CloneSelectedCard.DrawingCaseValue.GetUpperBound(0) + 1;
            int realHeight = gameEngine.CloneSelectedCard.DrawingCaseValue.GetUpperBound(1) + 1;

            Vector2 vecDeltaScaleCardOverMap = new Vector2(realWidth * gameEngine.CaseSize, realHeight * gameEngine.CaseSize) * (gameEngine.ScaleCardOverMap - gameEngine.ScaleMap) / 2f;

            Vector2 centerCard = new Vector2((gameEngine.CurrentPlayerCard.Center.X + 0.5f) * gameEngine.CaseSize * gameEngine.ScaleCardOverMap, (gameEngine.CurrentPlayerCard.Center.Y + 0.5f) * gameEngine.CaseSize * gameEngine.ScaleCardOverMap);
            Vector2 centerRotatedCard = new Vector2((gameEngine.CloneSelectedCard.Center.X + 0.5f) * gameEngine.CaseSize * gameEngine.ScaleCardOverMap, (gameEngine.CloneSelectedCard.Center.Y + 0.5f) * gameEngine.CaseSize * gameEngine.ScaleCardOverMap);

            Vector2 posCard = gameEngine.PosMap + gameEngine.VecPosSelectedCardOnMap + centerRotatedCard - vecDeltaScaleCardOverMap;

            //this.Game.Window.Title = posCard.ToString();

            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    Vector2 posCase = new Vector2(x * gameEngine.CaseSize * gameEngine.ScaleCardOverMap, y * gameEngine.CaseSize * gameEngine.ScaleCardOverMap);

                    PlayerCase caseCard = gameEngine.CurrentPlayerCard.Cases[x, y];

                    BaseCase caseMap = gameEngine.GetCaseMapFromRotatedCard(gameEngine.PosSelectedCardOnMap.X, gameEngine.PosSelectedCardOnMap.Y, x, y, realWidth, realHeight);

                    if (caseCard != null)
                    {
                        int caseValue = gameEngine.CurrentPlayerCard.DrawingCaseValue[x, y];

                        if (gameEngine.ContextType == ContextType.CardRotatedOverMap)
                        {
                            //--- Affiche la case
                            DrawCase(caseCard, caseValue, GetColorFlower(gameEngine.CloneSelectedCard.FlowerType), gameEngine.ListAllPlayerColor[gameEngine.CurrentPlayer], posCard, posCase, gameEngine.ScaleCardOverMap, gameEngine.CurrentCardRotation, centerCard);

                            //---> Affiche les valeurs
                            DrawCaseValuesMap(caseCard, posCard, posCase, gameEngine.ScaleCardOverMap, gameEngine.CurrentCardRotation, centerCard);
                        }
                        else if (caseMap is PlayerCase)
                        {
                            PlayerCase caseMapPlayer = (PlayerCase)caseMap;

                            //---> Conflit
                            if (caseCard != null && caseMap != null && caseMapPlayer.FlowerType != FlowerType.None && caseMapPlayer.Player != gameEngine.CurrentPlayer)
                            {
                                //--- Affiche la case
                                DrawCase(caseCard, caseValue, GetColorFlower(gameEngine.CloneSelectedCard.FlowerType), Color.Gray, posCard, posCase, gameEngine.ScaleCardOverMap, gameEngine.CurrentCardRotation, centerCard);

                                //---> Affiche les valeurs
                                DrawCaseValuesConflict(caseMapPlayer, caseCard, posCase, gameEngine.ScaleCardOverMap);
                            }
                            //---> Renfort
                            else if (caseCard != null && caseMap != null && caseMapPlayer.FlowerType != FlowerType.None && caseMapPlayer.Player == gameEngine.CurrentPlayer)
                            {
                                //--- Affiche la case
                                DrawCase(caseCard, caseValue, GetColorFlower(gameEngine.CloneSelectedCard.FlowerType), gameEngine.ListAllPlayerColor[gameEngine.CurrentPlayer], posCard, posCase, gameEngine.ScaleCardOverMap, gameEngine.CurrentCardRotation, centerCard);

                                //---> Affiche les valeurs
                                DrawCaseValuesRenfort(caseMapPlayer, caseCard, posCase, gameEngine.ScaleCardOverMap);
                            }
                        }
                        else
                        {

                            //---> Nouvelle installation
                            //else
                            {
                                //--- Affiche la case
                                DrawCase(caseCard, caseValue, GetColorFlower(gameEngine.CloneSelectedCard.FlowerType), gameEngine.ListAllPlayerColor[gameEngine.CurrentPlayer], posCard, posCase, gameEngine.ScaleCardOverMap, gameEngine.CurrentCardRotation, centerCard);

                                //---> Affiche les valeurs
                                DrawCaseValuesMap(caseCard, posCard, posCase, gameEngine.ScaleCardOverMap, gameEngine.CurrentCardRotation, centerCard);
                            }
                        }
                    }
                }
            }
        }

        private void DrawCardPuttingDown()
        {
            int width = gameEngine.CurrentPlayerCard.DrawingCaseValue.GetUpperBound(0) + 1;
            int height = gameEngine.CurrentPlayerCard.DrawingCaseValue.GetUpperBound(1) + 1;

            int realWidth = gameEngine.CloneSelectedCard.DrawingCaseValue.GetUpperBound(0) + 1;
            int realHeight = gameEngine.CloneSelectedCard.DrawingCaseValue.GetUpperBound(1) + 1;

            Vector2 vecDeltaScaleCardPuttingDown = new Vector2(realWidth * gameEngine.CaseSize, realHeight * gameEngine.CaseSize) * (gameEngine.ScaleCardPuttingDown - gameEngine.ScaleMap) / 2f;

            Color colorBorder = gameEngine.ListAllPlayerColor[gameEngine.CurrentPlayer];
            Color colorBackground = GetColorFlower(gameEngine.CurrentPlayerCard.FlowerType);

            Vector2 centerCard = new Vector2((gameEngine.CurrentPlayerCard.Center.X + 0.5f) * gameEngine.CaseSize * gameEngine.ScaleCardPuttingDown, (gameEngine.CurrentPlayerCard.Center.Y + 0.5f) * gameEngine.CaseSize * gameEngine.ScaleCardPuttingDown);
            Vector2 centerRotatedCard = new Vector2((gameEngine.CloneSelectedCard.Center.X + 0.5f) * gameEngine.CaseSize * gameEngine.ScaleCardPuttingDown, (gameEngine.CloneSelectedCard.Center.Y + 0.5f) * gameEngine.CaseSize * gameEngine.ScaleCardPuttingDown);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2 posCard = gameEngine.PosMap + gameEngine.VecPosSelectedCardOnMap + centerRotatedCard - vecDeltaScaleCardPuttingDown;
                    Vector2 posCase = new Vector2(x * gameEngine.CaseSize * gameEngine.ScaleCardPuttingDown, y * gameEngine.CaseSize * gameEngine.ScaleCardPuttingDown);

                    PlayerCase caseCard = gameEngine.CurrentPlayerCard.Cases[x, y];

                    //Case caseMap = GetCaseMapFromRotatedCard(x + posSelectedCardOngameEngine.Map.X, y + posSelectedCardOngameEngine.Map.Y);

                    if (caseCard != null)
                    {
                        int caseValue = gameEngine.CurrentPlayerCard.DrawingCaseValue[x, y];

                        //--- Affiche la case
                        DrawCase(caseCard, caseValue, colorBackground, colorBorder, posCard, posCase, gameEngine.ScaleCardPuttingDown, gameEngine.CurrentCardRotation, centerCard);

                        //---> Affiche les valeurs
                        DrawCaseValuesMap(caseCard, posCard, posCase, gameEngine.ScaleCardPuttingDown, gameEngine.CurrentCardRotation, centerCard);
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

            Vector2 centerCase = new Vector2(gameEngine.CaseSize / 2f);

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
            int width = (int)((float)gameEngine.CaseSize * scale);
            int height = (int)((float)gameEngine.CaseSize * scale);

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

            Vector2 vec1 = new Vector2((float)gameEngine.CaseSize * gameEngine.scale * 0.15f, (float)gameEngine.CaseSize * scale * 0.75f);
            Vector2 vec2 = new Vector2((float)gameEngine.CaseSize * gameEngine.scale * 0.4f, (float)gameEngine.CaseSize * scale * 0.75f);
            Vector2 vec3 = new Vector2((float)gameEngine.CaseSize * gameEngine.scale * 0.75f, (float)gameEngine.CaseSize * scale * 0.75f);


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
            int width = (int)((float)gameEngine.CaseSize * scale);
            int height = (int)((float)gameEngine.CaseSize * scale);

            float defenserValue = gameEngine.GetDefenserValue(caseMap, caseCard);
            float strikerValue = gameEngine.GetStrikerValue(caseMap, caseCard);

            SpriteBatch.Begin();

            //---> Défenseur
            SpriteBatch.DrawString(fontCase, Math.Round(defenserValue, 0, MidpointRounding.AwayFromZero).ToString(), pos + new Vector2((float)gameEngine.CaseSize * gameEngine.scale * 0.15f, (float)gameEngine.CaseSize * gameEngine.scale * 0.7f), Color.White);

            //---> Attaquant
            SpriteBatch.DrawString(fontCase, Math.Round(strikerValue, 0, MidpointRounding.AwayFromZero).ToString(), pos + new Vector2((float)gameEngine.CaseSize * gameEngine.scale * 0.65f, (float)gameEngine.CaseSize * gameEngine.scale * 0.7f), Color.White);

            SpriteBatch.End();
        }

        private void DrawCaseValuesRenfort(PlayerCase caseMap, PlayerCase caseCard, Vector2 pos, float scale)
        {
            int width = (int)((float)gameEngine.CaseSize * gameEngine.scale);
            int height = (int)((float)gameEngine.CaseSize * gameEngine.scale);

            SpriteBatch.Begin();

            SpriteBatch.DrawString(fontCase, Math.Round((double)(caseMap.BonusDefenser * (caseMap.Defenser + caseCard.Defenser)), 0, MidpointRounding.AwayFromZero).ToString(), pos + new Vector2((float)gameEngine.CaseSize * gameEngine.scale * 0.15f, (float)gameEngine.CaseSize * gameEngine.scale * 0.7f), Color.Black);
            SpriteBatch.DrawString(fontCase, (caseMap.Defenser + caseCard.Defenser).ToString(), pos + new Vector2((float)gameEngine.CaseSize * gameEngine.scale * 0.4f, (float)gameEngine.CaseSize * gameEngine.scale * 0.7f), Color.Black);
            SpriteBatch.DrawString(fontCase, Math.Round((double)(caseMap.MalusStricker * (caseMap.Defenser + caseCard.Defenser)), 0, MidpointRounding.AwayFromZero).ToString(), pos + new Vector2((float)gameEngine.CaseSize * gameEngine.scale * 0.65f, (float)gameEngine.CaseSize * gameEngine.scale * 0.7f), Color.Black);

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

        private Color GetColorRelay(RelayType relayType)
        {
            Color color = Color.White;

            switch (relayType)
            {
                case RelayType.None:
                    break;
                case RelayType.Pollen:
                    color = Color.LightCoral;
                    break;
                case RelayType.Portal:
                    color = Color.LightGreen;
                    break;
                case RelayType.AddSpores:
                    color = Color.LightSkyBlue;
                    break;
                default:
                    break;
            }

            return color;
        }
        #endregion

        public void DrawBlob(GameTime gameTime)
        {
            //this.vertexBlobArray[0].Position.X = gameEngine.MouseState.X/10;
            //this.vertexBlobArray[0].Position.Y = -gameEngine.MouseState.Y/10;

            //int nbPlayerCase = gameEngine.Map.Cases.OfType<PlayerCase>().Count();
            //int playerCaseId = 0;

            //vertexBlobArray = new VertexPositionColor[nbPlayerCase];
            //List<VertexPositionColor> listVertex = new List<VertexPositionColor>();
            List<VertexBlob> listVertex = new List<VertexBlob>();
            float delta = 0.5f;

            for (int y = 0; y < gameEngine.Map.Height; y++)
            {
                for (int x = 0; x < gameEngine.Map.Width; x++)
                {
                    BaseCase baseCase = this.gameEngine.Map.Cases[x, y];

                    if (baseCase is PlayerCase)
                    {
                        PlayerCase playerCase = (PlayerCase)baseCase;

                        float vertexSize = 1f;

                        if (playerCase.GrowingCase)
                            vertexSize = MathHelper.Lerp(playerCase.StartValueGrowingCase, playerCase.EndValueGrowingCase, playerCase.PercentageGrowingCase);
                        else if (playerCase.NewCase)
                            break;

                        if(vertexSize>1f)
                            vertexSize = 1f;

                        if (gameEngine.AreCasesEqual(baseCase, this.gameEngine.Map.Cases, x - 1, y))
                        {
                            //listVertex.Add(new VertexPositionColor(new Vector3((x - delta) * gameEngine.CaseSize, -y * gameEngine.CaseSize, 0), gameEngine.ListAllPlayerColor[playerCase.Player]));

                            listVertex.Add(new VertexBlob(new Vector3((x - delta) * gameEngine.CaseSize, -y * gameEngine.CaseSize, 0), GetColorFlower(playerCase.FlowerType), vertexSize, gameEngine.ListAllPlayerColor[playerCase.Player]));
                        }

                        if (gameEngine.AreCasesEqual(baseCase, this.gameEngine.Map.Cases, x + 1, y))
                        {
                            //listVertex.Add(new VertexPositionColor(new Vector3((x + delta) * gameEngine.CaseSize, -y * gameEngine.CaseSize, 0), gameEngine.ListAllPlayerColor[playerCase.Player]));
                            listVertex.Add(new VertexBlob(new Vector3((x + delta) * gameEngine.CaseSize, -y * gameEngine.CaseSize, 0), GetColorFlower(playerCase.FlowerType), vertexSize, gameEngine.ListAllPlayerColor[playerCase.Player]));
                        }

                        if (gameEngine.AreCasesEqual(baseCase, this.gameEngine.Map.Cases, x, y - 1))
                        {
                            //listVertex.Add(new VertexPositionColor(new Vector3(x * gameEngine.CaseSize, (-y + delta) * gameEngine.CaseSize, 0), gameEngine.ListAllPlayerColor[playerCase.Player]));
                            listVertex.Add(new VertexBlob(new Vector3(x * gameEngine.CaseSize, (-y + delta) * gameEngine.CaseSize, 0), GetColorFlower(playerCase.FlowerType), vertexSize, gameEngine.ListAllPlayerColor[playerCase.Player]));
                        }

                        if (gameEngine.AreCasesEqual(baseCase, this.gameEngine.Map.Cases, x, y + 1))
                        {
                            //listVertex.Add(new VertexPositionColor(new Vector3(x * gameEngine.CaseSize, (-y - delta) * gameEngine.CaseSize, 0), gameEngine.ListAllPlayerColor[playerCase.Player]));
                            listVertex.Add(new VertexBlob(new Vector3(x * gameEngine.CaseSize, (-y - delta) * gameEngine.CaseSize, 0), GetColorFlower(playerCase.FlowerType), vertexSize, gameEngine.ListAllPlayerColor[playerCase.Player]));
                        }

                        //listVertex.Add(new VertexPositionColor(new Vector3(x * gameEngine.CaseSize, -y * gameEngine.CaseSize, 0), gameEngine.ListAllPlayerColor[playerCase.Player]));
                        listVertex.Add(new VertexBlob(new Vector3(x * gameEngine.CaseSize, -y * gameEngine.CaseSize, 0), GetColorFlower(playerCase.FlowerType), vertexSize, gameEngine.ListAllPlayerColor[playerCase.Player]));
                    }
                }
            }

            vertexBlobArray = listVertex.ToArray();

            if (vertexBlobArray != null && vertexBlobArray.Length > 0)
            {
                //---
                DrawMRT(gameTime);
                //---

                colorTex = colorTarget.GetTexture();
                normalTex = normalTarget.GetTexture();
            }
            else
            {
                int size = colorTarget.Width * colorTarget.Height;
                Color[] clrEmpty = new Color[size];

                clrEmpty.SetValue(Color.White, 0);

                colorTex = new Texture2D(gameEngine.GraphicsDevice, colorTarget.Width, colorTarget.Height);
                colorTex.SetData<Color>(clrEmpty);
            }

            //gameEngine.Game.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;

            //---
            sceneTex = sceneTarget.GetTexture();

            //playerTex = playerTarget.GetTexture();
            //unitTex = unitTarget.GetTexture();
            //infoTex = infoTarget.GetTexture();
            //---

            if (gameEngine.DoScreenShot)
            {
                sceneTex.Save(@"d:\sceneTex.Bmp", ImageFileFormat.Bmp);
                normalTex.Save(@"d:\normalTex.Bmp", ImageFileFormat.Bmp);
                colorTex.Save(@"d:\colorTex.Bmp", ImageFileFormat.Bmp);
                //playerTex.Save(@"d:\playerTex.Bmp", ImageFileFormat.Bmp);
                //unitTex.Save(@"d:\unitTex.Bmp", ImageFileFormat.Bmp);
                //infoTex.Save(@"d:\infoTex.Bmp", ImageFileFormat.Bmp);

                gameEngine.DoScreenShot = false;
            }

            //---
            blobColorEffect.Parameters["SceneBuffer"].SetValue(sceneTex);
            blobColorEffect.Parameters["NormalBuffer"].SetValue(normalTex);
            blobColorEffect.Parameters["ColorBuffer"].SetValue(colorTex);
            blobColorEffect.Parameters["CubeTex"].SetValue(cubeTex);

            //if (!simpleVertex)
            //{
            //    blobColorEffect.Parameters["PlayerBuffer"].SetValue(playerTex);
            //    blobColorEffect.Parameters["UnitBuffer"].SetValue(unitTex);
            //    blobColorEffect.Parameters["InfoBuffer"].SetValue(infoTex);
            //    blobColorEffect.Parameters["Zoom"].SetValue(-zoom);
            //}

            blobColorEffect.Begin();
            blobColorEffect.Techniques[0].Passes[0].Begin();

            quad.Render(Vector2.One * -1, Vector2.One);

            blobColorEffect.Techniques[0].Passes[0].End();
            blobColorEffect.End();
            //---

            //---
            //SpriteBatch.Begin();

            //if (drawPhysicObjects)
            //{
            //    DrawPhysicObjects();
            //}

            //UIManager.Draw(gameTime, SpriteBatch);

            //SpriteBatch.End();
            //if (drawPhysicObjects)
            //{
            //    SpriteBatch.Begin();
            //    DrawPhysicObjects();
            //    SpriteBatch.End();
            //}
            //---
        }

        //private void DrawPhysicObjects()
        //{
        //    float ratio = 750f / -zoom;
        //    PhysicsSimulatorView.factor = ratio;
        //    PhysicsSimulatorView.centerScreen = new Vector2(-vecTranslation.X * ratio + (float)this.gameEngine.Game.GraphicsDevice.Viewport.Width / 2f, -vecTranslation.Y * ratio + (float)this.gameEngine.Game.GraphicsDevice.Viewport.Height / 2f);
        //    PhysicsSimulatorView.Draw(SpriteBatch);
        //}

        private void DrawMRT(GameTime gameTime)
        {
            SetRTs();

            gameEngine.Game.GraphicsDevice.Clear(Color.White);

            #region RenderStates
            gameEngine.Game.GraphicsDevice.RenderState.PointSpriteEnable = true;
            gameEngine.Game.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            gameEngine.Game.GraphicsDevice.RenderState.SourceBlend = Blend.One;
            gameEngine.Game.GraphicsDevice.RenderState.DestinationBlend = Blend.One;
            gameEngine.Game.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;

            //gameEngine.Game.GraphicsDevice.RenderState.PointSizeMin = 1f;
            //gameEngine.Game.GraphicsDevice.RenderState.PointSizeMax = 2000f;

            //gameEngine.Game.GraphicsDevice.RenderState.PointSize = 100.0f;

            #endregion

            gameEngine.Game.GraphicsDevice.VertexDeclaration = vertexPosColDecl;

            ViewProjection = View * Projection;

            //--- BlobEffect 1
            blobEffect.Parameters["WorldViewProjection"].SetValue(ViewProjection);
            blobEffect.Parameters["Projection"].SetValue(Projection);
            blobEffect.Parameters["ParticleSize"].SetValue(104);
            blobEffect.Parameters["ViewportHeight"].SetValue((float)this.gameEngine.Game.GraphicsDevice.Viewport.Height);
            blobEffect.Parameters["GaussBlob"].SetValue(gaussianTex);

            //if (!simpleVertex)
            //{
            //    blobEffect.Parameters["GaussUnit"].SetValue(gaussianUnitTex);
            //    blobEffect.Parameters["Zoom"].SetValue(-zoom);
            //}

            blobEffect.Begin();
            blobEffect.Techniques[0].Passes[0].Begin();

            //if (simpleVertex)
            //{
            //gameEngine.Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.PointList,
            //         vertexBlobArray, 0, vertexBlobArray.Length);

            gameEngine.Game.GraphicsDevice.DrawUserPrimitives<VertexBlob>(PrimitiveType.PointList,
                     vertexBlobArray, 0, vertexBlobArray.Length);
            //}
            //else
            //{
            //    gameEngine.Game.GraphicsDevice.DrawUserPrimitives<VertexBlob>(PrimitiveType.PointList,
            //         vertexBlobArray, 0, vertexBlobArray.Length);
            //}

            blobEffect.Techniques[0].Passes[0].End();
            blobEffect.End();
            //---

            #region UnsetRenderStates
            //gameEngine.Game.GraphicsDevice.RenderState.PointSpriteEnable = false;
            //gameEngine.Game.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            gameEngine.Game.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            #endregion

            ResolveRTs();

            //SetRTs2();

            ////--- BlobEffect 2
            //blob2Effect.Parameters["WorldViewProjection"].SetValue(ViewProjection);
            //blob2Effect.Parameters["Projection"].SetValue(Projection);
            //blob2Effect.Parameters["ParticleSize"].SetValue(4f);
            //blob2Effect.Parameters["ViewportHeight"].SetValue((float)this.gameEngine.Game.GraphicsDevice.Viewport.Height);
            //blob2Effect.Parameters["GaussBlob"].SetValue(gaussianTex);
            //blob2Effect.Parameters["Zoom"].SetValue(-zoom);

            //blob2Effect.Begin();
            //blob2Effect.Techniques[0].Passes[0].Begin();

            //gameEngine.Game.GraphicsDevice.DrawUserPrimitives<VertexBlob>(PrimitiveType.PointList,
            //         vertexBlobArrayTest, 0, vertexBlobArray.Length);

            //blob2Effect.Techniques[0].Passes[0].End();
            //blob2Effect.End();
            //---



            //ResolveRTs2();
        }

        private void SetRTs()
        {
            //gameEngine.Game.GraphicsDevice.SetRenderTarget(4, infoTarget);
            //gameEngine.Game.GraphicsDevice.SetRenderTarget(3, unitTarget);
            //gameEngine.Game.GraphicsDevice.SetRenderTarget(2, playerTarget);
            gameEngine.Game.GraphicsDevice.SetRenderTarget(1, colorTarget);
            gameEngine.Game.GraphicsDevice.SetRenderTarget(0, normalTarget);
        }

        private void ResolveRTs()
        {
            gameEngine.Game.GraphicsDevice.SetRenderTarget(0, null);
            gameEngine.Game.GraphicsDevice.SetRenderTarget(1, null);
            //gameEngine.Game.GraphicsDevice.SetRenderTarget(2, null);
            //gameEngine.Game.GraphicsDevice.SetRenderTarget(3, null);
            //gameEngine.Game.GraphicsDevice.SetRenderTarget(4, null);
        }

        //private void SetRTs2()
        //{
        //    gameEngine.Game.GraphicsDevice.SetRenderTarget(0, infoTarget);
        //}

        //private void ResolveRTs2()
        //{
        //    gameEngine.Game.GraphicsDevice.SetRenderTarget(0, null);
        //}

        private void GenerateGaussianTexture(ref Texture2D texture, float gaussianDeviation)
        {
            texture = new Texture2D(this.gameEngine.Game.GraphicsDevice, GAUSSIANSIZE, GAUSSIANSIZE, 0, TextureUsage.None, SurfaceFormat.Single);

            int u, v;
            float dx, dy, I;

            float[] temp = new float[GAUSSIANSIZE * GAUSSIANSIZE];

            for (v = 0; v < GAUSSIANSIZE; ++v)
            {
                for (u = 0; u < GAUSSIANSIZE; ++u)
                {
                    dx = 2.0f * u / (float)GAUSSIANSIZE - 1.0f;
                    dy = 2.0f * v / (float)GAUSSIANSIZE - 1.0f;
                    I = GAUSSIANSIZE * (float)Math.Exp(-(dx * dx + dy * dy) / gaussianDeviation);

                    int pos = u + v * GAUSSIANSIZE;
                    temp[pos] = I;
                }
            }

            texture.SetData<float>(temp);
            //texture.Save(@"d:\gaussian.bmp", ImageFileFormat.Png);
        }
    }
}
