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
    public class Bullet : GameObject {

        private int lifespan = 25;

        // ----------------------------------------------------------------

        // Constructor (Uses GameObject constructor)
        public Bullet(float x, float y, float angle, ref AsteroidsGame game) : base(x, y, ref game) {
            this.sprite = game.tex_bullet;
            origin.X = sprite.Width / 2;
            origin.Y = sprite.Height / 2;
            collision_box.Width = 4;
            collision_box.Height = 4;
            motion_angle = angle;
            motion_speed = 10;
        }

        public override void UpdateObject() {
            if (lifespan < 1)
                game.Destroy(this);
            else
                lifespan--;
            LoopBorders();
        }

        protected override void Collision(ref GameObject other) {

        }

        public Vector2 GetMotion() {
            Vector2 result = motion;
            result.Normalize();
            return result;
        }
    }
}
