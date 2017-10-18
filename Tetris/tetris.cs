using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Core;
using Tetris;

namespace Tetris
{
    public class Tetris : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GameStateManager gamestates;
        private static Random random = new Random();
        Song song;

        public Tetris()
        {
            graphics = new GraphicsDeviceManager(this);
            //setup worldspace to abstract from screenspace
            Grid.Setup(16, 9, 800, 450);
            graphics.PreferredBackBufferWidth = (int)Grid.ScreenSize.X;
            graphics.PreferredBackBufferHeight = (int)Grid.ScreenSize.Y;
            Content.RootDirectory = "Content";
            this.song = Content.Load<Song>("tetris");
            AssetManager.content = Content;
            this.IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //setup alle gamestates  
            gamestates = new GameStateManager(spriteBatch);
            ClassicTetris game = new ClassicTetris();
            GameOver gameover = new GameOver();
            Menu menu = new Menu();
            gamestates.AddState("game", game);
            gamestates.AddState("gameover", gameover);
            gamestates.AddState("menu", menu);
            gamestates.SetStartingState("menu");
            MediaPlayer.Play(song);
            MediaPlayer.Volume = 0.25f;
            MediaPlayer.IsRepeating = true;
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
                return;
            }
            gamestates.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        protected override void Draw(GameTime gameTime)
        {
            gamestates.Draw((float)gameTime.ElapsedGameTime.TotalSeconds, spriteBatch, GraphicsDevice);
            base.Draw(gameTime);
        }

        public static Random Random { get { return random; } }
    }
}