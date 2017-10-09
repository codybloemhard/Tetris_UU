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
        private Random random;

        public ClassicTetris() { }

        public void Load(SpriteBatch batch)
        {
            this.batch = batch;
            random = new Random();
            objectmanager = new GameObjectManager();

            GameObject grid = new GameObject("grid", objectmanager);
            grid.Pos = new Vector2(0, 0);
            grid.Size = new Vector2(12.0f * 9.0f/22.0f, 9);
            grid.AddComponent("render", new CRenderSet(grid, "button", batch));
            grid.AddComponent("grid", new TetrisGrid(grid));
            this.grid = grid;

            GameObject obj = new GameObject("obj", objectmanager);
            obj.Pos = new Vector2(0, 0);
            obj.Size = (grid.GetComponent<TetrisGrid>() as TetrisGrid).BlockSize;
            obj.AddComponent("move", new CBlockMovement(obj, batch, 3));
            
            objectmanager.Init();
        }

        public void Unload()
        {
            objectmanager.Clear();
        }

        public void Update(float time)
        {
            objectmanager.Update(time);          
            if(Input.GetMouseButton(PressAction.RELEASED, MouseButton.LEFT))
            {
                GameObject obj = new GameObject("obj", objectmanager);
                obj.Pos = new Vector2(0, -1);
                obj.Size = (grid.GetComponent<TetrisGrid>() as TetrisGrid).BlockSize;
                obj.AddComponent("move", new CBlockMovement(obj, batch, (byte)(random.NextDouble() * 7)));
            } 
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