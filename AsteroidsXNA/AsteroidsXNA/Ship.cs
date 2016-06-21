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
    public class Ship : GameObject{

        private float thrust_angle, thrust_speed;
        private DebugLine line_thrust = null, line_motion = null;
        private DebugPoint point_thrust = null, point_motion = null;
        private bool debug = false;
        private SoundEffectInstance sound_bullet, sound_thrust;

        private Powerup.Type myPowerup;
        private int myPowerupAmount, timer;
        private bool shield;

        // ----------------------------------------------------------------
        #region Constructor
        // ----------------------------------------------------------------

        // Constructor (Uses GameObject constructor)
        public Ship(int x, int y, ref AsteroidsGame game) : base(x, y, ref game) {
            this.sprite = game.tex_ship;
            origin.X = sprite.Width / 2;
            origin.Y = sprite.Height / 2;
            collision_box.Width = 24;
            collision_box.Height = 24;
            sound_thrust = game.sfx_thrust.CreateInstance();
            sound_thrust.IsLooped = true;
            sound_thrust.Volume = .5f;
            myPowerup = Powerup.Type.None;
            myPowerupAmount = 0;
            timer = 0;
            ShieldSet(false);
        }

        #endregion
        // ----------------------------------------------------------------
        #region Update
        // ----------------------------------------------------------------

        public override void UpdateObject() {

            KeyboardControls();
            draw_angle = thrust_angle;
            SpeedFix();
            LoopBorders();
            timer++;
            UpdatePowerup();

            if (debug)
                UpdateDebugObjects();
        }

        private void SpeedFix() {
            // Make sure speed is positive
            if (motion_speed < 0)
                motion_speed = Math.Abs(motion_speed);
            // Speed cap
            if (motion_speed > 8)
                motion_speed = 8;
        }

        private void KeyboardControls() {
            // LEFT/RIGHT -- Rotate
            if (KeyboardCheck(Keys.Right))
                thrust_angle += 5;
            if (KeyboardCheck(Keys.Left))
                thrust_angle -= 5;

            // UP -- Thrust
            if (KeyboardCheck(Keys.Up) || KeyboardCheckPressed(Keys.Up))
                Thrust();
            else
                StopThrust();

            // SPACE -- Shoot
            if (KeyboardCheckPressed(Keys.Space))
                Shoot();

            // L SHIFT -- Use Powerup
            if (KeyboardCheckPressed(Keys.LeftShift))
                if (myPowerup == Powerup.Type.Laser)
                    ShootLaser();
                else if (myPowerup == Powerup.Type.Nuke)
                    DropNuke();
            if (myPowerup == Powerup.Type.Shield) {
                if (KeyboardCheck(Keys.LeftShift))
                    ShieldSet(true);
                else if (KeyboardCheckReleased(Keys.LeftShift))
                    ShieldSet(false);
            }

            // ESC -- Exit to Menu
            if (KeyboardCheckPressed(Keys.Escape)) {
                StopThrust();
                game.EndGame();
            }
        }

        #endregion
        // ----------------------------------------------------------------
        #region Collision
        // ----------------------------------------------------------------
        protected override void Collision(ref GameObject other) {
            if (!(myPowerup == Powerup.Type.Shield && shield)) {
                if (other is Asteroid || other is IronBall || other is Star) {
                    sound_thrust.Stop();
                    Death();
                }
            } else {
                if (other is Asteroid) {
                    ((Asteroid)other).BreakUp();
                    game.ScoreAdd(((Asteroid)other).size * 30);
                }
            }
            
        }

        #endregion
        // ----------------------------------------------------------------
        #region Drawing
        // ----------------------------------------------------------------
        public override void DrawSpecial() {
            if (shield && myPowerup == Powerup.Type.Shield)
                DrawShield();
            DrawPowerupAmount();
            DrawPowerupLabel();
        }

        private void DrawPowerupLabel() {
            if (myPowerup == Powerup.Type.None)
                return;
            Vector2 FontPos = new Vector2(2, game.screenHeight - 16);
            spriteBatch.DrawString(game.fnt_text, myPowerup.ToString("G"), FontPos, Color.Yellow);
        }

        private void DrawPowerupAmount() {
            if (myPowerup == Powerup.Type.None)
                return;
            int textOffset;
            string output = "" + myPowerupAmount;

            if (myPowerup == Powerup.Type.Laser || myPowerup == Powerup.Type.Nuke)
                textOffset = 22;
            else
                textOffset = 28;

            Vector2 FontOrigin = game.fnt_text.MeasureString(output) / 2;
            FontOrigin.X = (float)((int)FontOrigin.X);
            FontOrigin.Y = (float)((int)FontOrigin.Y);
            Vector2 FontPos = new Vector2((int)location.X, (int)location.Y - textOffset);
            spriteBatch.DrawString(game.fnt_text, "" + myPowerupAmount, FontPos, Color.Yellow, 0, FontOrigin, 1, SpriteEffects.None, 0);

        }

        private void DrawShield() {
            Rectangle rec = new Rectangle((int)(location.X - origin.X), (int)(location.Y - origin.Y), game.tex_shield.Width, game.tex_shield.Height);
            spriteBatch.Draw(game.tex_shield, rec, Color.White);
        }

        #endregion
        // ----------------------------------------------------------------
        #region Ship Functions
        // ----------------------------------------------------------------

        private void Shoot() {
            game.Create_Bullet(location.X, location.Y, draw_angle);
            sound_bullet = game.sfx_bullet.CreateInstance();
            sound_bullet.Volume = .05f;
            sound_bullet.Play();
        }

        private void Thrust() {
            sound_thrust.Play();
            MotionAddRelative(ThrustVector());
            sprite = game.tex_shipThrust;
        }

        private void StopThrust() {
            sound_thrust.Stop();
            thrust_speed = 0;
            motion_speed *= 0.995f;
            sprite = game.tex_ship;
        }

        private void Death() {
            sound_bullet = game.sfx_gameover.CreateInstance();
            sound_bullet.Volume = .5f;
            sound_bullet.Play();
            game.gameOver = true;
            game.Destroy(this);
        }

        #endregion
        // ----------------------------------------------------------------
        #region Powerup Stuff
        // ----------------------------------------------------------------

        public void SetPowerup(Powerup.Type power) {
            if (myPowerup != power)
                myPowerupAmount = 0;
            if (power == Powerup.Type.Laser)
                myPowerupAmount += 30;
            else if (power == Powerup.Type.Shield)
                myPowerupAmount += 30;
            else if (power == Powerup.Type.Nuke)
                myPowerupAmount += 3;
            myPowerup = power;
        }

        private void UpdatePowerup() {
            if (myPowerup != Powerup.Type.None) {
                // If shield is up, decrease powerup amount
                if (myPowerup == Powerup.Type.Shield && timer % 30 == 0 && shield)
                    myPowerupAmount--;
                // If powerup amount is 0, set powerup to None
                if (myPowerupAmount == 0)
                    myPowerup = Powerup.Type.None;
            }
        }

        private void ShieldSet(bool set) {
            if (set) {
                shield = true;
                collision_box.Width = 24;
                collision_box.Height = 24;
            } else {
                shield = false;
                collision_box.Width = 36;
                collision_box.Height = 36;
            }
        }

        private void ShootLaser() {
            game.Create_Laser(location.X, location.Y, draw_angle);
            myPowerupAmount--;
            sound_bullet = game.sfx_laser.CreateInstance();
            sound_bullet.Volume = .05f;
            sound_bullet.Play();
        }

        private void DropNuke() {
            myPowerupAmount--;
            game.Nuke();
            sound_bullet = game.sfx_nuke.CreateInstance();
            sound_bullet.Volume = .5f;
            sound_bullet.Play();
        }

        #endregion
        // ----------------------------------------------------------------
        #region Other
        // ----------------------------------------------------------------

        // Creates a Vector from thrust angle and speed
        private Vector2 ThrustVector() {
            Vector2 thrust = new Vector2();
            SetVectorFromAngleMagnitude(ref thrust, thrust_angle, .25f);
            return thrust;
        }

        public Vector2 GetLocation() {
            return location;
        }

        #endregion
        // ----------------------------------------------------------------
        #region Debug

        public void ToggleDebug() {
            if (line_thrust == null || line_motion == null || point_thrust == null || point_motion == null) {
                this.line_motion = game.Create_DebugLine();
                this.line_thrust = game.Create_DebugLine();
                this.point_motion = game.Create_DebugPoint();
                this.point_thrust = game.Create_DebugPoint();
            }
            if (debug)
                debug = false;
            else
                debug = true;
        }

        public void UpdateDebugObjects() {
            if (line_thrust == null || line_motion == null || point_thrust == null || point_motion == null)
                return;
            line_thrust.SetPosition(location);
            line_thrust.SetAngle(thrust_angle);
            line_thrust.SetLength(thrust_speed * 2);
            line_thrust.SetColor(Color.Red);

            line_motion.SetPosition(location);
            line_motion.SetAngle(motion_angle);
            line_motion.SetLength(motion_speed * 2);
            line_motion.SetColor(Color.Blue);

            point_thrust.SetPosition(location + (ThrustVector()*10));
            point_thrust.SetColor(Color.Yellow);

            point_motion.SetPosition(location + (motion*10));
            point_motion.SetColor(Color.Cyan);
        }

        #endregion
        // ---------------------------------------------------------------- 
    }
}
