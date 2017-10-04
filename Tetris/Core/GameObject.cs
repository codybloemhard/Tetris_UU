using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core
{
    public class Bounds
    {
        public float x, y, w, h;

        public Bounds(float x, float y, float w, float h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }

        public bool Intersects(Bounds b)
        {
            if (x < b.x + b.w && x + w > b.x &&
                y < b.y + b.h && y + h > b.y)
                return true;
            return false;
        }

        public bool Inside(Vector2 p)
        {
            if (p.X > x && p.X < x + w && p.Y > y && p.Y < y + h)
                return true;
            return false;
        }
    }
    
    public abstract partial class GameObject
    {
        /*zodat manager zich zelf can registreren,
        maar dat niet mogelijk is van buitenaf door andere objecten.
        */
        public partial class GameObjectManager { }

        private bool dirtybounds = true, dirtydrawscale = true;
        private Vector2 pos, size, sizemul;
        public string tag = "";
        protected GameObjectManager manager;
        protected Texture2D sprite;
        protected Bounds bounds;
        protected Color colour;

        public GameObject()
        {
            bounds = new Bounds(0, 0, 0, 0);
            dirtybounds = true;
        }
        public GameObject(string tag)
        {
            this.tag = tag;
            bounds = new Bounds(0, 0, 0, 0);
            dirtybounds = true;
        }

        public abstract void Init();
        public abstract void Update(GameTime gameTime);
        public virtual void Draw(GameTime time, SpriteBatch batch)
        {
            if (dirtydrawscale)
            {
                sizemul = Size * Grid.Scale(new Vector2(sprite.Width, sprite.Height));
                dirtydrawscale = false;
            }
            //scale de sprite zodat alles resolutie independed is.
            batch.Draw(sprite, Grid.ToScreenSpace(Pos), null, colour, 0.0f, Vector2.Zero, sizemul, SpriteEffects.None, 0.0f);
        }

        //GetBounds creates a rectangle that matches the dimensions of the drawn sprite
        /*System with a diryflag ensures we do not have to calculate new bounds
        Everytime we either check for collision or updadate our position.*/
        public Bounds GetBounds()
        {
            if (!dirtybounds) return bounds;
            bounds.x = pos.X;
            bounds.y = pos.Y;
            bounds.w = size.X;
            bounds.h = size.Y;
            dirtybounds = false;
            return bounds;
        }
        /*Find with Tag functies zoals in Unity3D, zodat
        we niet harde links hoeven te leggen tussen objecten.
        Het zou een zooi worden als we straks een grotere game maken
        en objecten onderling zouden linken in de main class.
        */
        public GameObject FindWithTag(string tag)
        {
            return manager.FindWithTag(tag);
        }

        public GameObject[] FindAllWithTag(string tag)
        {
            return manager.FindAllWithTag(tag);
        }

        public GameObject[] FindAllWithTags(string[] tags)
        {
            return manager.FindAllWithTags(tags);
        }

        //this method uses the rectangle created in GetBounds to check if two sprites collide
        public bool Collides(GameObject e)
        {
            if (GetBounds().Intersects(e.GetBounds())) return true;
            return false;
        }

        public Vector2 Pos
        {
            get { return pos; }
            set { pos = value;  dirtybounds = true; }
        }

        public Vector2 Size
        {
            get { return size; }
            set { size = value; dirtybounds = true; dirtydrawscale = true; }
        }

        public Color Colour
        {
            get { return colour; }
            set { colour = value; }
        }
    }
}