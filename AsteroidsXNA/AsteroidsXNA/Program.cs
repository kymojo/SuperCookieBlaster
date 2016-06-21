using System;

namespace AsteroidsXNA {
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (AsteroidsGame game = new AsteroidsGame())
            {
                game.Run();
            }
        }
    }
#endif
}

