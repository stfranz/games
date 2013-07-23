using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GoingBeyondTutorial
{
    class MenuManager
    {
        private List<Menu> mMenus;
        public KeyboardState PrevKeyboadState { get; set; }

        public MenuManager()
        {
            mMenus = new List<Menu>();
        }
        public void AddMenu(Menu menu)
        {
            mMenus.Add(menu);
            menu.Parent = this;
        }

        public void Update(GameTime gametime)
        {
            foreach (Menu m in mMenus)
            {
                if (m.UpdateEnabled)
                {
                    m.Update(gametime);
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (Menu m in mMenus)
            {
                if (m.DrawEnabled)
                {
                    m.Draw(sb);
                }
            }
        }
    }
    class Menu
    {
        public MenuManager Parent { get; set; }

        public delegate void MenuItemSelected();
        public delegate void SetFloatValue(float newVal);

        // Positions of Menu Title and MenuItems
        public int TitleX;          
        public int TitleY;
        public int ElementsY;
        public int ElementsDeltaY;

        public bool Centered { get; set; }
        public bool Enabled
        {
            get
            {
                return DrawEnabled && UpdateEnabled;
            }
            set
            {
                DrawEnabled = value;
                UpdateEnabled = value;
            }

        }
        public bool UpdateEnabled { get; set; }
        public bool DrawEnabled { get; set; }

        private List<MenuItem> elements;
        private int currentElement;
        private TextSprite Title;
        private SpriteBin MenuSprites;

        public Menu(SpriteFont menufont, string title, bool beginEnabled)
        {
            Centered = false;
            TitleX = 50;
            TitleY = 25;
            ElementsY = 200;
            ElementsDeltaY = 75;

            elements = new List<MenuItem>();
            MenuSprites = new SpriteBin(menufont);
            Title = MenuSprites.AddTextSprite(title);
            Title.Centered = Centered;
            Title.Position = new Vector2(TitleX, TitleY);
            Enabled = beginEnabled;
        }
        public Menu(SpriteFont menufont, string title, bool beginEnabled, bool isCentered)
        {
            Centered = isCentered;
            TitleX = 50;
            TitleY = 25;
            ElementsY = 200;
            ElementsDeltaY = 75;

            elements = new List<MenuItem>();
            MenuSprites = new SpriteBin(menufont);
            Title = MenuSprites.AddTextSprite(title);
            Title.Centered = Centered;
            Title.Position = new Vector2(TitleX, TitleY);
            Enabled = beginEnabled;
        }
        public Menu(int menuCenterX, int titleY, int elemsY, int elemDeltaY, SpriteFont menufont, string title, bool beginEnabled)
        {
            Centered = false;
            TitleX = menuCenterX;
            TitleY = titleY;
            ElementsY = elemsY;
            ElementsDeltaY = elemDeltaY;

            elements = new List<MenuItem>();
            MenuSprites = new SpriteBin(menufont);
            Title = MenuSprites.AddTextSprite(title);
            Title.Centered = Centered;
            Title.Position = new Vector2(TitleX, TitleY);
            Enabled = beginEnabled;
        }
        public Menu(int menuCenterX, int titleY, int elemsY, int elemDeltaY, SpriteFont menufont, string title, bool beginEnabled, bool isCentered)
        {
            Centered = isCentered;
            TitleX = menuCenterX;
            TitleY = titleY;
            ElementsY = elemsY;
            ElementsDeltaY = elemDeltaY;

            elements = new List<MenuItem>();
            MenuSprites = new SpriteBin(menufont);
            Title = MenuSprites.AddTextSprite(title);
            Title.Centered = Centered;
            Title.Position = new Vector2(TitleX, TitleY);
            Enabled = beginEnabled;
        }

        public void AddMenuItem(string text, MenuItemSelected action)
        {
            TextSprite elemSprite = MenuSprites.AddTextSprite(text);
            elemSprite.Position = new Vector2(TitleX, elements.Count * ElementsDeltaY + ElementsY);
            SelectedMenuItem m = new SelectedMenuItem(elemSprite, action, this);
            m.HomePosition = elemSprite.Position;
            elements.Add(m);
            if (elements.Count == 1)
            {
                elements[0].Highlight();
            }
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState currentState = Keyboard.GetState();
            if (currentState.IsKeyDown(Keys.Up) && Parent.PrevKeyboadState.IsKeyUp(Keys.Up))
            {
                elements[currentElement].UnHighlight();
                currentElement = currentElement - 1;
                if (currentElement < 0)
                    currentElement = 0;
                elements[currentElement].Highlight();
            }
            if (currentState.IsKeyDown(Keys.Left) && Parent.PrevKeyboadState.IsKeyUp(Keys.Left))
            {
                elements[currentElement].Decrease();
            }
            if (currentState.IsKeyDown(Keys.Right) && Parent.PrevKeyboadState.IsKeyUp(Keys.Right))
            {
                elements[currentElement].Increase();
            }
            if (currentState.IsKeyDown(Keys.Down) && Parent.PrevKeyboadState.IsKeyUp(Keys.Down))
            {
                elements[currentElement].UnHighlight();
                currentElement = currentElement + 1;
                if (currentElement >= elements.Count)
                    currentElement = elements.Count - 1;
                elements[currentElement].Highlight();
            }
            if (currentState.IsKeyDown(Keys.Enter) && Parent.PrevKeyboadState.IsKeyUp(Keys.Enter))
            {
                elements[currentElement].Select();
            }
            Parent.PrevKeyboadState = currentState;
        }

        public void Draw(SpriteBatch sb)
        {
            MenuSprites.Draw(sb);
        }
    }

    class MenuItem
    {
        public Vector2 HomePosition { get; set; }

        public virtual void Select()
        {
        }
        public virtual void Highlight()
        {
        }
        public virtual void UnHighlight()
        {
        }
        public virtual void Decrease()
        {
        }
        public virtual void Increase()
        {
        }
    }

    class SelectedMenuItem : MenuItem
    {
        public TextSprite Text { get; set; }
        public Menu.MenuItemSelected Action { get; set; }
        public Menu Parent { get; set; }
        private Vector2 selOffset;

        public SelectedMenuItem(TextSprite sprite, Menu.MenuItemSelected action, Menu parent)
        {
            Text = sprite;
            Action = action; 
            Parent = parent;
            
            Text.TextColor = Color.White;
            Text.Scale = 1.0f;
            Text.Centered = Parent.Centered;
            selOffset = new Vector2(20, 0);
        }
        public override void Select()
        {
            if (Action != null)
            {
                Action();
            }
        }
        public override void Highlight()
        {
            Text.TextColor = Color.Red;
            Text.Position += selOffset;
        }
        public override void UnHighlight()
        {
            Text.TextColor = Color.White;
            Text.Position -= selOffset;
        }
    }
}
