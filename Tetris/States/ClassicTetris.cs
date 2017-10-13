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
        }
        
        public void Unload()
        {
            objectmanager.Clear();
        }

        public void Update(float time)
        {
            objectmanager.Update(time);
        }

        public void Draw(GameTime time, SpriteBatch batch, GraphicsDevice device)
        {
            device.Clear(Color.Black);
            batch.Begin();
            objectmanager.Draw();
            batch.End();
        }
    }
}