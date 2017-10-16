﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//Buttons, text, etc. Simpel en spreekt voor zich.
namespace Core
{
    public static class UI
    {
        public static void TextInCenter(String s, Vector2 offset, Vector2 size, SpriteBatch batch, SpriteFont font, Color col)
        {
            batch.DrawString(font, s, (size / 2f) + offset, col, 0, font.MeasureString(s) / 2f, 1f, SpriteEffects.None, 1f);
        }
    }

    public class UIElement
    {
        protected Vector2 position;
        private Vector2 size, sizemul;
        private bool dirtysize = true;
        public Color colour;

        public UIElement(Vector2 position, Vector2 size)
        {
            this.position = position;
            this.size = size;
            colour = Color.White;
        }

        public UIElement(Vector2 position, Vector2 size, Color colour)
        {
            this.position = position;
            this.size = size;
            this.colour = colour;
        }

        protected Vector2 DrawSize(Vector2 physicalsize)
        {
            if (dirtysize)
            {
                sizemul = size * Grid.Scale(physicalsize);
                dirtysize = false;
            }
            return sizemul;
        }

        public Vector2 Size
        {
            get { return size; }
            set { size = value; dirtysize = true; }
        }
    }

    public class UITextureElement : UIElement
    {
        protected Texture2D texture;

        public UITextureElement(string texture, Vector2 position, Vector2 size)
            : base(position, size)
        {
            this.texture = AssetManager.GetResource<Texture2D>(texture);
        }

        public void Draw(SpriteBatch batch)
        {
            Vector2 drawsize = DrawSize(new Vector2(texture.Width, texture.Height));
            batch.Draw(texture, Grid.ToScreenSpace(position), null, 
                colour, 0.0f, Vector2.Zero, drawsize, 
                SpriteEffects.None, 0.0f);
        }
    }

    public class Text : UIElement
    {
        public string text;

        public Text(string text, Vector2 position, Vector2 size) 
            : base(position, size)
        {
            this.text = text;
        }

        public void Draw(SpriteBatch batch, SpriteFont font)
        {
            UI.TextInCenter(text, Grid.ToScreenSpace(position), Grid.ToScreenSpace(Size), batch, font, colour);
        }
    }

    public class Button : UITextureElement
    {
        public Color baseColour, highlightColour, downColour, textColour;
        protected string text;
        protected Bounds bounds;
        protected Action pressAction;

        public Button(string text, string texture, Action a,Vector2 position, Vector2 size) 
            : base(texture, position, size)
        {
            colour = Color.White;
            highlightColour = Color.White;
            downColour = Color.White;
            textColour = Color.Black;
            baseColour = Color.White;
            this.text = text;
            this.texture = AssetManager.GetResource<Texture2D>(texture);
            bounds = new Bounds(position.X, position.Y, size.X, size.Y);
            pressAction = a;
        }

        public void SetupColours(Color baseColour, Color highlightColour, Color downColour, Color textColour)
        {
            this.baseColour = baseColour;
            this.highlightColour = highlightColour;
            this.downColour = downColour;
            this.textColour = textColour;
            colour = baseColour;
        }

        public void Update()
        {
            Vector2 mpos = Input.GetMousePosition();
            bool down = Input.GetMouseButton(PressAction.DOWN, MouseButton.LEFT);
            bool pressed = Input.GetMouseButton(PressAction.RELEASED, MouseButton.LEFT);
            bool hover = bounds.Inside(mpos);
            if (hover && pressed && pressAction != null) pressAction();
            if (hover && down) colour = downColour;
            else if (hover) colour = highlightColour;
            else colour = baseColour;
        }

        public void Draw(SpriteBatch batch, SpriteFont font)
        {
            base.Draw(batch);
            UI.TextInCenter(text, Grid.ToScreenSpace(position), Grid.ToScreenSpace(Size), batch, font, textColour);
        }
    }
}