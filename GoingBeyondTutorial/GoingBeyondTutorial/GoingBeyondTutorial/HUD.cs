using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace GoingBeyondTutorial
{
    class HUD
    {
        private int score;
        private int health;
        private int targetsRem;

        private Vector2 scorePosition;
        private Vector2 healthPosition;
        private Vector2 targHitPosition;
        private Vector2 gameStartPosition;
        private Vector2 gameOverPosition;
        private Vector2 labelOffset;

        private Rectangle hudBox;
        private Rectangle healthBox;
        private Rectangle gameOverBox;
        private Texture2D boxTexture;
        private SpriteFont scoreFont;

        public HUD()
        {
            hudBox = new Rectangle(0, Game1.BackBufferHeight - 50, Game1.BackBufferWidth, 50);
            gameOverBox = new Rectangle(Game1.BackBufferWidth / 3, Game1.BackBufferHeight / 3, Game1.BackBufferWidth / 3, Game1.BackBufferHeight / 3);
            scorePosition = new Vector2(Game1.BackBufferWidth / 10, Game1.BackBufferHeight - 45);
            targHitPosition = new Vector2(Game1.BackBufferWidth * 8 / 10, Game1.BackBufferHeight - 45);
            healthPosition = new Vector2(Game1.BackBufferWidth / 3, Game1.BackBufferHeight - 35);
            gameOverPosition = new Vector2(gameOverBox.Left + 20, gameOverBox.Top + 20);
            labelOffset = new Vector2(0, 10);
        }

        public void LoadContent(ContentManager Content)
        {
            scoreFont = Content.Load<SpriteFont>("score");
            boxTexture = Content.Load<Texture2D>("health");
        }

        public void Update(HudData data)
        {
            score = (int)data.Score;
            health = data.Health;
            targetsRem = World.TotalTargets - data.TargetsHit;
            healthBox = new Rectangle((int)healthPosition.X, (int)healthPosition.Y, health, boxTexture.Height);
        }
   
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw background
            spriteBatch.Draw(boxTexture, hudBox, Color.Black);

            // Draw data
            spriteBatch.DrawString(scoreFont, score.ToString(), scorePosition, Color.Red);
            spriteBatch.DrawString(scoreFont, targetsRem.ToString(), targHitPosition, Color.Red);
            spriteBatch.Draw(boxTexture, healthBox, Color.Red);

            // Draw labels
            spriteBatch.DrawString(scoreFont, "Score", scorePosition + (labelOffset*2), Color.Aqua);
            spriteBatch.DrawString(scoreFont, "Targets Remaining ", targHitPosition + (labelOffset*2), Color.Aqua);
            spriteBatch.DrawString(scoreFont, "Health ", healthPosition + labelOffset, Color.Aqua);
        }
        public void DrawGameOver(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(boxTexture, gameOverBox, Color.Black);
            spriteBatch.DrawString(scoreFont, "Game Over!", gameOverPosition, Color.White);
            spriteBatch.DrawString(scoreFont, "Final Score: " + score.ToString(), gameOverPosition + (labelOffset*4), Color.White);
            spriteBatch.DrawString(scoreFont, "Targets Hit: " + (World.TotalTargets - targetsRem).ToString(), gameOverPosition + (labelOffset * 8), Color.White);
            spriteBatch.DrawString(scoreFont, "Press ESC to return to the Main Menu", gameOverPosition + (labelOffset * 12), Color.White);
        }
        public void DrawGameStart(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(boxTexture, gameOverBox, Color.Black);
            spriteBatch.DrawString(scoreFont, "Hit targets and avoid buildings", gameOverPosition + (labelOffset * 2), Color.Red);
            spriteBatch.DrawString(scoreFont, "to score points", gameOverPosition + (labelOffset * 5), Color.Red);
            spriteBatch.DrawString(scoreFont, "Good Hunting!", gameOverPosition + (labelOffset * 8), Color.Red);
            spriteBatch.DrawString(scoreFont, "Press P to play", gameOverPosition + (labelOffset * 12), Color.White);
        }
    }
}
