using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            foreach (var d in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes) {
                var r = new Point(d.Width, d.Height);
                if (!resolutions.Contains<Point>(r) && d.AspectRatio == 16f / 9f) resolutions.Add(r);
            }
            resolutions.Sort((a, b) => a.X - b.X);
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            var ser = new XmlSerializer(typeof(Settings));
            Stream f = File.OpenRead(@"Content/Settings.xml");
            settings = (Settings)ser.Deserialize(f);
            f.Close();
            fonts.Add("MenuFont", Content.Load<SpriteFont>("MenuFont"));

            TextureManager.Textures["Square"] = Content.Load<Texture2D>("Square");
            TextureManager.Textures["Numbers"] = Content.Load<Texture2D>("Numbers");
            TextureManager.Sources["Square"] = new Rectangle(0, 0, 16, 16);

            TextureManager.Sources["1"] = new Rectangle(0, 0, 3, 7);
            TextureManager.Sources["0"] = new Rectangle(34, 0, 5, 7);
            for (var i = 2; i < 9; i++)
                TextureManager.Sources[(i).ToString()] = new Rectangle(2 + ((i - 2) * 4), 0, 5, 7);

            TextureManager.Textures["platform_tileset_stage1"] = Content.Load<Texture2D>("platform_tileset_stage1");
            TextureManager.Sources["stage1_platform_top_left"] =        new Rectangle(0, 0, 16, 16);
            TextureManager.Sources["stage1_platform_top_middle"] =      new Rectangle(16, 0, 16, 16);
            TextureManager.Sources["stage1_platform_top_right"] =       new Rectangle(32, 0, 16, 16);
            TextureManager.Sources["stage1_platform_middle_left"] =     new Rectangle(0, 16, 16, 16);
            TextureManager.Sources["stage1_platform_middle"] =          new Rectangle(16, 16, 16, 16);
            TextureManager.Sources["stage1_platform_middle_right"] =    new Rectangle(32, 16, 16, 16);
            TextureManager.Sources["stage1_platform_bottom_left"] =     new Rectangle(0, 32, 16, 16);
            TextureManager.Sources["stage1_platform_bottom_middle"] =   new Rectangle(16, 32, 16, 16);
            TextureManager.Sources["stage1_platform_bottom_right"] =    new Rectangle(32, 32, 16, 16);
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
            var set = new XmlSerializer(typeof(Settings));
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

            // Avoid antialiasing
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
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
