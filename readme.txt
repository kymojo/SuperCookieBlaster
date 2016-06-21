 * Super Cookie Blaster -- CSCD371 Final (Winter 2015)
 * Kyle Johnson
 * 
 * This is a rendition of asteroids, programmed in XNA.
 * 
 * ----------------------------------
 *          << CONTROLS >>
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
 * o I used XNA instead of WPF (not a shortcoming, but doesn't match my writeup).
 * o I used serialized data for highscore saving, not SQLite (version complications).
 * o Game needs balancing (point amounts, difficulty, etc.)
 * o No options or help menu.