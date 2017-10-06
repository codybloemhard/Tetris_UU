using System;
using System.Collections.Generic;
using Core;

namespace Tetris
{
    public class TetrisGrid
    {
        private GameObject[,] grid;

        public TetrisGrid()
        {
            grid = new GameObject[12, 22];
        }
        
        public void CheckCollision(CBlockMovement block)
        {

        }

        public void LockDown(CBlockMovement block)
        {

        }
    }
}
