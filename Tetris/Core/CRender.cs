﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core
{
    public class CRender : Component
    {
        protected Vector2 sizemul;
        protected SpriteBatch batch;
        protected Texture2D sprite;
        public Color colour;

        public CRender(GameObject parent, string sprite, SpriteBatch batch) : base(parent)
        {
            this.batch = batch;
            this.sprite = AssetManager.GetResource<Texture2D>(sprite);
            colour = Color.White;
        }

        public override void Update(float time)
        {
            if (sprite == null) return;
            base.Update(time);
            if (gameObject.DirtySize)
                sizemul = gameObject.Size * Grid.Scale(new Vector2(sprite.Width, sprite.Height));
            //scale de sprite zodat alles resolutie independent is.
            batch.Draw(sprite, Grid.ToScreenSpace(gameObject.Pos), null, colour, 0.0f, Vector2.Zero, sizemul, SpriteEffects.None, 0.0f);
        }
    }
}