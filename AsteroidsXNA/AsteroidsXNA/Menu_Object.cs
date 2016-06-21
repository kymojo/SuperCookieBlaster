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

namespace AsteroidsXNA {
    public class Menu_Object : GameObject{

        public Menu_Object(ref AsteroidsGame game) : base(240, 304, ref game) {
            sprite = game.tex_cookieSml;
            origin.X = sprite.Width / 2;
            origin.Y = sprite.Height / 2;
        }

        public override void UpdateObject() {

            // Keyboard
            if (KeyboardCheckPressed(Keys.Up))
                game.MainMenuMarkerUp();

            if (KeyboardCheckPressed(Keys.Down))
                game.MainMenuMarkerDown();

            if (KeyboardCheckPressed(Keys.Enter))
                game.MainMenuSelect();

            location.Y = 304 + ((int)game.menuMarker * 80);

            draw_angle += 1;

        }

        protected override void Collision(ref GameObject other) { }

        public override void DrawSpecial() {
            Rectangle rec;
            rec = new Rectangle(232, 0, game.tex_splash.Width, game.tex_splash.Height);
            spriteBatch.Draw(game.tex_splash, rec, Color.White);
            rec = new Rectangle(264, 272, game.tex_btnStart.Width, game.tex_btnStart.Height);
            spriteBatch.Draw(game.tex_btnStart, rec, Color.White);
            rec = new Rectangle(264, 352, game.tex_btnExit.Width, game.tex_btnExit.Height);
            spriteBatch.Draw(game.tex_btnExit, rec, Color.White);
        }

    }
}
