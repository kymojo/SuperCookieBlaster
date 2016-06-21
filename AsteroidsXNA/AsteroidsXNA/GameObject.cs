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

    public abstract class GameObject {

        protected AsteroidsGame game;

        // Drawing Controls
        protected Vector2 origin;
        protected float draw_angle, draw_xscale, draw_yscale, draw_depth;
        protected Color draw_color;
        protected bool visible;
        protected Texture2D sprite;
        protected Rectangle sourceRectangle = new Rectangle();
        protected SpriteBatch spriteBatch;

        // Position
        protected Vector2 location, motion;
        protected float motion_angle, motion_speed;

        // Collisions
        protected Rectangle collision_box;

        protected enum KeyAction { NONE, HELD, PRESSED, RELEASED }

        // Base Constructor (x, y, sprite, spriteBatch)
        public GameObject(float x, float y, ref AsteroidsGame game) {
            this.game = game;

            // Position / Movement
            location.X = x;
            location.Y = y;
            motion_angle = 0;
            motion_speed = 0;

            // Drawing
            this.sprite = game.tex_blank;
            origin.X = 0;
            origin.Y = 0;
            draw_angle = 0;
            draw_xscale = 1;
            draw_yscale = 1;
            this.spriteBatch = game.spriteBatch;
            draw_color = Color.White;
            draw_depth = 1;
            visible = true;

            collision_box = new Rectangle((int)x, (int)y, sprite.Width, sprite.Height);
        }

        //====================================================================================
        #region Methods
        //====================================================================================

        //------------------------------------------------------------------------------------
        #region Drawing / Updating
        //------------------------------------------------------------------------------------

        // Draws the Object
        public void Draw() {
            if (!visible)
                return;
            sourceRectangle.Height = (int)(sprite.Height * draw_yscale);
            sourceRectangle.Width = (int)(sprite.Width * draw_xscale);
            spriteBatch.Draw(sprite, location, sourceRectangle, draw_color, DegToRads(draw_angle), origin, 1.0f, SpriteEffects.None, draw_depth);
            DrawSpecial();
        }

        public virtual void DrawSpecial() { }

        // Updates the Object
        public virtual void Update() {
            // Special Update (subclass specific)
            UpdateObject();

            // Vector Movement
            AngleFix360(motion_angle);
            SetVectorFromAngleMagnitude(ref motion, motion_angle, motion_speed);
            MoveVector(motion);

            // Collision Detection
            collision_box.X = (int)(location.X - origin.X);
            collision_box.Y = (int)(location.Y - origin.Y);
            foreach (GameObject obj in game.objects) {
                if (collision_box.Intersects(obj.collision_box)) {
                    if (!obj.Equals(this) && game.toDestroy.IndexOf(obj) == -1) {
                        var other = obj;
                        Collision(ref other);
                    }
                }
            }

        }

        public abstract void UpdateObject();

        #endregion
        //------------------------------------------------------------------------------------
        #region Collision
        //------------------------------------------------------------------------------------

        protected abstract void Collision(ref GameObject other);

        #endregion
        //------------------------------------------------------------------------------------
        #region Motion
        //------------------------------------------------------------------------------------

        // Move the object based upon vector
        protected void MoveVector(Vector2 vector) {
            location.X += vector.X;
            location.Y += vector.Y;
        }

        // Add relative motion to the object
        protected void MotionAddRelative(Vector2 vector) {
            motion.X += vector.X;
            motion.Y += vector.Y;
            motion_angle = VectorAngle(motion);
            motion_speed = VectorMagnitude(motion);
        }

        // Reposition the object to (x, y)
        protected void LocationSet(float x, float y) {
            location.X = x;
            location.Y = y;
        }

        // Make sure angle is 0-359 degrees
        private void AngleFix360(float theta) {
            if (theta > 359 || theta < 0) {
                theta = theta % 360;
                if (theta < 0)
                    theta += 360;
            }
        }

        // Used to jump across the sides of the screen
        protected void LoopBorders() {

            if (location.X < -(sprite.Width / 2))
                location.X += game.screenWidth + sprite.Width;

            if (location.X > game.screenWidth + (sprite.Width / 2))
                location.X -= game.screenWidth + sprite.Width;

            if (location.Y < -(sprite.Height / 2))
                location.Y += game.screenHeight + sprite.Height;

            if (location.Y > game.screenHeight + (sprite.Height / 2))
                location.Y -= game.screenHeight + sprite.Width;
        }

        // Reverse Speed
        public void ReverseSpeed() {
            motion_speed *= -1;
        }

        // Stop
        public void StopMotion() {
            motion_speed = 0;
            motion_angle = 0;
        }

        #endregion
        // ----------------------------------------------------------------
        #region Vector Functions
        // ----------------------------------------------------------------     

        // Sets the (x, y) of a vector from given angle and magnitude
        protected void SetVectorFromAngleMagnitude(ref Vector2 vector, float angle, float mag) {
            int x_sign = 1, y_sign = 1;

            // Adjust angles for Quadrants
            if (angle >= 90 || angle < 180) {       // Quadrant II
                angle = 180 - angle;
                x_sign = -1;
            } else if (angle >= 180 || angle < 270) {  // Quadrant III
                angle = angle - 180;
                x_sign = -1;
                y_sign = -1;
            } else if (angle >= 270) {                 // Quadrant IV
                angle = 360 - angle;
                y_sign = -1;
            }

            // Trigonometry!!!
            vector.X = x_sign * mag * (float)Math.Cos(DegToRads(angle));
            vector.Y = y_sign * mag * (float)Math.Sin(DegToRads(angle));
        }

        // Returns the magnitude of a vector
        private float VectorMagnitude(Vector2 vector) {
            if (vector == null)
                return -1;
            return (float)Math.Pow((vector.X * vector.X) + (vector.Y * vector.Y), .5);
        }

        // Returns the angle of a vector
        private float VectorAngle(Vector2 vector) {
            if (vector == null)
                return -1;
            return RadsToDeg((float)Math.Atan2(vector.Y, vector.X));
        }

        #endregion
        // ----------------------------------------------------------------
        #region Keyboard Code
        // ----------------------------------------------------------------

        private KeyAction KeyboardCheckState(Keys key) {

            KeyAction result = KeyAction.NONE;

            // Key Pressed
            if (game.kbNewState.IsKeyDown(key)) {
                result = KeyAction.HELD;
                // Pressed
                if (!game.kbOldState.IsKeyDown(key)) {
                    result = KeyAction.PRESSED;
                }
                //Released
            } else if (game.kbOldState.IsKeyDown(key)) {
                result = KeyAction.RELEASED;
            }

            // Update saved state.
            return result;
        }

        /// <summary>
        /// Returns whether or not the given key is down.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected bool KeyboardCheck(Keys key) {
            if (KeyboardCheckState(key) == KeyAction.HELD || KeyboardCheckState(key) == KeyAction.PRESSED)
                return true;
            return false;
        }

        /// <summary>
        /// Returns whether or not the given key is pressed.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected bool KeyboardCheckPressed(Keys key) {
            if (KeyboardCheckState(key) == KeyAction.PRESSED)
                return true;
            return false;
        }

        /// <summary>
        /// Returns whether or not the given key is released.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected bool KeyboardCheckReleased(Keys key) {
            if (KeyboardCheckState(key) == KeyAction.RELEASED)
                return true;
            return false;
        }


        #endregion
        //------------------------------------------------------------------------------------
        #region Helpers
        //------------------------------------------------------------------------------------

        // Degrees to Radians
        protected float DegToRads(float degrees) {
            return degrees * ((float)Math.PI / 180f);
        }

        protected float RadsToDeg(float radians) {
            return radians * (180f / (float)Math.PI);
        }

        #endregion
        //------------------------------------------------------------------------------------

        #endregion
        //====================================================================================
    }
}
