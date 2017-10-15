using System;
using System.Collections.Generic;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tetris
{
    public class TetrisGrid : Component
    {
        private int[,] grid;
        private CRenderSet renderer;
        private Vector2 blocksize;
        private Random random;
        private SpriteBatch batch;
        private const int GW = 12, GH = 20;//grid width and height
        private byte nextblock;
        private GameObject nextblockIndicator;

        public TetrisGrid(GameObject parent, SpriteBatch batch) : base(parent)
        {
            blocksize = gameObject.Size / new Vector2(GW, GH);
            this.batch = batch;
            random = new Random();
        }
        
        public override void Init()
        {
            grid = new int[GW, GH];
            renderer = gameObject.Renderer as CRenderSet;
            renderer.InitSet(grid, new string[] { "", "block" }, gameObject.Pos, gameObject.Size);
            nextblockIndicator = new GameObject(gameObject.Manager);
            nextblockIndicator.Pos = new Vector2(12.0f*9.0f/20.0f + 1, 1);
            nextblockIndicator.Size = blocksize;
            nextblockIndicator.AddComponent("block", new CBlock(nextblockIndicator, batch, 0));
            SpawnNewPiece();
        }

        public override void Update(float time)
        {
            base.Update(time);
        }
        //true als lockdown is gebeurt
        public bool CheckCollision(CBlock block, int move)
        {
            int[] minmax = block.GetMinMax(block.Shape);
            //kijk of het block naar links of naar rechts mag (part0)
            int lx, rx, ry;
            GridSpace(block.GameObject.Pos, minmax[1], 0, out rx, out ry);
            GridSpace(block.GameObject.Pos, minmax[0], 0, out lx, out ry);
            bool cangoL = lx > 0;
            bool cangoR = rx < GW - 1;
            //collision with lockeddown blocks
            for(int x = 0; x < 4; x++)
                for(int y = 0; y < 4; y++)
                {
                    if (block.Shape[x, y])
                    {
                        int xx, yy;
                        GridSpace(block.GameObject.Pos, x, y, out xx, out yy);
                        if (xx < 0 || xx > GW - 1 || yy < 0 || yy > GH - 1)
                        {
                            cangoL = cangoR = false;
                            continue;
                        }
                        if (grid[xx, MathHelper.Clamp(yy + 1, 0, GH - 1)] == 1)
                        {
                            LockDown(block);
                            return true;
                        }     
                        //kan naar rechts of links? (part1)
                        if (grid[Math.Max(0, xx - 1), Math.Max(yy, 0)] == 1)
                            cangoL = false;
                        if (grid[Math.Min(GW - 1, xx + 1), Math.Max(yy, 0)] == 1)
                            cangoR = false;
                    }
                }
            if (move != 0 && (((cangoL && move == -1) || (cangoR && move == 1))))
                block.GameObject.Pos += Vector2.UnitX * move * blocksize;
            if (block.GameObject.Pos.Y + ((minmax[3] + 1) * BlockSize.Y) > gameObject.Pos.Y + gameObject.Size.Y)
            {
                LockDown(block);
                return true;
            }
            return false;
        }
        //vertaal de wereld-positie naar de grid-positie
        private void GridSpace(Vector2 pos, int x, int y, out int xx, out int yy)
        {
            xx = (int)(((pos.X + ((x + 0.5f) * BlockSize.X)) - GameObject.Pos.X) / GameObject.Size.X * GW);
            yy = (int)(((pos.Y + ((y + 0.5f) * BlockSize.Y)) - GameObject.Pos.Y) / GameObject.Size.Y * GH);
        }
        //hoeveel blokken hoog is deze column?
        private int ColumnHeight(int rowindex)
        {
            for (int y = 0; y < GH; y++)
                if (grid[rowindex, y] != 0)
                    return GH - y;
            return 0;
        }
        //zet block vast in grid
        private void LockDown(CBlock block)
        {
            for(int x = 0; x < 4; x++) 
                for(int y = 0; y < 4; y++)
                {
                    if(block.Shape[x, y])
                    {
                        int xx, yy;
                        GridSpace(block.GameObject.Pos, x, y, out xx, out yy);
                        if (xx < 0 || xx > GW - 1 || yy < 0 || yy > GH - 1)
                            return;
                        grid[xx, yy] = 1;
                    }
                }
            block.GameObject.Destroy();
            UpdateField();
            SpawnNewPiece();
        }
        //kijk of er een hele rij is die we weg kunnen halen
        private void UpdateField()
        {
            for (int y = 0; y < GH; y++)
            {
                bool complete = true;
                for (int x = 0; x < GW; x++)
                {
                    if(grid[x, y] == 0)
                    {
                        complete = false;
                        break;
                    }
                }
                if (complete)
                {
                    for (int x = 0; x < GW; x++)
                        grid[x, y] = 0;
                    ShiftField(y);
                }
            }
        }
        //schuif alles boven de lijn 1 naar beneden
        private void ShiftField(int emptyrow)
        {
            for (int y = emptyrow; y > 1; y--)
                for (int x = 0; x < GW; x++)
                    grid[x, y] = grid[x, y - 1];
            for (int x = 0; x < GW; x++)
                grid[x, 0] = 0;
        }
        //kijk of de shape in het veld past zonder al vaste blokken te raken
        public bool CheckValidRotation(CBlock block, bool[,] shape)
        {
            int[] minmax = block.GetMinMax(shape);
            int minx, miny, maxx, maxy;
            GridSpace(block.GameObject.Pos, minmax[0], minmax[2], out minx, out miny);
            GridSpace(block.GameObject.Pos, minmax[1], minmax[3], out maxx, out maxy);
            if (minx <= 0 || maxx >= GW - 1 || miny <= 0 || maxy >= GH - 1)
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
        //fastworward een blok naar beneden
        public void BoostDown(CBlock block)
        {
            int[] minmax = block.GetMinMax(block.Shape);
            int blockwidth = minmax[1] - minmax[0] + 1;
            int[] deltas = new int[blockwidth];
            for (int i = 0; i < blockwidth; i++)
            {
                int xx, yy;
                GridSpace(block.GameObject.Pos, minmax[0] + i, minmax[3], out xx, out yy);
                int height = ColumnHeight(xx);
                deltas[i] = GH - yy - height;
            }
            int delta = 0;
            for (int i = 0; i < blockwidth; i++)
                if (deltas[i] > delta)
                    delta = deltas[i];         
            if (delta > 1)
            block.GameObject.Pos += Vector2.UnitY * 1.0f * blocksize;
        }
        //zet een blok zover naar beneden dat hij niet meer veder kan
        public void SettleDown(CBlock block)
        {
            int[] minmax = block.GetMinMax(block.Shape);
            int blockwidth = minmax[1] - minmax[0] + 1;
            int[] deltas = new int[blockwidth];
            for (int i = 0; i < blockwidth; i++)
            {
                int xx, yy;
                GridSpace(block.GameObject.Pos, minmax[0] + i, minmax[3], out xx, out yy);
                int height = ColumnHeight(xx);
                deltas[i] = GH - yy - height;
            }
            int delta = 1000;
            for (int i = 0; i < blockwidth; i++)
                if (deltas[i] < delta)
                    delta = deltas[i];
            delta--;
            if (delta < 0) delta = 0;
            block.GameObject.Pos += Vector2.UnitY * delta * blocksize;
        }
        //spawn een nieuwe blok
        public void SpawnNewPiece()
        {
            GameObject obj = new GameObject("obj", gameObject.Manager);
            obj.Pos = new Vector2(0, -blocksize.Y*3);
            obj.Size = blocksize;
            obj.AddComponent("block", new CBlock(obj, batch, nextblock));
            obj.AddComponent("move", new CBlockMovement(obj));
            nextblock = (byte)(random.NextDouble() * 7);
            nextblockIndicator.GetComponent<CBlock>().SetShape(nextblock);
        }

        public Vector2 BlockSize { get { return blocksize; } }
        public byte NextBlock { get { return nextblock; } }
    }
}