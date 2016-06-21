using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;

namespace AsteroidsXNA {
    public class DebugLine : GameObject{

        public DebugLine(int x, int y, ref AsteroidsGame game) : base(x, y, ref game) {
            this.sprite = game.tex_blank;
            origin.X = sprite.Width / 2;
            origin.Y = sprite.Height / 2;
            draw_yscale = .4f;
        }

        public override void UpdateObject() { }

        protected override void Collision(ref GameObject other) { }

        public void SetAngle(float angle) {
            draw_angle = angle;
        }

        public void SetLength(float length) {
            draw_xscale = length;
        }

        public void SetPosition(Vector2 location) {
            this.location.X = location.X;
            this.location.Y = location.Y;
        }

        public void SetColor(Color color) {
            draw_color = color;
        }

    }
}
