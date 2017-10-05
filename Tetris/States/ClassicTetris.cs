using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tetris.States
{
    public class Block : GameObject
    {
        public Block()
        {
            sprite = AssetManager.GetResource<Texture2D>("block");
            tag = "block";
            Size = new Vector2(1f, 1f);
        }

        public override void Init()
        {
            Pos = new Vector2(1, 1);
        }

        public override void Update(GameTime time)
        {

        }

        public override void Draw(GameTime time, SpriteBatch batch)
        {
            base.Draw(time, batch);
        }
    }

    public class ClassicTetris : GameState
    {
        private GameObject.GameObjectManager objectmanager;
        private Block b;

        public ClassicTetris() { }

        public void Load()
        {
            objectmanager = new GameObject.GameObjectManager();
            b = new Block();
            objectmanager.Add(b);
            objectmanager.Init();
        }

        public void Unload()
        {
            objectmanager.Clear();
        }

        public void Update(GameTime time)
        {
            objectmanager.Update(time);
        }

        public void Draw(GameTime time, SpriteBatch batch, GraphicsDevice device)
        {
            device.Clear(Color.Black);
            batch.Begin();
            objectmanager.Draw(time, batch);
            batch.End();
        }
    }
}
