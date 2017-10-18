using System;
using Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Tetris
{
    public class Menu : GameState
    {
        private SpriteFont bigFont, mainFont;
        private Text titleText;
        private Button gotoGame, gotoHighscores;

        public Menu() { }
        
        public void Load(SpriteBatch batch)
        {
            bigFont = AssetManager.GetResource<SpriteFont>("menuFont");
            mainFont = AssetManager.GetResource<SpriteFont>("mainFont");
            titleText = new Text("Tetris", Vector2.Zero, new Vector2(16, 3));
            titleText.colour = Color.Red;
            gotoGame = new Button("Play", "block", () => GameStateManager.RequestChange(new GameStateChange("game", CHANGETYPE.LOAD)),
            new Vector2(2, 4), new Vector2(4, 3));
            gotoGame.SetupColours(Color.Gray, Color.White, Color.DarkGray, Color.Red);
            gotoHighscores = new Button("Highscores", "block", () => GameStateManager.RequestChange(new GameStateChange("highscores", CHANGETYPE.LOAD)),
            new Vector2(10, 4), new Vector2(4, 3));
            gotoHighscores.SetupColours(Color.Gray, Color.White, Color.DarkGray, Color.Red);
            Console.WriteLine(Highscores.Highscore);
        }

        public void Unload()
        {
            
        }

        public void Update(float time)
        {
            gotoGame.Update();
            gotoHighscores.Update();
        }

        public void Draw(float time, SpriteBatch batch, GraphicsDevice device)
        {
            device.Clear(Color.Black);
            batch.Begin();
            {
                titleText.Draw(batch, bigFont);
                gotoGame.Draw(batch, mainFont);
                gotoHighscores.Draw(batch, mainFont);
            }
            batch.End();
        }
    }
}