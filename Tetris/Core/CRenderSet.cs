using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core
{
    public class CRenderSet : CRender
    {
        private int[,] set;
        private Texture2D[] textures;
        private Vector2 pos, size;
        private Vector2[] sizemultipliers;
        private Vector2 blocksize;

        public CRenderSet(GameObject parent, string sprite, SpriteBatch batch)
            : base(parent, sprite, batch)
        {
        }

        public void InitSet(int[,] set, string[] sprites, Vector2 pos, Vector2 size)
        {
            textures = new Texture2D[sprites.Length];
            for (int i = 0; i < sprites.Length; i++)
                textures[i] = AssetManager.GetResource<Texture2D>(sprites[i]);
            sizemultipliers = new Vector2[textures.Length];
            blocksize = new Vector2(size.X / set.GetLength(0), size.Y / set.GetLength(1));
            for (int i = 0; i < sizemultipliers.Length; i++)
            {
                if (textures[i] == null)
                    sizemultipliers[i] = Vector2.Zero;
                else
                    sizemultipliers[i] = blocksize * Grid.Scale(new Vector2(textures[i].Width, textures[i].Height));
            }
            this.set = set;
            this.pos = pos;
            this.size = size;
        }
        public int[,] Set { set { set = value; } }

        public override void Update(float time)
        {
            base.Update(time);
            if (!active) return;
            Render();
        }

        private void Render()
        { 
            if (set == null) return;
            for (int x = 0; x < set.GetLength(0); x++)
                for(int y = 0; y < set.GetLength(1); y++)
                {
                    int i = set[x, y];
                    if (textures[i] == null) continue;
                    Vector2 cstep = blocksize * new Vector2(x, y);
                    batch.Draw(textures[i], Grid.ToScreenSpace(pos + (cstep)), null, colour, 0.0f, Vector2.Zero, sizemultipliers[i], SpriteEffects.None, 0.0f);
                }
        }
    }
}