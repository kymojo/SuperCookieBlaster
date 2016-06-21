/*
 * Super Cookie Blaster -- CSCD371 Final (Winter 2015)
 * Kyle Johnson
 * 
 * This is a rendition of asteroids, programmed in XNA.
 * 
 * ----------------------------------
 *          << CONTROLS >>
 *          
 * [In Menu]
 * o Arrow Keys     -- Move Cursor
 * o Enter          -- Choose Option
 * o M key          -- Toggle Music
 * 
 * [In Game]
 * o Left / Right   -- Rotate Ship
 * o Up             -- Thrust
 * o Space          -- Shoot
 * o Left Shift     -- Use Powerup
 * o Escape         -- Return to Menu
 * 
 * ----------------------------------
 *          << STUFF TO KNOW >>
 *          
 * [Obstacles]
 * o Cookies        -- Break into smaller pieces when shot
 * o Iron Ball      -- Moves when shot; Can be destroyed with Nuke
 * o Mr. Star       -- Appears randomly and flies at the ship
 * 
 * [Powerups]
 * o Laser  (RED)   -- Fires lasers that cut through cookies
 * o Shield (BLUE)  -- Protects the ship from damage
 * o Nuke   (GREEN) -- Destroys objects on screen
 * 
 * ----------------------------------
 *          << SHORTCOMINGS ? >>
 *          
 * o I used XNA instead of WPF (not a shortcoming, but doesn't match my writeup).
 * o I used serialized data for highscore saving, not SQLite (version complications).
 * o Game needs balancing (point amounts, difficulty, etc.)
 * o No options or help menu.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace AsteroidsXNA {

    /// <summary>
    /// This is the main type for your game
    /// </summary>

    public class AsteroidsGame : Microsoft.Xna.Framework.Game {

        //====================================================================================
        #region GAME VARIABLES
        //====================================================================================

        // Game Stuff
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        Random random = new Random();
        public int screenWidth = 640;
        public int screenHeight = 480;
        public KeyboardState kbNewState, kbOldState;

        public enum GameState { MainMenu, Playing }
        public GameState gameState;
        public int level, score, highscore;
        public enum MenuMarker { Start, Exit }
        public MenuMarker menuMarker;
        public bool gameOver;

        // Textures
        public Texture2D tex_splash, tex_btnStart, tex_btnExit;
        public Texture2D tex_blank, tex_background, tex_ship, tex_shipThrust;
        public Texture2D tex_cookieBig, tex_cookieMed, tex_cookieSml, tex_ironBall, tex_star;
        public Texture2D tex_bullet, tex_laser, tex_shield;
        public Texture2D[] tex_powerup = new Texture2D[4];

        // Font
        public SpriteFont fnt_text;

        // Sounds
        public SoundEffectInstance sound_music, sound_level, sound_menu;
        public SoundEffect sfx_music, sfx_menu, sfx_level, sfx_nuke, sfx_gameover, sfx_star;
        public SoundEffect sfx_bullet, sfx_cookie, sfx_ironBall, sfx_thrust, sfx_laser;

        // Game Objects Stuff
        public List<GameObject> toCreate = new List<GameObject>();
        public List<GameObject> objects = new List<GameObject>();
        public List<GameObject> toDestroy = new List<GameObject>();
        public Ship obj_ship;

        // DEBUG VARIABLE
        public bool debug = false;

        #endregion
        //====================================================================================
        #region GAME CONSTRUCTOR
        //====================================================================================

        public AsteroidsGame() {
            this.Window.Title = "Super Cookie Blaster";
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.ApplyChanges();
        }

        #endregion
        //====================================================================================
        #region INITIALIZE
        //====================================================================================
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        
        protected override void Initialize() {
            // TODO: Add your initialization logic here
            base.Initialize();
            gameState = GameState.MainMenu;
            menuMarker = MenuMarker.Start;
            level = 1;
            gameOver = true;
            ScoresLoad();
        }
        
        #endregion
        //====================================================================================
        #region LOAD CONTENT
        //====================================================================================
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>

        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // Load Background
            tex_background = Content.Load<Texture2D>("tex_background");

            // Load Sprites
            tex_splash = Content.Load<Texture2D>("tex_logo");
            tex_btnStart = Content.Load<Texture2D>("tex_btnStart");
            tex_btnExit = Content.Load<Texture2D>("tex_btnExit");
            tex_blank = Content.Load<Texture2D>("tex_square");
            tex_ship = Content.Load<Texture2D>("tex_ship");
            tex_shipThrust = Content.Load<Texture2D>("tex_shipThrust");
            tex_cookieBig = Content.Load<Texture2D>("tex_cookieBig");
            tex_cookieMed = Content.Load<Texture2D>("tex_cookieMed");
            tex_cookieSml = Content.Load<Texture2D>("tex_cookieSml");
            tex_bullet = Content.Load<Texture2D>("tex_bullet");
            tex_ironBall = Content.Load<Texture2D>("tex_ironBall");
            tex_laser = Content.Load<Texture2D>("tex_laser");
            tex_shield = Content.Load<Texture2D>("tex_shield");
            tex_star = Content.Load<Texture2D>("tex_star");

            for (int i = 0; i < 4; i++ )
                tex_powerup[i] = Content.Load<Texture2D>("tex_powerup" + i);
                
            fnt_text = Content.Load<SpriteFont>("fnt_text");

            // Load Sounds
            sfx_music = Content.Load<SoundEffect>("sfx_music");
            sfx_menu = Content.Load<SoundEffect>("sfx_menu");
            sfx_bullet = Content.Load<SoundEffect>("sfx_bullet");
            sfx_cookie = Content.Load<SoundEffect>("sfx_cookie");
            sfx_ironBall = Content.Load<SoundEffect>("sfx_ironBall");
            sfx_thrust = Content.Load<SoundEffect>("sfx_thrust");
            sfx_level = Content.Load<SoundEffect>("sfx_level");
            sfx_laser = Content.Load<SoundEffect>("sfx_laser");
            sfx_nuke = Content.Load<SoundEffect>("sfx_nuke");
            sfx_gameover = Content.Load<SoundEffect>("sfx_gameover");
            sfx_star = Content.Load<SoundEffect>("sfx_star");

            sound_music = sfx_music.CreateInstance();
            sound_music.IsLooped = true;
            sound_music.Volume = .5f;

            sound_menu = sfx_menu.CreateInstance();

            sound_level = sfx_level.CreateInstance();
            sound_level.Volume = .5f;


            // Start Game
            GameStart();
        }

        #endregion
        //====================================================================================
        #region UNLOAD CONTENT
        //====================================================================================
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>

        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        #endregion
        //====================================================================================
        #region UPDATE
        //====================================================================================
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>

        protected override void Update(GameTime gameTime) {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // <Keyboard Update>
            kbNewState = Keyboard.GetState();

            // Music Control
            if (kbNewState.IsKeyDown(Keys.M))
                if (!kbOldState.IsKeyDown(Keys.M)) {
                    if (sound_music.State == SoundState.Stopped)
                        sound_music.Resume();
                    else
                        sound_music.Stop();
                }

            if (gameState == GameState.Playing) {
                RandomEvents();
                if (kbNewState.IsKeyDown(Keys.Escape) && gameOver)
                    EndGame();
            }
                

            // Create Objects
            foreach (GameObject obj in toCreate)
                objects.Add(obj);
            toCreate.Clear();

            // Update Objects
            foreach (GameObject obj in objects)
                obj.Update();

            // Delete Objects
            foreach (GameObject obj in toDestroy)
                objects.Remove(obj);

            if (toDestroy.Count > 0) {
                CheckLevel();
                toDestroy.Clear();
            }

            // </Keyboard Update>
            kbOldState = kbNewState;

            base.Update(gameTime);
        }

        #endregion
        //====================================================================================
        #region DRAW
        //====================================================================================
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Background
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, null);
            spriteBatch.Draw(tex_background, new Rectangle(0, 0, screenWidth, screenHeight), new Rectangle(0, 0, screenWidth, screenHeight), Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 1);
            spriteBatch.End();

            
            spriteBatch.Begin();
            // <-- /DRAW -->

            // Sprites
            foreach(GameObject obj in objects)
                obj.Draw();

            // Scores
            string output = "Highscore: " + highscore;
            Vector2 FontOrigin = fnt_text.MeasureString(output);
            Vector2 FontPos = new Vector2(2, 0);
            spriteBatch.DrawString(fnt_text, output, FontPos, Color.Yellow);
            output = "Score: " + score;
            FontOrigin = fnt_text.MeasureString(output);
            FontPos = new Vector2(2, 16);
            spriteBatch.DrawString(fnt_text, output, FontPos, Color.Yellow);

            // Game Over
            if (gameOver && gameState == GameState.Playing) {
                FontOrigin = fnt_text.MeasureString("GAME OVER") / 2;
                FontPos = new Vector2(screenWidth/2, screenHeight/2);
                spriteBatch.DrawString(fnt_text, "GAME OVER", FontPos, Color.Yellow, 0, FontOrigin, 1, SpriteEffects.None, 0);
            }

            // <-- /DRAW -->
            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
        //====================================================================================
        #region CREATE / DESTROY OBJECTS
        //====================================================================================

        public void Destroy(GameObject obj) {
            toDestroy.Add(obj);
        }

        public void DestroyAll() {
            foreach (GameObject obj in objects)
                toDestroy.Add(obj);
        }

        public void Create_Menu() {
            var me = this;
            Menu_Object menu = new Menu_Object(ref me);
            toCreate.Add(menu);
        }

        // ----------------------------------------------------------------
        #region Create Game Objects
        // ----------------------------------------------------------------
        public void RandomEdgeCoordinates(out int x, out int y) {
            switch (random.Next(4)) {
                case 0:
                    y = 0;
                    x = random.Next(screenWidth);
                    break;
                case 1:
                    y = screenHeight;
                    x = random.Next(screenWidth);
                    break;
                case 2:
                    x = 0;
                    y = random.Next(screenHeight);
                    break;
                case 3:
                    x = screenWidth;
                    y = random.Next(screenHeight);
                    break;
                default:
                    x = 0;
                    y = 0;
                    break;
            }
        }

        public Ship Create_Ship() {
            if (obj_ship != null)
                return obj_ship;
            var me = this;
            Ship instance = new Ship(this.Window.ClientBounds.Width / 2, this.Window.ClientBounds.Height / 2, ref me);
            toCreate.Add(instance);

            obj_ship = instance;
            if (debug)
                obj_ship.ToggleDebug();

            return instance;
        }

        public Asteroid Create_Asteroid(int x, int y, int size) {
            var me = this;
            Asteroid instance = new Asteroid(x, y, size, ref me);
            toCreate.Add(instance);
            return instance;
        }

        public IronBall Create_IronBall() {
            var me = this;
            int x, y;
            RandomEdgeCoordinates(out x, out y);
            IronBall instance = new IronBall(x, y, ref me);
            toCreate.Add(instance);
            return instance;
        }

        public DebugPoint Create_DebugPoint() {
            var me = this;
            DebugPoint instance = new DebugPoint(0, 0, ref me);
            toCreate.Add(instance);
            return instance;
        }

        public DebugLine Create_DebugLine() {
            var me = this;
            DebugLine instance = new DebugLine(0, 0, ref me);
            toCreate.Add(instance);
            return instance;
        }

        public Bullet Create_Bullet(float x, float y, float angle) {
            var me = this;
            Bullet instance = new Bullet(x, y, angle, ref me);
            toCreate.Add(instance);
            return instance;
        }

        public Laser Create_Laser(float x, float y, float angle) {
            var me = this;
            Laser instance = new Laser(x, y, angle, ref me);
            toCreate.Add(instance);
            return instance;
        }

        public Powerup Create_Powerup(Powerup.Type type) {
            var me = this;
            Powerup instance = new Powerup(type, ref me);
            toCreate.Add(instance);
            return instance;
        }

        public Star Create_Star() {
            int x, y;
            RandomEdgeCoordinates(out x, out y);
            var me = this;
            Star instance = new Star(x, y, ref me);
            toCreate.Add(instance);
            return instance;
        }

        #endregion
        // ----------------------------------------------------------------

        #endregion
        //====================================================================================
        #region GAME METHODS
        //====================================================================================

        // Called when the game is first opened
        private void GameStart() {
            MainMenuStart();
            sound_music.Play();
        }

        public void ScoreAdd(int toAdd) {
            score += toAdd;
            if (score > highscore)
                highscore = score;
        }

        public void ScoresSave() {
            string filename = "scores.dat";
            // Create Save File
            if (File.Exists(filename))
                File.Delete(filename);
            Stream stream = File.Create(filename);

            XmlSerializer serializer = new XmlSerializer(typeof(int));
            serializer.Serialize(stream, highscore);
            stream.Close();
        }

        public void ScoresLoad() {
            string filename = "scores.dat";
            // Create Save File
            if (!File.Exists(filename)) {
                highscore = 1000;
                return;
            }
                
            Stream stream = File.Open(filename, FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(int));
            highscore = (int)serializer.Deserialize(stream);
            stream.Close();
        }

        // ----------------------------------------------------------------
        #region Main Menu Methods
        // ----------------------------------------------------------------

        // Creates menu items
        private void MainMenuStart() {
            Create_Menu();
        }

        public void MainMenuMarkerUp() {
            if (menuMarker != MenuMarker.Start)
                menuMarker--;
            else
                menuMarker = MenuMarker.Exit;
            sfx_menu.Play();
        }

        public void MainMenuMarkerDown() {
            if (menuMarker != MenuMarker.Exit)
                menuMarker++;
            else
                menuMarker = MenuMarker.Start;
            sfx_menu.Play();
        }

        public void MainMenuSelect() {
            
            switch (menuMarker) {
                case MenuMarker.Start:
                    gameState = GameState.Playing;
                    DestroyAll();
                    PlayGame();
                    break;
                case MenuMarker.Exit:
                    ScoresSave();
                    this.Exit();
                    break;
            }
            
        }

        #endregion
        // ----------------------------------------------------------------
        #region Game Methods
        // ----------------------------------------------------------------
        
        // Creates game objects
        public void PlayGame() {
            gameOver = false;
            if (obj_ship == null)
                Create_Ship();
            LevelStart(level);
        }

        public void EndGame() {
            DestroyAll();
            MainMenuStart();
            obj_ship = null;
            level = 1;
            gameState = GameState.MainMenu;
            score = 0;
        }

        public void RandomEvents() {
            if (random.Next(5000) == 0)
                Create_Star();
            else if (random.Next(1000) == 0)
                Create_Powerup(Powerup.Type.Laser);
            else if (random.Next(2000) == 0)
                Create_Powerup(Powerup.Type.Shield);
            else if (random.Next(3000) == 0)
                Create_Powerup(Powerup.Type.Nuke);
        }

        public void Nuke() {
            foreach (GameObject obj in objects)
                if (obj is Asteroid)
                    ((Asteroid)obj).BreakUp();
                else if (obj is IronBall)
                    Destroy(obj);
        }

        public void LevelStart(int level) {
            int x, y;
            for (int i = 0; i < (level+2); i++) {
                RandomEdgeCoordinates(out x, out y);
                Create_Asteroid(x, y, random.Next(2,4));
            }
            if (level % 3 == 0)
                Create_IronBall();
            ScoreAdd((level-1) * 500);
        }
        
        private void CheckLevel() {
            if (gameState != GameState.Playing)
                return;

            bool noAsteroids = true;
            // Check Screen for Asteroids
            foreach (GameObject obj in objects)
                if (obj is Asteroid) {
                    noAsteroids = false;
                    break;
                }   
            // Double Check for Breaking Asteroids
            if (noAsteroids) {
                foreach (GameObject obj in toCreate)
                    if (obj is Asteroid) {
                        noAsteroids = false;
                        break;
                    }      
            }
            // If asteroids, forget it.
            if (!noAsteroids)
                return;

            level++;
            sound_level.Play();
            LevelStart(level);
        }

        #endregion
        // ----------------------------------------------------------------

        #endregion
        //====================================================================================
    }
}
