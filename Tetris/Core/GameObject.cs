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
        private List<GameObject> childs;
        private Dictionary<string, Component> components;
        private Component[] comparray;//for fast iteration
        protected GameObject parent;
        protected GameObjectManager manager;
        protected Texture2D sprite;
        protected Bounds bounds;
        protected Color colour;
        public string tag = "";

        public GameObject()
        {
            construct();
        }
        public GameObject(string tag)
        {
            construct();
            this.tag = tag;
        }
        private void construct()
        {
            bounds = new Bounds(0, 0, 0, 0);
            dirtybounds = true;
            dirtydrawscale = true;
            colour = Color.White;
            childs = new List<GameObject>();
            components = new Dictionary<string, Component>();
        }

        public abstract void Init();
        public virtual void Update(GameTime gameTime)
        {
            for (int i = 0; i < comparray.Length; i++)
                comparray[i].Update();
        }
        public virtual void Draw(GameTime time, SpriteBatch batch)
        {
            if (dirtydrawscale)
            {
                sizemul = size * Grid.Scale(new Vector2(sprite.Width, sprite.Height));
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

        public bool HasComponent(string name)
        {
            return components.ContainsKey(name);
        }
        public T GetComponent<T>()
        {
            for (int i = 0; i < comparray.Length; i++)
            {
                if (comparray[i] is T)
                    return (T)Convert.ChangeType(comparray[i], typeof(T));
            }
            return default(T);
        }
        public T GetComponent<T>(string name)
        {
            if(components.ContainsKey(name))
                return (T)Convert.ChangeType(components[name], typeof(T));
            return default(T);
        }
        public void AddComponent(string name, Component com)
        {
            components.Add(name, com);
            comparray = new Component[components.Count];
            components.Values.CopyTo(comparray, 0);
        }
        public int ComponentCount { get { return components.Count; }  }

        public GameObject Parent { get { return parent; } }
        public GameObject[] Childeren { get { return childs.ToArray(); } }
        public GameObject GetChild(int i)
        {
            if (i < 0 || i > childs.Count - 1)
                return null;
            return childs[i];
        }
        public void AddChild(GameObject obj)
        {
            childs.Add(obj);
        }
        public void RemoveChild(GameObject obj)
        {
            childs.Remove(obj);
        }
        public void SetParent(GameObject obj)
        {
            parent = obj;
            obj.AddChild(this);
        }
        public void DeChild()
        {
            parent.RemoveChild(this);
            parent = null;
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