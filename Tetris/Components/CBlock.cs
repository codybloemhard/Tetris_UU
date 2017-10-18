using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Core;

namespace Tetris
{
    public class CBlock : Component
    {
        private bool[,] shape;
        private TetrisGrid grid;
        private byte shapeN;

        public CBlock(GameObject parent, SpriteBatch batch, byte shapeN)
            : base(parent)
        {
            grid = gameObject.FindWithTag("grid").GetComponent<TetrisGrid>();
            shape = new bool[4, 4];
            //create the child object with the shape array
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    GameObject obj = new GameObject("child", gameObject.Manager);
                    obj.AddComponent("", new CRender(obj, "block", batch));
                    obj.SetParent(gameObject);
                    obj.LocalPos = new Vector2(x * parent.Size.X, y * parent.Size.Y);
                    obj.LocalSize = new Vector2(1.0f, 1.0f);
                    obj.active = false;
                }
            }
            //transform into shape
            SetShape(shapeN);
        }

        public bool[,] Shape { get { return shape; } }
        //returns minX, maxX, minY, maxY op die volgorde
        public int[] GetMinMax(bool[,] shape)
        {
            int[] ans = new int[4] { 9, 0, 9, 0 };
            for (int x = 0; x < 4; x++)
                for (int y = 0; y < 4; y++)
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
        //simpele matrix operatie
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
        //rows of columns omdraaien
        private bool[,] Reorder(bool[,] mat, bool hor)
        {
            int w = mat.GetLength(0);
            int h = mat.GetLength(1);
            bool[,] ans = new bool[h, w];
            if (hor)
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                        ans[x, y] = mat[w - x - 1, y];
            else
                for (int x = 0; x < w; x++)
                    for (int y = 0; y < h; y++)
                        ans[x, y] = mat[x, h - y - 1];

            return ans;
        }
        //de juiste childs aan en uitzetten
        private void UpdateShape(bool[,] newshape)
        {
            shape = newshape;
            Color[] colours = { Color.Cyan, Color.Blue, Color.Orange, Color.Yellow, Color.Green, Color.Purple, Color.Red };
            Color c = colours[shapeN];
            for (int x = 0; x < 4; x++)
                for (int y = 0; y < 4; y++)
                {
                    GameObject obj = gameObject.Childeren[x * 4 + y];
                    if (shape[x, y]) obj.active = true;
                    else obj.active = false;
                    obj.Renderer.colour = c;
                }
        }
        //in een bepaalde tetris figuur transformeren
        public void SetShape(byte shapeN)
        {
            if (shapeN < 0 || shapeN > 6)
                shapeN = 0;
            this.shapeN = shapeN;
            //load the block shape from a file
            int block = 0, counter = 0;
            bool[] buffer = new bool[16];
            string text = System.IO.File.ReadAllText("Blocks.txt");
            for (int i = 0; i < text.Length; i++)
            {
                if (block < shapeN)
                {
                    if (text[i] == ';')
                        block++;
                    continue;
                }
                if (text[i] == 't')
                    buffer[counter++] = true;
                if (text[i] == 'f')
                    buffer[counter++] = false;
                if (counter == 15)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        shape[j % 4, j / 4] = buffer[j];
                        buffer[j] = false;
                    }
                    break;
                }
            }
            //set childs 
            UpdateShape(shape);
        }
        //draaien van het figuur
        public void Rotate(int rotate)
        {
            if (rotate != 0)
            {
                bool[,] newshape = Transpose(shape);
                newshape = Reorder(newshape, rotate == 1 ? true : false);
                bool ok = grid.CheckValidRotation(this, newshape);
                if (ok) UpdateShape(newshape);
            }
        }

        public byte ShapeN { get { return (byte)(shapeN + 1); } }
    }
}