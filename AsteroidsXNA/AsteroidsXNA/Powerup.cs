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
    public class Powerup : GameObject {

        public enum Type { None, Laser, Shield, Nuke }
        private Type type;
        private Random random;
        private int time, frame;

        public Powerup (Type type , ref AsteroidsGame game) : base(0, 0, ref game) {
            frame = 0;
            sprite = game.tex_powerup[frame];
            origin.X = sprite.Width / 2;
            origin.Y = sprite.Height / 2;
            collision_box.Width = 24;
            collision_box.Height = 24;

            random = new Random(this.GetHashCode());
            SetType(type);
            if (random.Next(2) == 0) {
                motion_angle = 180f;
                location.X = game.screenWidth;
            }
                
            motion_speed = 1;
            time = 0;
            location.Y = random.Next(32,game.screenHeight-32);
        }

        public override void UpdateObject() {
            time++;
            if (time % 5 == 0)
                frame++;
            if (frame > 3)
                frame = 0;
            sprite = game.tex_powerup[frame];
            if (location.X > game.screenWidth + 16 || location.X < -16)
                game.Destroy(this);
        }

        protected override void Collision(ref GameObject other) {
            if (other is Ship) {
                ((Ship)other).SetPowerup(type);
                game.Destroy(this);
            }
            if (other is Bullet || other is Laser) {
                if (type == Type.Laser)
                    game.ScoreAdd(100);
                else if (type == Type.Shield)
                    game.ScoreAdd(300);
                else if (type == Type.Nuke)
                    game.ScoreAdd(1000);

                if (other is Bullet)
                    game.Destroy(other);
                game.Destroy(this);
            }
        }

        public void SetType(Type type) {
            this.type = type;
            switch (type) {
                case Type.Laser:
                    draw_color = Color.Red;
                    break;
                case Type.Shield:
                    draw_color = Color.Aqua;
                    break;
                case Type.Nuke:
                    draw_color = Color.Green;
                    break;
            }
        }
    }
}
