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
            Editor.Instance.objects.Add(new Block(pos.X, pos.Y, sprite));
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
            foreach (Block o in Editor.Instance.objects) {
                if (o.type.Contains("Exit"))
                    file += "exit " + o.bounds.x + " " + o.bounds.y + " " + (o.type.Contains("Vert") ? "true" : "false") + "\n";
                else if (o.type.Contains("rope"))
                    file += "rope " + o.bounds.x + " " + o.bounds.y + " 10";
                else
                    file += o.type + " " + o.bounds.x + " " + o.bounds.y + "\n";
            }

            if (file.Length > 0)
                file = file.Substring(0, file.Length - 1);

            Console.WriteLine(file);
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

        public List<Block> objects;
        public List<EditorTool> tools;

        private static Editor instance;
        public static Editor Instance { get { if (instance == null) instance = new Editor(); return instance; } private set { instance = value; } }

        public EditorTool tool;

        public Editor() {
            objects = new List<Block>();
            state = lastState = Keyboard.GetState();
            mstate = mlastState = Mouse.GetState();

            tools = new List<EditorTool>();
            ToolGroup blocks = new ToolGroup(0, 720 - 32, 32, 32, "stage1_platform_middle");
            tools.Add(blocks);

            SaveTool savetool = new SaveTool(128, 720 - 32, 32, 32);
            tools.Add(savetool);

            EraserTool eraserTool = new EraserTool(64, 720 - 32, 32, 32);
            tools.Add(eraserTool);

            ToolGroup exits = new ToolGroup(32, 720 - 32, 32, 32, "HorExit");
            tools.Add(exits);
            
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

            BlockTool exit;
            exit = new BlockTool(32, 32, 32, 32, "HorExit");
            exits.Add(exit);
            tools.Add(exit);
            exit = new BlockTool(64, 32, 32, 32, "VertExit");
            exits.Add(exit);
            tools.Add(exit);

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
                if (!mlastState.LeftButton.HasFlag(ButtonState.Pressed)) foreach (EditorTool t in tools) {
                    if (t.enabled && t.bounds.Contains(pos)) {
                        tool = t;
                        t.PickedUp();
                        found = true;
                        break;
                    }
                }
                if (!found && tool != null) tool.OnClick(pos);
            }

            return this;
        }

        public void LoadLevel(string level) {
            objects.Clear();
            try {
                using (System.IO.StreamReader file = new System.IO.StreamReader(@"Content/Levels" + level + ".lvl")) {
                    int low = 0;
                    int high = 0;
                    int left = 0;
                    int right = 0;
                    while (!file.EndOfStream) {
                        string line = file.ReadLine();
                        var sp = line.Split(' ');
                        BlockTool t = new BlockTool(int.Parse(sp[1]), int.Parse(sp[2]), 32, 32, sp[0]);
                        var b = t.bounds;
                        if (b.X < left) left = b.X;
                        if (b.Y < low) low = b.Y;
                        if (b.X+32 > right) right = b.X+32;
                        if (b.Y+32 > high) high = b.Y+32;
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

    }
}
