using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml.Serialization;
using System.IO;

namespace ChanceOfPrecipitation {

    public class Game1 : Microsoft.Xna.Framework.Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Settings settings;

        public static Dictionary<String, SpriteFont> fonts = new Dictionary<string, SpriteFont>();

        public static Game1 Instance { get; private set; }

        RenderTarget2D renderTarget;

        public const int BufferWidth = 1280;
        public const int BufferHeight = 720;

        GameState state;
        public static List<Point> resolutions;

        int screenWidth;
        int screenHeight;

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Instance = this;
        }
        protected override void Initialize() {
            base.Initialize();
            ApplySettings();
            state = new MainMenu();
            renderTarget = new RenderTarget2D(GraphicsDevice, 1280, 720);
            resolutions = new List<Point>();
            foreach (DisplayMode d in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes) {
                Point r = new Point(d.Width, d.Height);
                if (!resolutions.Contains<Point>(r) && d.AspectRatio == 16f / 9f) resolutions.Add(r);
            }
            resolutions.Sort((a, b) => a.X - b.X);
        }
        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            XmlSerializer ser = new XmlSerializer(typeof(Settings));
            Stream f = File.OpenRead(@"Content/Settings.xml");
            settings = (Settings)ser.Deserialize(f);
            f.Close();
            fonts.Add("MenuFont", Content.Load<SpriteFont>("MenuFont"));

        }
        protected override void UnloadContent() {
        }

        public void ApplySettings() {
            if (settings == null) return;
            graphics.PreferredBackBufferHeight = settings.screenHeight;
            graphics.PreferredBackBufferWidth = settings.screenWidth;
            screenHeight = settings.screenHeight;
            screenWidth = settings.screenWidth;
            graphics.IsFullScreen = settings.fullscreen;
            graphics.ApplyChanges();

            SaveSettings();
        }

        public void SaveSettings() {
            XmlSerializer set = new XmlSerializer(typeof(Settings));
            File.Delete(@"Content/Settings.xml");
            Stream f = File.OpenWrite(@"Content/Settings.xml");
            set.Serialize(f, settings);
            f.Close();
        }

        protected override void Update(GameTime gameTime) {
            state = state.Update();

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.SetRenderTarget(renderTarget);

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            state.Draw(spriteBatch);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.Draw(renderTarget, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            base.Draw(gameTime);
        }

        public void Quit() {
            this.Exit();
        }

    }

    interface GameState {
        void Draw(SpriteBatch sb);
        GameState Update();
    }

}
