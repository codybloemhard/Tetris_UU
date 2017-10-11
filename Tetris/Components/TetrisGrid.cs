using System;
using System.Collections.Generic;
using Core;
using Microsoft.Xna.Framework;

namespace Tetris
{
    public class TetrisGrid : Component
    {
        private int[,] grid;
        private CRenderSet renderer;
        private Vector2 blocksize;
        
        public TetrisGrid(GameObject parent) : base(parent)
        {
            blocksize = gameObject.Size / new Vector2(12, 22);
        }
        
        public override void Init()
        {
            grid = new int[12, 22];
            renderer = gameObject.Renderer as CRenderSet;
            renderer.InitSet(grid, new string[] { "", "block" }, gameObject.Pos, gameObject.Size);
        }

        public override void Update(float time)
        {
            base.Update(time);
        }

        public void CheckCollision(CBlockMovement block, int move)
        {
            int[] minmax = block.GetMinMax(block.Shape);
            //kijk of het block naar links of naar rechts mag (part0)
            bool cangoL = (block.GameObject.Pos.X + ((minmax[0] - 1) * BlockSize.X) >= gameObject.Pos.X);
            bool cangoR = (block.GameObject.Pos.X + ((minmax[1] + 1) * BlockSize.X) <= gameObject.Pos.X + gameObject.Size.X);
            
            //collision with lockeddown blocks
            for(int x = 0; x < 4; x++)
                for(int y = 0; y < 4; y++)
                {
                    if (block.Shape[x, y])
                    {
                        int xx, yy;
                        GridSpace(block, x, y, out xx, out yy);
                        if(grid[xx, Math.Min(yy + 1, 21)] == 1)
                        {
                            LockDown(block);
                            return;
                        }
                        //kan naar rechts of links? (part1)
                        if (grid[Math.Max(0, xx - 1), yy] == 1)
                            cangoL = false;
                        if (grid[Math.Min(11, xx + 1), yy] == 1)
                            cangoR = false;
                    }
                }
            if (move != 0 && ((cangoL && move == -1) || (cangoR && move == 1)))
                block.GameObject.Pos += Vector2.UnitX * move * blocksize;
            if (block.GameObject.Pos.Y + ((minmax[3] + 1) * BlockSize.Y) > gameObject.Pos.Y + gameObject.Size.Y)
                LockDown(block);
        }

        void GridSpace(CBlockMovement block, int x, int y, out int xx, out int yy)
        {
            xx = (int)(((block.GameObject.Pos.X + ((x + 0.5f) * BlockSize.X)) - GameObject.Pos.X) / GameObject.Size.X * 12);
            yy = (int)(((block.GameObject.Pos.Y + ((y + 0.5f) * BlockSize.Y)) - GameObject.Pos.Y) / GameObject.Size.Y * 22);
        }

        private void LockDown(CBlockMovement block)
        {
            for(int x = 0; x < 4; x++) 
                for(int y = 0; y < 4; y++)
                {
                    if(block.Shape[x, y])
                    {
                        int xx, yy;
                        GridSpace(block, x, y, out xx, out yy);
                        grid[xx, yy] = 1;
                    }
                }
            block.GameObject.Destroy();
            UpdateField();
        }
        //kijk of er een hele rij is die we weg kunnen halen
        private void UpdateField()
        {
            for(int y = 0; y < 22; y++)
                for(int x = 0; x < 12; x++)
                {

                }
        }
        //kijk of de shape in het veld past zonder al vaste blokken te raken
        public bool CheckValidRotation(CBlockMovement block, bool[,] shape)
        {
            int[] minmax = block.GetMinMax(shape);
            float minx, miny, maxx, maxy;
            minx = block.GameObject.Pos.X + minmax[0] * blocksize.X;
            maxx = block.GameObject.Pos.X + minmax[1] * blocksize.X;
            miny = block.GameObject.Pos.Y + minmax[2] * blocksize.Y;
            maxy = block.GameObject.Pos.Y + minmax[3] * blocksize.Y;
            
            if(minx < gameObject.Pos.X || maxx >= gameObject.Pos.X + gameObject.Size.X
                || miny < gameObject.Pos.Y || maxy >= gameObject.Pos.Y + gameObject.Size.Y)
                return false;
            for(int x = 0; x < 4; x++)
                for(int y = 0; y < 4; y++)
                {
                    int xx, yy;
                    GridSpace(block, x, y, out xx, out yy);
                    if (grid[xx, yy] == 1)
                        return false;
                }
            return true;
        }

        public Vector2 BlockSize { get { return blocksize; } }
    }
}
