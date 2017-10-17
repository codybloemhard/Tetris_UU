using System;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tetris
{
    public class GameOver : GameState
    {
        private SpriteFont bigFont, mainFont;
        private Text titleText, scoreText, levelText;
        private Button playAgain;

        public GameOver() { }

        public void Load(SpriteBatch batch)
        {
            bigFont = AssetManager.GetResource<SpriteFont>("menuFont");
            mainFont = AssetManager.GetResource<SpriteFont>("mainFont");
            titleText = new Text("Game Over!", Vector2.Zero, new Vector2(16, 3));
            titleText.colour = Color.Red;
            scoreText = new Text("Your Score: " + DataManager.GetData<int>("score"), 
                new Vector2(0, 3), new Vector2(16, 1));
            scoreText.colour = new Color(0, 255, 0);
            levelText = new Text("Level: " + (DataManager.GetData<byte>("level") + 1),
                new Vector2(0, 4), new Vector2(16, 1));
            levelText.colour = Color.Blue;
            playAgain = new Button("Play Again!", "block", () => GameStateManager.RequestChange(new GameStateChange("game", CHANGETYPE.LOAD)), new Vector2(6, 5), new Vector2(4, 2));
            playAgain.SetupColours(Color.Gray, Color.White, Color.DarkGray, Color.Red);
        }

        public void Unload() { }

        public void Update(float time)
        {
            playAgain.Update();
        }

        public void Draw(float time, SpriteBatch batch, GraphicsDevice device)
        {
            device.Clear(Color.Black);
            batch.Begin();
            {
                titleText.Draw(batch, bigFont);
                scoreText.Draw(batch, mainFont);
                levelText.Draw(batch, mainFont);
                playAgain.Draw(batch, mainFont);
            }
            batch.End();
        }
    }
}