using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;
using System.IO;

namespace ChanceOfPrecipitation {

    public class Game1 : Game {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public Settings settings;

        public static Dictionary<String, SpriteFont> fonts = new Dictionary<string, SpriteFont>();

        public static Game1 Instance { get; private set; }

        private RenderTarget2D renderTarget;

        public const int BufferWidth = 1280;
        public const int BufferHeight = 720;

        private IGameState state;
        public static List<Point> resolutions;

        private int screenWidth;
        private int screenHeight;

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

            TextureManager.textures["Square"] = Content.Load<Texture2D>("Square");
            TextureManager.textures["Numbers"] = Content.Load<Texture2D>("Numbers");
            TextureManager.blocks["Square"] = new BlockInfo("Square", new Rectangle(0, 0, 16, 16));

            TextureManager.blocks["1"] = new BlockInfo("Numbers", new Rectangle(0, 0, 3, 7));
            TextureManager.blocks["0"] = new BlockInfo("Numbers", new Rectangle(34, 0, 5, 7));
            for (var i = 2; i <= 9; i++)
                TextureManager.blocks[i.ToString()] = new BlockInfo("Numbers", new Rectangle(2 + (i - 2) * 4, 0, 5, 7));

            TextureManager.textures["Numbers"] = Content.Load<Texture2D>("Numbers");
            TextureManager.blocks["1"] = new BlockInfo("Numbers", new Rectangle(0, 0, 5, 17));
            TextureManager.blocks["0"] = new BlockInfo("Numbers", new Rectangle(93, 0, 11, 17));
            TextureManager.blocks["$"] = new BlockInfo("Numbers", new Rectangle(104, 0, 11, 17));
            for (var i = 2; i <= 9; i++)
                TextureManager.blocks[i.ToString()] = new BlockInfo("Numbers", new Rectangle(5 + (i - 2) * 11, 0, 11, 17));

            const int scale = 2;
            TextureManager.textures["platform_tileset_stage1"] = Content.Load<Texture2D>("platform_tileset_stage1");
            TextureManager.blocks["stage1_platform_top_left"] =        new BlockInfo("platform_tileset_stage1", new Rectangle(0, 0, 16, 16),   scale);
            TextureManager.blocks["stage1_platform_top_middle"] =      new BlockInfo("platform_tileset_stage1", new Rectangle(16, 0, 16, 16),  scale);
            TextureManager.blocks["stage1_platform_top_right"] =       new BlockInfo("platform_tileset_stage1", new Rectangle(32, 0, 16, 16),  scale);
            TextureManager.blocks["stage1_platform_middle_left"] =     new BlockInfo("platform_tileset_stage1", new Rectangle(0, 16, 16, 16),  scale);
            TextureManager.blocks["stage1_platform_middle"] =          new BlockInfo("platform_tileset_stage1", new Rectangle(16, 16, 16, 16), scale); 
            TextureManager.blocks["stage1_platform_middle_right"] =    new BlockInfo("platform_tileset_stage1", new Rectangle(32, 16, 16, 16), scale); 
            TextureManager.blocks["stage1_platform_bottom_left"] =     new BlockInfo("platform_tileset_stage1", new Rectangle(0, 32, 16, 16),  scale); 
            TextureManager.blocks["stage1_platform_bottom_middle"] =   new BlockInfo("platform_tileset_stage1", new Rectangle(16, 32, 16, 16), scale); 
            TextureManager.blocks["stage1_platform_bottom_right"] =    new BlockInfo("platform_tileset_stage1", new Rectangle(32, 32, 16, 16), scale);

            TextureManager.textures["Lazer"] = Content.Load<Texture2D>("LazerTileset");

            TextureManager.textures["Items"] = Content.Load<Texture2D>("Items");

            TextureManager.blocks["GreenCanister"] = new BlockInfo("Items", new Rectangle(0, 0, 32, 32));
            TextureManager.blocks["RedCanister"] = new BlockInfo("Items", new Rectangle(32, 0, 32, 32));
            TextureManager.blocks["YellowCanister"] = new BlockInfo("Items", new Rectangle(64, 0, 32, 32));

            TextureManager.textures["ItemStand"] = Content.Load<Texture2D>("ItemStand");
            TextureManager.blocks["shop"] = new BlockInfo("", new Rectangle(0, 0, 64, 48));
            TextureManager.blocks["stand"] = new BlockInfo("ItemStand", new Rectangle(0, 0, 16, 48));

            TextureManager.textures["coin"] = Content.Load<Texture2D>("coin");
            TextureManager.blocks["coin"] = new BlockInfo("coin", new Rectangle(0, 0, 8, 8), 1.5f);

            TextureManager.textures["letters"] = Content.Load<Texture2D>("letters");
            TextureManager.blocks[" "] = new BlockInfo("letters", new Rectangle(286, 0, 11, 17));
            TextureManager.blocks["-"] = new BlockInfo("letters", new Rectangle(297, 0, 11, 17));

            for (var i = 0; i < 26; i++)
                TextureManager.blocks[Convert.ToChar(i + 97).ToString()] = new BlockInfo("letters", new Rectangle(i * 11, 0, 11, 17));
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
            Exit();
        }

    }

    internal interface IGameState {
        void Draw(SpriteBatch sb);
        IGameState Update();
    }

}
