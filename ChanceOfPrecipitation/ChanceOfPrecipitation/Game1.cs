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

        public static Dictionary<string, SpriteFont> fonts = new Dictionary<string, SpriteFont>();

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
            fonts.Add("MenuFont", Content.Load<SpriteFont>("Fonts/MenuFont"));

            TextureManager.textures["Square"] = Content.Load<Texture2D>("Spritesheets/Square");
            TextureManager.textures["Numbers"] = Content.Load<Texture2D>("Fonts/Numbers");
            TextureManager.blocks["Square"] = new TextureInfo("Spritesheets/Square", new Rectangle(0, 0, 16, 16));

            TextureManager.blocks["1"] = new TextureInfo("Numbers", new Rectangle(0, 0, 3, 7));
            TextureManager.blocks["0"] = new TextureInfo("Numbers", new Rectangle(34, 0, 5, 7));
            for (var i = 2; i <= 9; i++)
                TextureManager.blocks[i.ToString()] = new TextureInfo("Numbers", new Rectangle(2 + (i - 2) * 4, 0, 5, 7));

            TextureManager.textures["Numbers"] = Content.Load<Texture2D>("Fonts/Numbers");
            TextureManager.blocks["1"] = new TextureInfo("Numbers", new Rectangle(0, 0, 5, 17));
            TextureManager.blocks["0"] = new TextureInfo("Numbers", new Rectangle(93, 0, 11, 17));
            TextureManager.blocks["$"] = new TextureInfo("Numbers", new Rectangle(104, 0, 11, 17));
            for (var i = 2; i <= 9; i++)
                TextureManager.blocks[i.ToString()] = new TextureInfo("Numbers", new Rectangle(5 + (i - 2) * 11, 0, 11, 17));

            const float scale = 2;
            TextureManager.textures["platform_tileset_stage1"] = Content.Load<Texture2D>("Tilesets/platform_tileset_stage1");
            TextureManager.blocks["stage1_platform_top_left"] =        new TextureInfo("platform_tileset_stage1", new Rectangle(0, 0, 16, 16),   scale);
            TextureManager.blocks["stage1_platform_top_middle"] =      new TextureInfo("platform_tileset_stage1", new Rectangle(16, 0, 16, 16),  scale);
            TextureManager.blocks["stage1_platform_top_right"] =       new TextureInfo("platform_tileset_stage1", new Rectangle(32, 0, 16, 16),  scale);
            TextureManager.blocks["stage1_platform_middle_left"] =     new TextureInfo("platform_tileset_stage1", new Rectangle(0, 16, 16, 16),  scale);
            TextureManager.blocks["stage1_platform_middle"] =          new TextureInfo("platform_tileset_stage1", new Rectangle(16, 16, 16, 16), scale);
            TextureManager.blocks["stage1_platform_middle_right"] =    new TextureInfo("platform_tileset_stage1", new Rectangle(32, 16, 16, 16), scale);
            TextureManager.blocks["stage1_platform_bottom_left"] =     new TextureInfo("platform_tileset_stage1", new Rectangle(0, 32, 16, 16),  scale);
            TextureManager.blocks["stage1_platform_bottom_middle"] =   new TextureInfo("platform_tileset_stage1", new Rectangle(16, 32, 16, 16), scale);
            TextureManager.blocks["stage1_platform_bottom_right"] =    new TextureInfo("platform_tileset_stage1", new Rectangle(32, 32, 16, 16), scale);

            TextureManager.textures["Lazer"] = Content.Load<Texture2D>("Tilesets/LazerTileset");

            TextureManager.textures["Items"] = Content.Load<Texture2D>("Spritesheets/Items");

            TextureManager.blocks["GreenCanister"] = new TextureInfo("Items", new Rectangle(0, 0, 32, 32));
            TextureManager.blocks["RedCanister"] = new TextureInfo("Items", new Rectangle(32, 0, 32, 32));
            TextureManager.blocks["YellowCanister"] = new TextureInfo("Items", new Rectangle(64, 0, 32, 32));

            TextureManager.textures["ItemStand"] = Content.Load<Texture2D>("Spritesheets/ItemStand");
            TextureManager.blocks["shop"] = new TextureInfo("", new Rectangle(0, 0, 64, 48));
            TextureManager.blocks["stand"] = new TextureInfo("ItemStand", new Rectangle(0, 0, 16, 48));

            TextureManager.textures["coin"] = Content.Load<Texture2D>("Spritesheets/coin");
            TextureManager.blocks["coin"] = new TextureInfo("coin", new Rectangle(0, 0, 8, 8), 1.5f);

            TextureManager.textures["letters"] = Content.Load<Texture2D>("Fonts/letters");
            TextureManager.blocks[" "] = new TextureInfo("letters", new Rectangle(286, 0, 11, 17));
            TextureManager.blocks["-"] = new TextureInfo("letters", new Rectangle(297, 0, 11, 17));

            TextureManager.textures["portal"] = Content.Load<Texture2D>("Spritesheets/Portal");

            for (var i = 0; i < 4; i++)
                TextureManager.blocks["portal" + (i + 1)] = new TextureInfo("portal", new Rectangle(0, i * 32, 32, 32), scale / 2) { Frames = 3, Delay= 10 };

            TextureManager.textures["player"] = Content.Load<Texture2D>("Spritesheets/Player");
            TextureManager.blocks["playerIdle"] = new TextureInfo("player", new Rectangle(0, 0, 15, 32)) { Frames = 6, Delay = 10 };
            TextureManager.blocks["playerWalking"] = new TextureInfo("player", new Rectangle(0, 32, 15, 32)) { Frames = 8, Delay = 2 };

            TextureManager.textures["enemy"] = Content.Load<Texture2D>("Spritesheets/Enemy");
            TextureManager.blocks["enemy"] = new TextureInfo("enemy", new Rectangle(0, 0, 16, 32));

            TextureManager.textures["rope"] = Content.Load<Texture2D>("Spritesheets/Rope");
            TextureManager.blocks["ropeTop"] = new TextureInfo("rope", new Rectangle(0, 0, 4, 32));
            TextureManager.blocks["ropeMid"] = new TextureInfo("rope", new Rectangle(4, 0, 4, 32));
            TextureManager.blocks["ropeBot"] = new TextureInfo("rope", new Rectangle(8, 0, 4, 32));

            TextureManager.textures["SaveTool"] = Content.Load<Texture2D>("Tools/SaveTool");
            TextureManager.blocks["SaveTool"] = new TextureInfo("SaveTool", new Rectangle(0, 0, 32, 32));

            TextureManager.textures["Exit"] = Content.Load<Texture2D>("Tools/Exit");
            TextureManager.blocks["VertExit"] = new TextureInfo("Exit", new Rectangle(0, 0, 32, 32));
            TextureManager.blocks["HorExit"] = new TextureInfo("Exit", new Rectangle(32, 0, 32, 32));

            TextureManager.textures["EraserTool"] = Content.Load<Texture2D>("Tools/EraserTool");
            TextureManager.blocks["EraserTool"] = new TextureInfo("EraserTool", new Rectangle(0, 0, 32, 32));

            TextureManager.textures["Plus"] = Content.Load<Texture2D>("Tools/Plus");
            TextureManager.blocks["Plus"] = new TextureInfo("Plus", new Rectangle(0, 0, 32, 32));
            TextureManager.textures["Minus"] = Content.Load<Texture2D>("Tools/Minus");
            TextureManager.blocks["Minus"] = new TextureInfo("Minus", new Rectangle(0, 0, 32, 32));
            TextureManager.textures["ClearTool"] = Content.Load<Texture2D>("Tools/ClearTool");
            TextureManager.blocks["ClearTool"] = new TextureInfo("ClearTool", new Rectangle(0, 0, 32, 32));

            for (var i = 0; i < 26; i++)
                TextureManager.blocks[Convert.ToChar(i + 97).ToString()] = new TextureInfo("letters", new Rectangle(i * 11, 0, 11, 17));
            new Level(File.ReadAllText("Content/Levels/level.txt"), "level");
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
