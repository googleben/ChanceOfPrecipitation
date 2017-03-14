using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

        private int index;

        protected List<MenuOption> options;

        private readonly Menu fromMenu;

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

            for (var i = 0; i < options.Count; i++) {
                var opt = options[i];
                var m = font.MeasureString(opt.text);
                sb.DrawString(font, opt.text, new Vector2(Game1.BufferWidth / 2 - m.X / 2, ypos), i == index ? Color.White : Color.Gray);
                ypos += space + (int)m.Y;
            }
        }

        public IGameState Update() {
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
    
}
