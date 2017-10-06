using System;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tetris.States
{
    public class ClassicTetris : GameState
    {
        private GameObject.GameObjectManager objectmanager;
        private GameObject obj;

        public ClassicTetris() { }

        public void Load(SpriteBatch batch)
        {
            objectmanager = new GameObject.GameObjectManager();
            obj = new GameObject();
            obj.AddComponent("render", new CRender(obj, "block", batch));
            obj.Pos = new Vector2(2, 1);
            obj.Size = new Vector2(2, 2);
            objectmanager.Add(obj);
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
