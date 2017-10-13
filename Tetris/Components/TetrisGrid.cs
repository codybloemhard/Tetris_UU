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
        //true als lockdown is gebeurt
        public bool CheckCollision(CBlockMovement block, int move)
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
                        GridSpace(block.GameObject.Pos, x, y, out xx, out yy);
                        if(grid[xx, Math.Min(yy + 1, 21)] == 1)
                        {
                            LockDown(block);
                            return true;
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
            {
                LockDown(block);
                return true;
            }
            return false;
        }
        
        /*public bool CheckCollision(CBlockMovement block, bool[,] shape, Vector2 pos)
        {
            int[] minmax = block.GetMinMax(shape);
            //collision with lockeddown blocks
            for (int x = 0; x < 4; x++)
                for (int y = 0; y < 4; y++)
                {
                    if (shape[x, y])
                    {
                        int xx, yy;
                        GridSpace(pos, x, y, out xx, out yy);
                        if (grid[xx, Math.Min(yy + 1, 21)] == 1)
                            return true;
                    }
                }
            if (block.GameObject.Pos.Y + ((minmax[3] + 1) * BlockSize.Y) > gameObject.Pos.Y + gameObject.Size.Y)
                return true;
            return false;
        }*/
        
        private void GridSpace(Vector2 pos, int x, int y, out int xx, out int yy)
        {
            xx = (int)(((pos.X + ((x + 0.5f) * BlockSize.X)) - GameObject.Pos.X) / GameObject.Size.X * 12);
            yy = (int)(((pos.Y + ((y + 0.5f) * BlockSize.Y)) - GameObject.Pos.Y) / GameObject.Size.Y * 22);
        }

        private int ColumnHeight(int rowindex)
        {
            for (int y = 0; y < 22; y++)
                if (grid[rowindex, y] != 0)
                    return 22 - y;
            return 0;
        }

        private void LockDown(CBlockMovement block)
        {
            for(int x = 0; x < 4; x++) 
                for(int y = 0; y < 4; y++)
                {
                    if(block.Shape[x, y])
                    {
                        int xx, yy;
                        GridSpace(block.GameObject.Pos, x, y, out xx, out yy);
                        grid[xx, yy] = 1;
                    }
                }
            block.GameObject.Destroy();
            UpdateField();
        }
        //kijk of er een hele rij is die we weg kunnen halen
        private void UpdateField()
        {
            for (int y = 0; y < 22; y++)
            {
                bool complete = true;
                for (int x = 0; x < 12; x++)
                {
                    if(grid[x, y] == 0)
                    {
                        complete = false;
                        break;
                    }
                }
                if (complete)
                {
                    for (int x = 0; x < 12; x++)
                        grid[x, y] = 0;
                    ShiftField(y);
                }
            }
        }
        //schuif alles boven de lijn 1 naar beneden
        private void ShiftField(int emptyrow)
        {
            for (int y = emptyrow; y > 1; y--)
                for (int x = 0; x < 12; x++)
                    grid[x, y] = grid[x, y - 1];
            for (int x = 0; x < 12; x++)
                grid[x, 0] = 0;
        }
        //kijk of de shape in het veld past zonder al vaste blokken te raken
        public bool CheckValidRotation(CBlockMovement block, bool[,] shape)
        {
            int[] minmax = block.GetMinMax(shape);
            int minx, miny, maxx, maxy;
            GridSpace(block.GameObject.Pos, minmax[0], minmax[2], out minx, out miny);
            GridSpace(block.GameObject.Pos, minmax[1], minmax[3], out maxx, out maxy);
            if (minx <= 0 || maxx >= 11 || miny <= 0 || maxy >= 21)
                return false;
            
            for (int x = 0; x < 4; x++)
                for(int y = 0; y < 4; y++)
                {
                    int xx, yy;
                    GridSpace(block.GameObject.Pos, x, y, out xx, out yy);
                    if (grid[xx, yy] == 1)
                        return false;
                }
            return true;
        }

        public void BoostDown(CBlockMovement block)
        {
            
        }

        public void SettleDown(CBlockMovement block)
        {
            
        }

        public Vector2 BlockSize { get { return blocksize; } }
    }
}