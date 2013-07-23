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
using Microsoft.Kinect;

namespace GoingBeyondTutorial
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public const int BackBufferWidth = 1280;
        public const int BackBufferHeight = 720;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont menuFont;
        MenuManager mMenus;
        KinectManager Kinect { get; set; }
        Texture2D hand;
        World mWorld;
        Player p1;

        bool inMenu;
        bool inGame;
        bool GameStart;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = BackBufferWidth;
            graphics.PreferredBackBufferHeight = BackBufferHeight;
        }

        protected override void Initialize()
        {
            Kinect = new KinectManager(graphics);
            p1 = new Player(Kinect);
            mWorld = new World(Content, graphics, this, p1);

            IsFixedTimeStep = false;
            inMenu = true;
            inGame = false;
            GameStart = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mWorld.LoadContent("world.xml");
            
            menuFont = Content.Load<SpriteFont>("menuFont");
            CreateMenus(menuFont);
            hand = Content.Load<Texture2D>("hand");

            HUD disp = new HUD();
            disp.LoadContent(Content);
            p1.Display = disp;
        }

        protected override void UnloadContent()
        {
            Kinect.UnloadContent();
        }

        /// <summary>
        ///  CreateMenus creates the Main Menu and the Confirm Quit menu, and
        ///  activates the main menu.
        /// </summary>
        /// <param name="menuFont"></param>
        protected void CreateMenus(SpriteFont menuFont)
        {
            Menu mainMenu = new Menu(menuFont, "GAME TITLE - Main Menu", true, false);
            Menu confirmQuit = new Menu(BackBufferWidth / 2, BackBufferHeight / 4, (BackBufferHeight / 2), 50, 
                                    menuFont, "Are you sure you want to exit?", false, true);

            mainMenu.AddMenuItem("New Game", () => { this.inMenu = false; this.inGame = true; });
            mainMenu.AddMenuItem("Calibrate Kinect", null);
            mainMenu.AddMenuItem("Quit", () => { confirmQuit.Enabled = true; mainMenu.Enabled = false; });

            confirmQuit.AddMenuItem("Yes", () => { this.Exit(); });
            confirmQuit.AddMenuItem("No", () => { confirmQuit.Enabled = false; mainMenu.Enabled = true; });

            mMenus = new MenuManager();
            mMenus.AddMenu(mainMenu);
            mMenus.AddMenu(confirmQuit);
        }

        protected override void Update(GameTime gameTime)
        {
            if (inGame)
            {
                KeyboardState keyboard = Keyboard.GetState();
                if (keyboard.IsKeyDown(Keys.P))
                {
                    GameStart = true;
                }
                else if (keyboard.IsKeyDown(Keys.Escape))
                {
                    inGame = false; 
                    inMenu = true;
                    GameStart = false;
                    mWorld.Reset();
                }
                if (GameStart)
                {
                    mWorld.Update(gameTime, keyboard);
                }
            }
            else if (inMenu)
            {
                mMenus.Update(gameTime);
            }
            else
            {
                // calibrate kinect?
            }
                base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            if (inGame)
            {
                SetRenderStates3D();
                mWorld.Draw(gameTime);

                spriteBatch.Begin();
                p1.Display.Draw(spriteBatch);
                if (!GameStart)
                {
                    p1.Display.DrawGameStart(spriteBatch);
                }
                if (mWorld.GameOver)
                {
                    p1.Display.DrawGameOver(spriteBatch);
                }
                spriteBatch.End();
            }
            else if (inMenu)
            {
                spriteBatch.Begin();
                mMenus.Draw(spriteBatch);
                spriteBatch.End();
            }
            else
            {
                // draw kinect calibration messages
            }
            base.Draw(gameTime);
        }

        protected void SetRenderStates3D()
        {
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }
    }
}

