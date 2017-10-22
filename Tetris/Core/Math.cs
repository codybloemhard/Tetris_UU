using System;
using Microsoft.Xna.Framework;

namespace Tetris
{
    public static class MathH
    {
        private static Random rand;

        static MathH()
        {
            rand = new Random();
        }

        public static Vector2 RandomUnitVector()
        {
            Vector2 vec = new Vector2((float)rand.NextDouble(), (float)rand.NextDouble());
            vec.Normalize();
            double r = rand.NextDouble();
            if (r < 0.5f) vec.X *= -1;
            r = rand.NextDouble();
            if (r < 0.5f) vec.Y *= -1;
            return vec;
        }
    }
}