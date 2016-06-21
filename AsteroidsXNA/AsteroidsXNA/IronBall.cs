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

    public class IronBall : GameObject {

        private Random random;
        private SoundEffectInstance sound_ting;

        public IronBall(int x, int y, ref AsteroidsGame game) : base(x, y, ref game) {
            sprite = game.tex_ironBall;
            origin.X = sprite.Width / 2;
            origin.Y = sprite.Height / 2;
            collision_box.Width = 24;
            collision_box.Height = 24;

            random = new Random(this.GetHashCode());
            motion_angle = (float)random.Next(360);
            motion_speed = 1;
        }

        public override void UpdateObject() {
            /*
            if (lifespan < 1 && (location.X < 0 || location.X > 800 || location.Y < 0 || location.Y > 480))
                game.Destroy(this);
            else
                lifespan--;
             */
            LoopBorders();

        }

        protected override void Collision(ref GameObject other) {
            if (other is Bullet) {
                MotionAddRelative(((Bullet)other).GetMotion());
                game.Destroy(other);
                sound_ting = game.sfx_ironBall.CreateInstance();
                sound_ting.Volume = .2f;
                sound_ting.Play();
                game.ScoreAdd(5);
            }
            if (other is Laser) {
                game.Destroy(other);
            }
        }



    }
}
