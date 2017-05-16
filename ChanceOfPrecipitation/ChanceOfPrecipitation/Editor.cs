using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Text.RegularExpressions;
using System.IO;

namespace ChanceOfPrecipitation {

    public abstract class EditorTool {

        public Rectangle bounds;
        public string sprite;
        public bool enabled = true;
        TextureInfo info;
        Texture2D texture;

        public EditorTool(int x, int y, int width, int height, string sprite) {
            this.bounds = new Rectangle(x, y, width, height);
            this.sprite = sprite;
            info = TextureManager.blocks[sprite];
            texture = TextureManager.textures[info.texName];
        }

        public void Draw(SpriteBatch sb) {
            sb.Draw(texture, bounds, info.src, Color.White);
        }

        public abstract void OnClick(Point clickPos);

        public virtual void PickedUp() { }

    }

    public class ToolGroup : EditorTool {

        bool open = false;

        List<EditorTool> tools = new List<EditorTool>();

        public ToolGroup(int x, int y, int width, int height, string sprite) : base(x, y, width, height, sprite) { }

        public override void OnClick(Point clickPos) {
        }

        public override void PickedUp() {
            open = !open;
            foreach (EditorTool t in tools) t.enabled = open;
        }

        public void Add(EditorTool tool) {
            tools.Add(tool);
            tool.bounds.X = this.bounds.X;
            tool.bounds.Y = this.bounds.Y - tools.Count * 32;
            tool.enabled = open;
        }

    }

    public class BlockTool : EditorTool {
        public BlockTool(int x, int y, int width, int height, string sprite) : base(x, y, width, height, sprite) {}

        public override void OnClick(Point clickPos) {
            Point pos = new Point(clickPos.X - (clickPos.X % 32) + Editor.viewport.X, clickPos.Y - (clickPos.Y % 32) + Editor.viewport.Y);
            for (int i = Editor.Instance.objects.Count-1; i>=0; i--) {
                var b = (Rectangle) Editor.Instance.objects[i].bounds;
                if (b.X == pos.X && b.Y == pos.Y) Editor.Instance.objects.RemoveAt(i);
            }
            if (sprite=="ropeMid") Editor.Instance.objects.Add(new RopeBlock(pos.X, pos.Y));
            else Editor.Instance.objects.Add(new EditorBlock(pos.X, pos.Y, sprite));
        }

    }

    public class EraserTool : EditorTool {
        public EraserTool(int x, int y, int width, int height) : base(x, y, width, height, "EraserTool") { }

        public override void OnClick(Point clickPos) {
            Point pos = new Point(clickPos.X - (clickPos.X % 32) + Editor.viewport.X, clickPos.Y - (clickPos.Y % 32) + Editor.viewport.Y);

            for (int i = Editor.Instance.objects.Count - 1; i >= 0; i--) {
                var b = (Rectangle)Editor.Instance.objects[i].bounds;
                if (b.X == pos.X && b.Y == pos.Y) Editor.Instance.objects.RemoveAt(i);
            }
        }
    }

    public class SaveTool : EditorTool {
        public SaveTool(int x, int y, int width, int height) : base(x, y, width, height, "SaveTool") { }

        public override void OnClick(Point clickPos) {
            
        }

        public override void PickedUp() {
            string file = "";
            foreach (EditorBlock o in Editor.Instance.objects) {
                if (o.type.Contains("Exit"))
                    file += "exit " + o.bounds.X + " " + o.bounds.Y + " " + (o.type.Contains("Vert") ? "true" : "false") + "\n";
                else if (o.type.Contains("rope"))
                    file += "rope " + o.bounds.X + " " + o.bounds.Y + " " + (o as RopeBlock).length + "\n";
                else
                    file += o.type + " " + o.bounds.X + " " + o.bounds.Y + "\n";
            }

            if (file.Length > 0)
                file = file.Substring(0, file.Length - 1);

            File.WriteAllText(Editor.currentFile, file);

            Console.WriteLine(file);
        }

    }

    public class ClearTool : EditorTool {
        public ClearTool(int x, int y, int width, int height) : base(x, y, width, height, "ClearTool") { }

        public override void OnClick(Point clickPos) {

        }

        public override void PickedUp() {
            Editor.Instance.objects.Clear();
        }
    }

    public class RopeModTool : EditorTool {

        bool add;

        public RopeModTool(int x, int y, int width, int height, bool add) : base(x, y, width, height, add ? "Plus" : "Minus") {
            this.add = add;
        }

        public override void OnClick(Point clickPos) {
            if (Editor.mlastState.LeftButton == ButtonState.Pressed) return;
            Point pos = new Point(clickPos.X - (clickPos.X % 32) + Editor.viewport.X, clickPos.Y - (clickPos.Y % 32) + Editor.viewport.Y);

            for (int i = Editor.Instance.objects.Count - 1; i >= 0; i--) {
                var x = Editor.Instance.objects[i];
                var b = x.bounds;
                if (b.X == pos.X && b.Y == pos.Y && x is RopeBlock) (x as RopeBlock).length += add ? 1 : -1; 
            }
        }

