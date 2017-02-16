using System;
using System.Collections.Generic;
using System.IO;
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

    class MenuOption {

        public delegate GameState MakeState();

        public MakeState makeState;

        public String text;

        public MenuOption(String text, MakeState makeState) {
            this.makeState = makeState;
            this.text = text;
        }

    }

    abstract class Menu : GameState {

        SpriteFont font;

        public static KeyboardState lastState = Keyboard.GetState();

        int index;

        protected List<MenuOption> options;

        Menu fromMenu;

        public Menu(Menu fromMenu) {
            this.font = Game1.fonts["MenuFont"];
            index = 0;
            options = new List<MenuOption>();
            this.fromMenu = fromMenu;
            generateOptions();
        }

        public void Draw(SpriteBatch sb) {
            int ypos = 20;
            int space = ypos;

            for (int i = 0; i<options.Count; i++) {
                var opt = options[i];
                var m = font.MeasureString(opt.text);
                sb.DrawString(font, opt.text, new Vector2(Game1.BufferWidth / 2 - m.X / 2, ypos), i == index ? Color.White : Color.Gray);
                ypos += space+(int)m.Y;
            }
        }

        public GameState Update() {
            KeyboardState state = Keyboard.GetState();
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

        public void regenerateOptions() {
            options.Clear();
            generateOptions();
        }

        public virtual void generateOptions() {
            if (fromMenu == null) this.options.Add(new MenuOption("Exit", () =>
            {
                Game1.Instance.Quit();
                return this;
            }));
            else this.options.Add(new MenuOption("Back", () => fromMenu));
        }

    }

    class MainMenu : Menu {

        public MainMenu() : base(null) {}

        public new void Draw(SpriteBatch sb) {
            base.Draw(sb);
        }

        public new GameState Update() {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Game1.Instance.Quit();
            return base.Update();
        }

        public override void generateOptions() {
            options.Add(new MenuOption("Play", () => { return Playing.Instance; }));
            options.Add(new MenuOption("Settings", () => new SettingsMenu(this)));
            base.generateOptions();
        }

    }

    class SettingsMenu : Menu {

        public SettingsMenu(Menu fromMenu) : base(fromMenu) {}

        public override void generateOptions() {
            var settings = Game1.Instance.settings;
            base.generateOptions();
            this.options.Add(new MenuOption($"Resolution: {Game1.Instance.settings.screenWidth}x{Game1.Instance.settings.screenHeight}", () => {
                Point size = new Point(settings.screenWidth, settings.screenHeight);
                int index = Game1.resolutions.IndexOf(size);
                index++;
                if (index == Game1.resolutions.Count) index = 0;
                var r = Game1.resolutions[index];
                Game1.Instance.settings.screenWidth = r.X;
                Game1.Instance.settings.screenHeight = r.Y;
                regenerateOptions();
                
                return this;
            }));
            this.options.Add(new MenuOption("Fullscreen: " + (settings.fullscreen ? "On" : "Off"), () => {
                Game1.Instance.settings.fullscreen = !settings.fullscreen;
                regenerateOptions();
                return this;
            }));
            this.options.Add(new MenuOption("Apply", () => { Game1.Instance.ApplySettings(); return this; }));
        }

    }
    
}
