using System;
using System.Collections.Generic;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tetris
{
    public class CBlockMovement : Component
    {
        private TetrisGrid grid;
        private bool[,] shape;

        public CBlockMovement(GameObject parent, TetrisGrid grid, SpriteBatch batch, byte shapeN)
            : base(parent)
        {
            this.grid = grid;
            shape = new bool[4, 4];
            if (shapeN < 0 || shapeN > 6)
                shapeN = 0;
            //load the block shape from a file!
            int block = 0, counter = 0;
            bool[] buffer = new bool[16];
            string text = System.IO.File.ReadAllText("Blocks.txt");
            for(int i = 0; i < text.Length; i++)
            {
                if(block < shapeN)
                {
                    if (text[i] == ';')
                        block++;
                    continue;
                }
                if(text[i] == 't')
                    buffer[counter++] = true;
                if (text[i] == 'f')
                    buffer[counter++] = false;
                if(counter == 15)
                {
                    for(int j = 0; j < 16; j++)
                    {
                        shape[j % 4, j / 4] = buffer[j];
                        buffer[j] = false;
                    }
                    break;
                }
            }
            //create the child object with the shape array
            for(int x = 0; x < 4; x++)
            {
                for(int y = 0; y < 4; y++)
                {
                    GameObject obj = new GameObject(gameObject.Manager);
                    obj.AddComponent("", new CRender(obj, "block", batch));
                    obj.SetParent(gameObject);
                    obj.LocalPos = new Vector2(x * parent.Size.X, y * parent.Size.Y);
                    obj.LocalSize = new Vector2(1.0f, 1.0f);
                    obj.active = shape[x, y];
                }
            }
        }

        public override void Update(float time)
        {
            base.Update(time);
            gameObject.Pos += Vector2.UnitY * 0.2f * time;
        }
    }
}