        public override void PickedUp() {
            string file = "";
            foreach (EditorBlock o in Editor.Instance.objects) {
                if (o.type.Contains("Exit"))
                    file += "exit " + o.bounds.X + " " + o.bounds.Y + " " + (o.type.Contains("Vert") ? "true" : "false") + "\n";
                else if (o.type.Contains("rope"))
                    file += "rope " + o.bounds.X + " " + o.bounds.Y + " 10\n";
                else
                    file += o.type + " " + o.bounds.X + " " + o.bounds.Y + "\n";
            }

            if (file.Length > 0)
                file = file.Substring(0, file.Length - 1);

            File.WriteAllText(Editor.currentFile, file);

            Console.WriteLine(file);
        }
    }

    class EditorBlock : GameObject
    {

        public Rectangle bounds;
        public string type;
        protected readonly Texture2D texture;
        protected readonly TextureInfo info;

        public EditorBlock(int x, int y, string type)
        {
            this.type = type;

            info = TextureManager.blocks[type];
            texture = TextureManager.textures[info.texName];
            bounds = new Rectangle(x, y, (int)(info.src.Width * info.scale), (int)(info.src.Height * info.scale));
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, new Rectangle(bounds.X - Editor.viewport.X, bounds.Y - Editor.viewport.Y, bounds.Width, bounds.Height), info.src, Color.White);
        }

