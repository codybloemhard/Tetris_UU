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
        private TetrisGrid grid;
        private CBlock block;
        private float speed;

        public CBlockMovement(GameObject parent, float speed) : base(parent)
        {
            grid = gameObject.FindWithTag("grid").GetComponent<TetrisGrid>();
            block = gameObject.GetComponent<CBlock>();
            this.speed = speed;
        }

        public override void Update(float time)
        {
            base.Update(time);
            if (Input.GetKey(PressAction.RELEASED, Keys.S))
                grid.BoostDown(block);
            else if (Input.GetKey(PressAction.RELEASED, Keys.W))
                grid.SettleDown(block);
            gameObject.Pos += Vector2.UnitY * speed * time;
            int move = 0;
            if (Input.GetKey(PressAction.RELEASED, Keys.D))
                move = 1;
            if (Input.GetKey(PressAction.RELEASED, Keys.A))
                move = -1;
            grid.CheckCollision(block, move);
            int rotate = 0;
            if (Input.GetKey(PressAction.RELEASED, Keys.E)) rotate = 1;
            if (Input.GetKey(PressAction.RELEASED, Keys.Q)) rotate = 2;
            block.Rotate(rotate);
        }

        public float Speed { get { return speed; } }
    }
}