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
    public class Laser : GameObject {

        private int lifespan = 40;

        public Laser(float x, float y, float angle, ref AsteroidsGame game) : base(x, y, ref game) {
            this.sprite = game.tex_laser;
            origin.X = sprite.Width / 2;
            origin.Y = sprite.Height / 2;
            collision_box.Width = 10;
            collision_box.Height = 10;
            draw_angle = angle;
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

        protected override void Collision(ref GameObject other) { }

    }
}
