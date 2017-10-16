using System;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tetris
{
    public class ClassicTetris : GameState
    {
        private GameObjectManager objectmanager;
        private GameObject grid;
        private SpriteBatch batch;
        private Text scoreText, levelText;
        private SpriteFont scorefont;

        public ClassicTetris() { }

        public void Load(SpriteBatch batch)
        {
            this.batch = batch;
            objectmanager = new GameObjectManager();

            GameObject grid = new GameObject("grid", objectmanager);
            grid.Pos = new Vector2(0, 0);
            grid.Size = new Vector2(12.0f * 9.0f/20.0f, 9);
            grid.AddComponent("render", new CRenderSet(grid, "button", batch));
            grid.AddComponent("grid", new TetrisGrid(grid, batch));
            this.grid = grid;
            
            objectmanager.Init();
            //init UI
            scorefont = AssetManager.GetResource<SpriteFont>("mainFont");
            scoreText = new Text("Score: 0", new Vector2(12.0f * 9.0f / 20.0f + 0.5f, 0.2f), new Vector2(2, 0.5f));
            scoreText.colour = Color.Red;
            levelText = new Text("Level: 1", new Vector2(12.0f * 9.0f / 20.0f + 0.5f, 0.7f), new Vector2(2, 0.5f));
            levelText.colour = Color.Blue;
        }
        
        public void Unload()
        {
            objectmanager.Clear();
        }

        public void Update(float time)
        {
            objectmanager.Update(time);
            scoreText.text = "Score: " + DataManager.GetData<int>("score");
            levelText.text = "Level: " + (DataManager.GetData<byte>("level") + 1);
        }

        public void Draw(float time, SpriteBatch batch, GraphicsDevice device)
        {
            device.Clear(Color.Black);
            batch.Begin();
            {
                objectmanager.Draw();
                scoreText.Draw(batch, scorefont);
                levelText.Draw(batch, scorefont);
            }
            batch.End();
        }
    }
}