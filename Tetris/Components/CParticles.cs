using System;
using Core;
using Microsoft.Xna.Framework;

namespace Tetris
{
    public class CParticles : Component
    {
        private ParticleEmitter emitter;
        private TetrisGrid grid;
        private Random rand;

        public CParticles(GameObject parent, ParticleEmitter emitter) : base(parent)
        {
            this.emitter = emitter;
            rand = new Random();
        }

        public override void Init()
        {
            base.Init();
            grid = gameObject.FindWithTag("grid").GetComponent<TetrisGrid>();
        }

        public void EmitColumn(Vector2 from, Vector2 to, uint divisions)
        {
            Vector2 diff = to - from;
            Vector2 step = diff / divisions;
            for (uint i = 0; i < divisions; i++)
                for(int j = 0; j < 4; j++)
                    emitter.Emit(from + (step * i), grid.BlockSize, MathH.RandomUnitVector(),
                    RandomColor(), 5.0f, 120, 1.0f, 0.95f, 1);
        }

        public Color RandomColor()
        {
            Color[] colours = { Color.Cyan, Color.Blue, Color.Orange, Color.Yellow, Color.Green, Color.Purple, Color.Red };
            return colours[rand.Next(0, 6)];
        }
    }
}