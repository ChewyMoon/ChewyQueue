using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChewyQueue
{
    using System.Diagnostics;
    using System.Reflection;

    using NLog;

    /// <summary>
    /// A intelligent and easy to use League of Legends queue bot.
    /// </summary>
    internal class ChewyQueue
    {
        private static Logger Logger => LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The league game process name
        /// </summary>
        private const string LeagueGameProcessName = "League of Legends";

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            Console.WindowWidth = 150;
            Console.WindowHeight = 50;

            Logger.Info($"ChewyQueuer version {Assembly.GetExecutingAssembly().GetName().Version} started!");

            if (!Process.GetProcessesByName("LolClient").Any())
            {
                Logger.Error("Please start the Leauge of Legends client and log in.");
            }

            while (true)
            {
                await this.ProcessEvents();
            }

        }

        /// <summary>
        /// Processes the events.
        /// </summary>
        /// <returns></returns>
        private async Task ProcessEvents()
        {
            Process.GetProcessesByName(LeagueGameProcessName).FirstOrDefault()?.WaitForExit();

            Win32Imports.BringWindowToFront(LeagueGameProcessName);


        }
    }
}
