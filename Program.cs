using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PE2
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            GameSettings gameSettings = new GameSettings()
            {
                DebugMode = true
            };
            WindowSettings windowSettings = new WindowSettings()
            {
                VSync = false,
                Size = new Vector2(816, 489),
                Title = "PixelEngine 2.0"
            };

            Application.Run(new Main(gameSettings, windowSettings));
        }
    }
}
