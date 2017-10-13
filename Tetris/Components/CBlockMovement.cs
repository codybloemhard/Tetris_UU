using System;
using System.Collections.Generic;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tetris
{
    public class CBlockMovement : Component
    {
        private bool[,] shape;
        private TetrisGrid grid;

        public CBlockMovement(GameObject parent, SpriteBatch batch, byte shapeN)
            : base(parent)
        {
            grid = gameObject.FindWithTag("grid").GetComponent<TetrisGrid>();
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
                    GameObject obj = new GameObject("child", gameObject.Manager);
                    obj.AddComponent("", new CRender(obj, "block", batch));
                    obj.SetParent(gameObject);
                    obj.LocalPos = new Vector2(x * parent.Size.X, y * parent.Size.Y);
                    obj.LocalSize = new Vector2(1.0f, 1.0f);
                    obj.active = shape[x, y];
                }
            }
        }
        
        //minX, maxX, minY, maxY
        public int[] GetMinMax(bool[,] shape)
        {
            int[] ans = new int[4] { 9, 0, 9, 0 };

            for(int x = 0; x < 4; x++)
                for(int y = 0; y < 4; y++)
                {
                    if (shape[x, y])
                    {
                        ans[0] = Math.Min(ans[0], x);
                        ans[1] = Math.Max(ans[1], x);
                        ans[2] = Math.Min(ans[2], y);
                        ans[3] = Math.Max(ans[3], y);
                    }
                }

            return ans;
        }

        private bool[,] Transpose(bool[,] mat)
        {
            int w = mat.GetLength(0);
            int h = mat.GetLength(1);
            bool[,] ans = new bool[w, h];

            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    ans[x, y] = mat[y, x];
            return ans;
        }

        private bool[,] Reorder(bool[,] mat, bool hor)
        {
            int w = mat.GetLength(0);
            int h = mat.GetLength(1);
            bool[,] ans = new bool[h, w];
            if(hor)
                for(int y = 0; y < h; y++)
                    for(int x = 0; x < w; x++)
                        ans[x, y] = mat[w - x - 1, y];
            else
                for (int x = 0; x < w; x++)
                    for (int y = 0; y < h; y++)
                        ans[x, y] = mat[x, h - y - 1];

            return ans;
        }

        private void UpdateShape(bool[,] newshape)
        {
            shape = newshape;
            for (int x = 0; x < 4; x++)
                for (int y = 0; y < 4; y++)
                {
                    GameObject obj = gameObject.Childeren[x * 4 + y];
                    if (shape[x, y]) obj.active = true;
                    else obj.active = false;
                }
        }

        public bool[,] Shape { get { return shape; } }

        public override void Update(float time)
        {
            base.Update(time);
            if (Input.GetKey(PressAction.RELEASED, Keys.S))
                grid.BoostDown(this);
            else if (Input.GetKey(PressAction.RELEASED, Keys.W))
                grid.SettleDown(this);
            gameObject.Pos += Vector2.UnitY * 1.0f * time;
            int move = 0;
            if (Input.GetKey(PressAction.RELEASED, Keys.D))
                move = 1;
            if (Input.GetKey(PressAction.RELEASED, Keys.A))
                move = -1;
            grid.CheckCollision(this, move);
            int rotate = 0;
            if (Input.GetKey(PressAction.RELEASED, Keys.E)) rotate = 1;
            if (Input.GetKey(PressAction.RELEASED, Keys.Q)) rotate = 2;
            if(rotate != 0)
            {
                bool[,] newshape = Transpose(shape);
                newshape = Reorder(newshape, rotate == 1 ? true : false);
                bool ok = grid.CheckValidRotation(this, newshape);
                if(ok) UpdateShape(newshape);
            }
        }
    }
}