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
    public class Asteroid : GameObject {

        Random random;
        public int size;
        private int rotate_dir;
        private SoundEffectInstance sound_break;

        public Asteroid(int x, int y, int size, ref AsteroidsGame game) : base(x, y, ref game) {
            random = new Random(this.GetHashCode());
            this.size = size;
            SetSize(size);
            motion_angle = (float)random.Next(360);
            motion_speed = 1;
            draw_angle = (float)random.Next(360);
            rotate_dir = random.Next(2) -1;
            if (rotate_dir == 0)
                rotate_dir = 1;
        }

        public override void UpdateObject() {
            draw_angle += motion_speed * rotate_dir;
            LoopBorders();
        }

        protected override void Collision(ref GameObject other) {
            if (other is Bullet) {
                game.ScoreAdd(size*10);
                BreakUp();
                game.Destroy(other);
            } else if (other is Laser) {
                game.ScoreAdd(size * 10);
                BreakUp();
            }
        }

        public void BreakUp() {
            sound_break = game.sfx_cookie.CreateInstance();
            sound_break.Volume = .2f;
            sound_break.Play();
            if (size == 2) {
                game.Create_Asteroid((int)location.X, (int)location.Y, 1);
                game.Create_Asteroid((int)location.X, (int)location.Y, 1);
            }
            if (size == 3) {
                game.Create_Asteroid((int)location.X, (int)location.Y, 2);
                game.Create_Asteroid((int)location.X, (int)location.Y, 2);
            }
            game.Destroy(this);
        }

        public void SetSize(int size) {
            switch (size) {
                case 1:
                    sprite = game.tex_cookieSml;
                    draw_depth = .7f;
                    collision_box.Width = 20;
                    collision_box.Height = 20;
                    break;
                case 2:
                    sprite = game.tex_cookieMed;
                    draw_depth = .8f;
                    collision_box.Width = 48;
                    collision_box.Height = 48;
                    break;
                case 3:
                    sprite = game.tex_cookieBig;
                    draw_depth = .9f;
                    collision_box.Width = 64;
                    collision_box.Height = 64;
                    break;
            }
            origin.X = sprite.Width / 2;
            origin.Y = sprite.Height / 2;
        }

    }
}
