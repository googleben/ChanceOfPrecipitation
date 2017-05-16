using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace ChanceOfPrecipitation {
    internal class MenuOption {

        public delegate IGameState MakeState();

        public MakeState makeState;

        public string text;

        public MenuOption(string text, MakeState makeState) {
            this.makeState = makeState;
            this.text = text;
        }

    }

    internal abstract class Menu : IGameState {
        private readonly SpriteFont font;

        public static KeyboardState lastState = Keyboard.GetState();

        protected int index;

        protected List<MenuOption> options;

        protected readonly Menu fromMenu;

        float offset = 0;

        public Menu(Menu fromMenu) {
            font = Game1.fonts["MenuFont"];
            index = 0;
            options = new List<MenuOption>();
            this.fromMenu = fromMenu;
            GenerateOptions();
        }

        public void Draw(SpriteBatch sb) {
            var ypos = 20;
            var space = ypos;
            float y = 0;
            for (var i = 0; i < index; i++)
            {
                var opt = options[i];
                var m = font.MeasureString(opt.text);
                y = m.Y;
                ypos += space + (int)m.Y;
            }
            if (ypos > 720 - y) offset = ypos - (720 - y);
            else offset = 0;
            ypos = 2;
            for (var i = 0; i < options.Count; i++) {
                var opt = options[i];
                var m = font.MeasureString(opt.text);
                sb.DrawString(font, opt.text, new Vector2(Game1.BufferWidth / 2 - m.X / 2, ypos-offset), i == index ? Color.White : Color.Gray);
                ypos += space + (int)m.Y;
            }
        }

        public virtual IGameState Update() {
            var state = Keyboard.GetState();
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !lastState.IsKeyDown(Keys.Enter)) {
                lastState = state;
                return options[index].makeState.Invoke();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !lastState.IsKeyDown(Keys.Escape)) {
                lastState = state;
                if (fromMenu != null) return fromMenu;
                Game1.Instance.Quit();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && !lastState.IsKeyDown(Keys.Down)) {
                index++;
                if (index == options.Count) index = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up) && !lastState.IsKeyDown(Keys.Up)) {
                index--;
                if (index == -1) index = options.Count - 1;
            }

            lastState = state;

            return this;
        }

        public void RegenerateOptions() {
            options.Clear();
            GenerateOptions();
        }

        public virtual void GenerateOptions() {
            if (fromMenu == null) options.Add(new MenuOption("Exit", () =>
            {
                Game1.Instance.Quit();
                return this;
            }));
            else options.Add(new MenuOption("Back", () => fromMenu));
        }

    }

    internal class MainMenu : Menu {

        public MainMenu() : base(null) {}

        public new void Draw(SpriteBatch sb) {
            base.Draw(sb);
        }

        public new IGameState Update() {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Game1.Instance.Quit();
            return base.Update();
        }

        public override void GenerateOptions() {
            options.Add(new MenuOption("Play", () => Playing.Instance));
            options.Add(new MenuOption("Settings", () => new SettingsMenu(this)));
            options.Add(new MenuOption("Make", () => new EditorMenu(this)));
            base.GenerateOptions();
        }

    }

    internal class SettingsMenu : Menu {

        public SettingsMenu(Menu fromMenu) : base(fromMenu) {}

        public override void GenerateOptions() {
            var settings = Game1.Instance.settings;
            base.GenerateOptions();
            options.Add(new MenuOption($"Resolution: {Game1.Instance.settings.screenWidth}x{Game1.Instance.settings.screenHeight}", () => {
                var size = new Point(settings.screenWidth, settings.screenHeight);
                var index = Game1.resolutions.IndexOf(size);
                index++;
                if (index == Game1.resolutions.Count) index = 0;
                var r = Game1.resolutions[index];
                Game1.Instance.settings.screenWidth = r.X;
                Game1.Instance.settings.screenHeight = r.Y;
                RegenerateOptions();
                
                return this;
            }));
            options.Add(new MenuOption("Fullscreen: " + (settings.fullscreen ? "On" : "Off"), () => {
                Game1.Instance.settings.fullscreen = !settings.fullscreen;
                RegenerateOptions();
                return this;
            }));
            options.Add(new MenuOption("Apply", () => { Game1.Instance.ApplySettings(); return this; }));
        }

    }

    internal class EditorMenu : Menu {

        public EditorMenu(Menu fromMenu) : base(fromMenu) { }

        public override void GenerateOptions() {
            base.GenerateOptions();
            options.Add(new MenuOption("Load Level", () => new LoadMenu(this)));
            options.Add(new MenuOption("New Level", () => new NewMenu(this)));
        }

    }

    internal class LoadMenu : Menu {

        public LoadMenu(Menu fromMenu) : base(fromMenu) { }

        public override void GenerateOptions() {
            base.GenerateOptions();
            var levels = Directory.GetFiles("Content/Levels");
            foreach (string s in levels) {
                options.Add(new MenuOption(s.Substring("Content/Levels/".Length), () => {
                    Editor.Instance.LoadLevel(s);
                    return Editor.Instance;
                }));
            }
        }

    }

    internal class NewMenu : Menu {

        MenuOption header;
        MenuOption textField;
        MenuOption back;
        MenuOption enter;
        int backIndex;
        int enterIndex;

        public NewMenu(Menu fromMenu) : base(fromMenu) { }

        public override void GenerateOptions() {
            header = new MenuOption("Enter a file name", () => this);
            options.Add(header);
            textField = new MenuOption("_", () => this);
            options.Add(textField);
            enter = new MenuOption("Create", () => {
                if (textField.text.Length == 1) return this;
                Editor.currentFile = "Content/Levels/" + textField.text.Substring(0, textField.text.Length-1);
                Editor.Instance.objects.Clear();
                return Editor.Instance;
            });
            options.Add(enter);
            enterIndex = options.Count - 1;
            index = enterIndex;

            base.GenerateOptions();
            back = options[options.Count - 1];
            backIndex = options.Count - 1;
        }

        public override IGameState Update() {
            var state = Keyboard.GetState();
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !lastState.IsKeyDown(Keys.Enter)) {
                lastState = state;
                return options[index].makeState.Invoke();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !lastState.IsKeyDown(Keys.Escape)) {
                lastState = state;
                if (fromMenu != null) return fromMenu;
                Game1.Instance.Quit();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && !lastState.IsKeyDown(Keys.Down)) {
                if (index == backIndex) index = enterIndex;
                else index = backIndex;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up) && !lastState.IsKeyDown(Keys.Up)) {
                if (index == backIndex) index = enterIndex;
                else index = backIndex;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Back) && !lastState.IsKeyDown(Keys.Back)) {
                if (textField.text.Length > 1)
                    textField.text = textField.text.Substring(0, textField.text.Length - 2) + "_";
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemPeriod) && !lastState.IsKeyDown(Keys.OemPeriod)) {
                if (textField.text.Length > 1)
                    textField.text = textField.text.Substring(0, textField.text.Length - 1) + "._";
            }
            for (int i = (int)Keys.A; i<=(int)Keys.Z; i++) {
                if (Keyboard.GetState().IsKeyDown((Keys)i) && !lastState.IsKeyDown((Keys)i)) {
                    textField.text = textField.text.Substring(0, textField.text.Length - 1)+
                        (char)(i+(Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.LeftShift) ? 0 : 'a'-'A')) + "_";
                }
            }
            for (int i = (int)Keys.D0; i <= (int)Keys.D9; i++) {
                if (Keyboard.GetState().IsKeyDown((Keys)i) && !lastState.IsKeyDown((Keys)i)) {
                    textField.text = textField.text.Substring(0, textField.text.Length - 1) +
                        (char)(i) + "_";
                }
            }

            lastState = state;

            return this;
        }

    }
    
}