        public override void Update(List<GameObject> objects) {}
    }

    class RopeBlock : EditorBlock {

        public int length;

        public RopeBlock(int x, int y) : this(x, y, 1) { }
        public RopeBlock(int x, int y, int length) : base(x, y, "ropeMid") {
            this.length = length;
        }

        public override void Draw(SpriteBatch sb) {
            if (length < 1) length = 1;
            for (int i = 0; i< length; i++) sb.Draw(texture, new Rectangle(bounds.X - Editor.viewport.X, bounds.Y - Editor.viewport.Y + (i*32), bounds.Width, bounds.Height), info.src, Color.White);
        }

    }

    class Editor : IGameState {

        const int screenWidth = 1280;
        const int screenHeight = 720;

        public static Rectangle viewport;

        public static Texture2D background;
        public static Texture2D sprites;

        public static Texture2D playerSheet;

        ContentManager content;

        public static KeyboardState state;
        public static KeyboardState lastState;

        public static MouseState mstate;
        public static MouseState mlastState;

        public List<EditorBlock> objects;
        public List<EditorTool> tools;

        private static Editor instance;
        public static Editor Instance { get { if (instance == null) instance = new Editor(); return instance; } private set { instance = value; } }

        public static bool InstanceExists => instance != null;

        public EditorTool tool;

        public static string currentFile;

        public Editor() {
            objects = new List<EditorBlock>();
            state = lastState = Keyboard.GetState();
            mstate = mlastState = Mouse.GetState();

            tools = new List<EditorTool>();
            ToolGroup blocks = new ToolGroup(0, 720 - 32, 32, 32, "stage1_platform_middle");
            tools.Add(blocks);

            SaveTool savetool = new SaveTool(128, 720 - 32, 32, 32);
            tools.Add(savetool);
            ClearTool cleartool = new ClearTool(96, 720 - 32, 32, 32);
            tools.Add(cleartool);
            
            BlockTool block;
            block = new BlockTool(0, 0, 32, 32, "stage1_platform_top_left");
            blocks.Add(block);
            tools.Add(block);
            block = new BlockTool(0, 0, 32, 32, "stage1_platform_top_middle");
            blocks.Add(block);
            tools.Add(block);
            block = new BlockTool(0, 0, 32, 32, "stage1_platform_top_right");
            blocks.Add(block);
            tools.Add(block);
            block = new BlockTool(0, 0, 32, 32, "stage1_platform_middle_left");
            blocks.Add(block);
            tools.Add(block);
            block = new BlockTool(0, 0, 32, 32, "stage1_platform_middle");
            blocks.Add(block);
            tools.Add(block);
            block = new BlockTool(0, 0, 32, 32, "stage1_platform_middle_right");
            blocks.Add(block);
            tools.Add(block);
            block = new BlockTool(0, 0, 32, 32, "stage1_platform_bottom_left");
            blocks.Add(block);
            tools.Add(block);
            block = new BlockTool(0, 0, 32, 32, "stage1_platform_bottom_middle");
            blocks.Add(block);
            tools.Add(block);
            block = new BlockTool(0, 0, 32, 32, "stage1_platform_bottom_right");
            blocks.Add(block);
            tools.Add(block);
            block = new BlockTool(14, 0, 4, 32, "ropeMid");
            blocks.Add(block);
            tools.Add(block);

            ToolGroup exits = new ToolGroup(32, 720 - 32, 32, 32, "HorExit");
            tools.Add(exits);

            BlockTool exit;
            exit = new BlockTool(32, 32, 32, 32, "HorExit");
            exits.Add(exit);
            tools.Add(exit);
            exit = new BlockTool(64, 32, 32, 32, "VertExit");
            exits.Add(exit);
            tools.Add(exit);

            ToolGroup rope = new ToolGroup(64, 720 - 32, 32, 32, "ropeMid");
            tools.Add(rope);

            block = new BlockTool(14, 0, 4, 32, "ropeMid");
            rope.Add(block);
            tools.Add(block);
            RopeModTool mod = new RopeModTool(0, 0, 32, 32, false);
            rope.Add(mod);
            tools.Add(mod);
            mod = new RopeModTool(0, 0, 32, 32, true);
            rope.Add(mod);
            tools.Add(mod);

            viewport = new Rectangle(0, 0, 1280, 720);
        }

        public void Draw(SpriteBatch sb) {
            if (!Game1.Instance.IsMouseVisible) Game1.Instance.IsMouseVisible = true;
            foreach (var d in objects) d.Draw(sb);
            foreach (var t in tools) if (t.enabled) t.Draw(sb);
        }

        public IGameState Update() {
            lastState = state;
            state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Escape)) {
                Menu.lastState = state;
                Game1.Instance.IsMouseVisible = false;
                return new MainMenu();
            }

            if (state.IsKeyDown(Keys.Left) && !lastState.IsKeyDown(Keys.Left)) {
                viewport.X -= 32;
            }
            if (state.IsKeyDown(Keys.Right) && !lastState.IsKeyDown(Keys.Right)) {
                viewport.X += 32;
            }
            if (state.IsKeyDown(Keys.Up) && !lastState.IsKeyDown(Keys.Up)) {
                viewport.Y -= 32;
            }
            if (state.IsKeyDown(Keys.Down) && !lastState.IsKeyDown(Keys.Down)) {
                viewport.Y += 32;
            }

            mlastState = mstate;
            mstate = Mouse.GetState();
            if (mstate.LeftButton.HasFlag(ButtonState.Pressed)) {
                Point pos = new Point((int)(((float)mstate.X / Game1.Instance.settings.screenWidth) * 1280), (int)(((float)mstate.Y / Game1.Instance.settings.screenHeight) * 720));
                bool found = false;
                foreach (EditorTool t in tools) {
                    if (t.enabled && t.bounds.Contains(pos)) {
                        tool = t;
                        if (!mlastState.LeftButton.HasFlag(ButtonState.Pressed)) t.PickedUp();
                        found = true;
                        break;
                    }
                }
                if (!found && tool != null) tool.OnClick(pos);
            }
            if (mstate.RightButton.HasFlag(ButtonState.Pressed))
            {
                Point clickPos = new Point((int)(((float)mstate.X / Game1.Instance.settings.screenWidth) * 1280), (int)(((float)mstate.Y / Game1.Instance.settings.screenHeight) * 720));
                Point pos = new Point(clickPos.X - (clickPos.X % 32) + viewport.X, clickPos.Y - (clickPos.Y % 32) + viewport.Y);
                for (int i = objects.Count - 1; i >= 0; i--)
                {
                    var b = objects[i].bounds;
                    if (b.X == pos.X && b.Y == pos.Y) objects.RemoveAt(i);
                }
            }

            return this;
        }

        public void LoadLevel(string filename) {
            objects.Clear();
            currentFile = filename;
            var raw = File.ReadAllText(filename).Trim();
            var split = Regex.Split(raw, "\\s+");
            var scanner = split.Select<string, Func<Type, object>>((string s) => {
                return t =>
                (s as IConvertible).ToType(t, System.Globalization.CultureInfo.InvariantCulture);
            }).GetEnumerator();
            while (scanner.MoveNext()) {
                string type = (string)scanner.Current(typeof(string));
                Console.WriteLine(type);
                scanner.MoveNext();
                float x = (float)scanner.Current(typeof(float));
                scanner.MoveNext();
                float y = (float)scanner.Current(typeof(float));

                /*if (type == "shop") {
                    scanner.MoveNext();
                    string item1 = (string)scanner.Current(typeof(string));
                    scanner.MoveNext();
                    string item2 = (string)scanner.Current(typeof(string));
                    scanner.MoveNext();
                    string item3 = (string)scanner.Current(typeof(string));
                    blocks.Add(new ShopPlacementInfo(x, y, item1, item2, item3));
                }
                else if (type == "player") {
                    blocks.Add(new PlayerPlacementInfo(x, y));
                }
                else if (type == "portal") {
                    blocks.Add(new PortalPlacementInfo(x, y));
                }*/
                if (type == "rope") {
                    scanner.MoveNext();
                    int length = (int)scanner.Current(typeof(int));
                    objects.Add(new RopeBlock((int)x, (int)y, length));
                }
                else if (type == "exit") {
                    scanner.MoveNext();
                    bool vertical = (bool)scanner.Current(typeof(bool));
                    objects.Add(new EditorBlock((int)x, (int)y, vertical ? "VertExit" : "HorExit"));
                }
                else {
                    objects.Add(new EditorBlock((int)x, (int)y, type));
                }

            }
            scanner.Dispose();
        }

    }
}
