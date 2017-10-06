using System;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tetris
{
    public class ClassicTetris : GameState
    {
        private GameObject.GameObjectManager objectmanager;
        private GameObject obj;
        private TetrisGrid grid;

        public ClassicTetris() { }

        public void Load(SpriteBatch batch)
        {
            grid = new TetrisGrid();
            objectmanager = new GameObject.GameObjectManager();
            obj = new GameObject("obj", objectmanager);
            obj.Pos = new Vector2(2, 2);
            obj.Size = new Vector2(0.5f, 0.5f);
            obj.AddComponent("move", new CBlockMovement(obj, grid, batch, 6));
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
