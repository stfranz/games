using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GoingBeyondTutorial
{
    abstract class Sprite
    {
        public Vector2 Position { get; set; }
        public float Scale { get; set; }
        public float Rotation { get; set; }
        public Color TextColor { get; set; }
        public float Alpha { get; set; }
        public bool Centered { get; set; }

        public abstract Vector2 Size();
        public abstract void Draw(SpriteBatch sb);
    }
    class TextSprite : Sprite
    {
        public SpriteFont Font
        {
            get
            {
                return mFont;
            }
            set
            {
                mFont = value;
                if (Text != null)
                {
                    mOrigin = mFont.MeasureString(Text) / 2;
                }
            }
        }
        public string Text
        {
            get
            {
                return mText;
            }
            set
            {
                mText = value;
                if (mFont != null)
                {
                    mOrigin = mFont.MeasureString(Text) / 2;
                }
            }
        }
        public TextSprite(string text, SpriteFont font)
        {
            mFont = font;
            Text = text;
            Position = Vector2.Zero;
            TextColor = Color.White;
            Centered = false;
            Scale = 1;
            Alpha = 255;
        }

        public override Vector2 Size()
        {
            return mFont.MeasureString(Text);
        }

        private SpriteFont mFont;
        private string mText;
        private Vector2 mOrigin;

        public override void Draw(SpriteBatch sb)
        {
            Vector2 origin;
            if (Centered)
            {
                origin = mOrigin;
            }
            else
            {
                origin = Vector2.Zero;
            }
            sb.DrawString(Font, mText, Position, new Color(TextColor.R, TextColor.G, TextColor.B, (int)(Alpha * 255)), Rotation, origin, Scale, SpriteEffects.None, 0);
        }
    }
    class SpriteBin
    {
        List<Sprite> sprites;

        private SpriteFont _font;
        public SpriteBin(SpriteFont font)
        {
            sprites = new List<Sprite>();
            _font = font;
        }

        public TextSprite AddTextSprite(string content)
        {
            TextSprite s = new TextSprite(content, _font);
            sprites.Add(s);
            return s;
        }

        public void Add(Sprite s)
        {
            sprites.Add(s);
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (Sprite sprite in sprites)
            {
                sprite.Draw(sb);
            }
        }

    }
}
