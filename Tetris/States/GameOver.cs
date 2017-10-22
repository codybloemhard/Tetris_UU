using System;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tetris
{
    public class GameOver : GameState
    {
        private SpriteFont bigFont, mainFont;
        private Text titleText, scoreText, levelText, newHSText;
        private Button playAgain, gotoMenu;
        private bool newHS = false;
        private ParticleEmitter emitter;
        private Random rand;
        private float timer = 0f;

        public GameOver() { }
        
        public void Load(SpriteBatch batch)
        {
            newHS = Highscores.CheckHighScore((uint)DataManager.GetData<int>("score"));
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
            newHSText = new Text("NEW HIGHSCORE!", new Vector2(0, 8), new Vector2(16, 1));
            newHSText.colour = Color.Pink;
            playAgain = new Button("Play Again", "block", () => GameStateManager.RequestChange(new GameStateChange("game", CHANGETYPE.LOAD)),
            new Vector2(2, 4), new Vector2(4, 3));
            playAgain.SetupColours(Color.Gray, Color.White, Color.DarkGray, Color.Red);
            gotoMenu = new Button("Goto Menu", "block", () => GameStateManager.RequestChange(new GameStateChange("menu", CHANGETYPE.LOAD)),
            new Vector2(10, 4), new Vector2(4, 3));
            gotoMenu.SetupColours(Color.Gray, Color.White, Color.DarkGray, Color.Red);
            emitter = new ParticleEmitter("block", 400);
            rand = new Random();
        }
        
        public void Unload() { }

        public Color RandomColor()
        {
            Color[] colours = { Color.Cyan, Color.Blue, Color.Orange, Color.Yellow, Color.Green, Color.Purple, Color.Red };
            return colours[rand.Next(0, 6)];
        }

        public void Update(float time)
        {
            playAgain.Update();
            gotoMenu.Update();
            timer += time;
            if (timer > 0.2f)
            {
                timer = 0f;
                for (int j = 0; j < 20; j++)
                    emitter.Emit(new Vector2((j + 0.3f) * (16f/20f), -1), Vector2.One / 3.0f, Vector2.UnitY,
                    RandomColor(), 3.0f, 500, 1.0f, 1.0f, 1);
            }
            emitter.Update(time);
        }

        public void Draw(float time, SpriteBatch batch, GraphicsDevice device)
        {
            device.Clear(Color.Black);
            batch.Begin();
            {
                emitter.Draw(batch);
                titleText.Draw(batch, bigFont);
                scoreText.Draw(batch, mainFont);
                levelText.Draw(batch, mainFont);
                playAgain.Draw(batch, mainFont);
                gotoMenu.Draw(batch, mainFont);
                if(newHS) newHSText.Draw(batch, mainFont);
            }
            batch.End();
        }
    }
}