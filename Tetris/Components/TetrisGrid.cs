﻿using System;
using System.Collections.Generic;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

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
        private byte level = 0, blockcounter = 0;
        private GameObject nextblockIndicator;
        private bool gameover;
        private SoundEffect blocklock, lineclear;
        private CParticles emitter;

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
            Color[] colours = { Color.White, Color.Cyan, Color.Blue, Color.Orange, Color.Yellow, Color.Green, Color.Purple, Color.Red };
            renderer.InitSet(grid, "block", colours, gameObject.Pos, gameObject.Size);
            nextblockIndicator = new GameObject(gameObject.Manager);
            nextblockIndicator.Pos = new Vector2(12.0f*9.0f/20.0f + 0.5f, 1);
            nextblockIndicator.Size = blocksize;
            nextblockIndicator.AddComponent("block", new CBlock(nextblockIndicator, batch, 0));
            gameover = false;
            DataManager.SetData<int>("score", 0);
            DataManager.SetData<byte>("level", 0);
            SpawnNewPiece();
            blocklock = AssetManager.GetResource<SoundEffect>("blocklock");
            lineclear = AssetManager.GetResource<SoundEffect>("lineclear");
            emitter = gameObject.FindWithTag("emitter").GetComponent<CParticles>();
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
                        if (grid[xx, MathHelper.Clamp(yy + 1, 0, GH - 1)] != 0)
                        {
                            LockDown(block);
                            return true;
                        }     
                        //kan naar rechts of links? (part1)
                        if (grid[Math.Max(0, xx - 1), Math.Max(yy, 0)] != 0)
                            cangoL = false;
                        if (grid[Math.Min(GW - 1, xx + 1), Math.Max(yy, 0)] != 0)
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
                        {
                            block.GameObject.Destroy();
                            SpawnNewPiece();
                            return;
                        }
                        grid[xx, yy] = block.ShapeN;
                    }
                }
            blocklock.Play();
            DataManager.SetData<int>("score", DataManager.GetData<int>("score") + 1);
            block.GameObject.Destroy();
            UpdateField();
            SpawnNewPiece();
        }
        //kijk of er een hele rij is die we weg kunnen halen
        private void UpdateField()
        {
            for(int x = 0; x < GW; x++)
            {
                if (grid[x, 0] != 0)
                {
                    gameover = true;
                    GameStateManager.RequestChange(new GameStateChange("gameover", CHANGETYPE.LOAD));
                    return;
                }
            }
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
                    DataManager.SetData<int>("score", DataManager.GetData<int>("score") + 100);
                    lineclear.Play();
                    emitter.EmitColumn(gameObject.Pos + (Vector2.UnitY * y * blocksize), 
                        gameObject.Pos + (Vector2.UnitY * y * blocksize) + (Vector2.UnitX * GW * blocksize), GW);
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
                    if (grid[xx, yy] != 0)
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
            float delta = 1000f;
            for (int i = 0; i < blockwidth; i++)
                if (deltas[i] < delta)
                    delta = deltas[i];
            delta -= 1.5f;
            if (delta < 0) delta = 0;
            block.GameObject.Pos += Vector2.UnitY * delta * blocksize;
        }
        //spawn een nieuwe blok
        public void SpawnNewPiece()
        {
            if (gameover) return;
            blockcounter++;
            if(blockcounter > 10)
            {
                blockcounter = 0;
                level++;
                DataManager.SetData<byte>("level", level);
            }
            GameObject obj = new GameObject("obj", gameObject.Manager);
            obj.Pos = new Vector2(blocksize.X * 3, -blocksize.Y*3);
            obj.Size = blocksize;
            obj.AddComponent("block", new CBlock(obj, batch, nextblock));
            obj.AddComponent("move", new CBlockMovement(obj, 1.0f + ((float)level / 5.0f)));
            nextblock = (byte)(random.NextDouble() * 7);
            nextblockIndicator.GetComponent<CBlock>().SetShape(nextblock);
        }

        public Vector2 BlockSize { get { return blocksize; } }
        public byte NextBlock { get { return nextblock; } }
    }
}