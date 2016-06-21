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
    public class Star : GameObject{

        SoundEffectInstance sound_jingle;

        public Star(int x, int y, ref AsteroidsGame game) : base(x, y, ref game){
            sprite = game.tex_star;
            origin.X = sprite.Width / 2;
            origin.Y = sprite.Height / 2;
            collision_box.Width = 24;
            collision_box.Height = 24;
            FlyAtShip();
            sound_jingle = game.sfx_star.CreateInstance();
            sound_jingle.Volume = .2f;
            sound_jingle.Play();
        }

        public override void UpdateObject() {
            draw_angle--;
            if (location.X < -16 || location.X > game.screenWidth + 16 ||
                location.Y < -16 || location.Y > game.screenHeight + 16)
                game.Destroy(this);
        }

        protected override void Collision(ref GameObject other) {
            if (other is Bullet || other is Laser) {
                if (other is Bullet)
                    game.Destroy(other);
                game.ScoreAdd(1000);
                game.Destroy(this);
            }
        }

        private void FlyAtShip() {
            motion_speed = 8;
            Vector2 shipLocation = game.obj_ship.GetLocation();
            float deltaX = shipLocation.X - location.X;
            float deltaY = shipLocation.Y - location.Y;
            motion_angle = (float)(Math.Atan(deltaY / deltaX) * 180 / Math.PI);
            if (shipLocation.X < location.X)
                motion_angle += 180;
        }
    }
}
