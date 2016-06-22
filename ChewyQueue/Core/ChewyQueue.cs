namespace ChewyQueue.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Threading.Tasks;

    using Emgu.CV;
    using Emgu.CV.CvEnum;
    using Emgu.CV.Structure;

    using NLog;

    /// <summary>
    ///     A intelligent and easy to use League of Legends queue bot.
    /// </summary>
    internal class ChewyQueue
    {
        #region Constants

        /// <summary>
        ///     The image directory
        /// </summary>
        private const string ImageDirectory = "Images";

        /// <summary>
        ///     The league client process name
        /// </summary>
        private const string LeagueClientProcessName = "LolClient";

        /// <summary>
        ///     The league game process name
        /// </summary>
        private const string LeagueGameProcessName = "League of Legends";

        #endregion

        #region Properties

        private static Logger Logger => LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Gets or sets the images.
        /// </summary>
        /// <value>
        ///     The images.
        /// </value>
        private IList<Image<Bgr, byte>> Images { get; } = new List<Image<Bgr, byte>>();

        /// <summary>
        ///     Gets or sets the random.
        /// </summary>
        /// <value>
        ///     The random.
        /// </value>
        private Random Random { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Starts this instance.
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            Console.WindowWidth = 115;
            Console.WindowHeight = 35;
            Console.Title = $"ChewyQueuer Version {Assembly.GetExecutingAssembly().GetName().Version}";

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine(
                "  /$$$$$$  /$$                                          /$$$$$$                                                   ");
            Console.WriteLine(
                " /$$__  $$| $$                                         /$$__  $$                                                  ");
            Console.WriteLine(
                "| $$  \\__/| $$$$$$$   /$$$$$$  /$$  /$$  /$$ /$$   /$$| $$  \\ $$ /$$   /$$  /$$$$$$  /$$   /$$  /$$$$$$   /$$$$$$ ");
            Console.WriteLine(
                "| $$      | $$__  $$ /$$__  $$| $$ | $$ | $$| $$  | $$| $$  | $$| $$  | $$ /$$__  $$| $$  | $$ /$$__  $$ /$$__  $$");
            Console.WriteLine(
                "| $$      | $$  \\ $$| $$$$$$$$| $$ | $$ | $$| $$  | $$| $$  | $$| $$  | $$| $$$$$$$$| $$  | $$| $$$$$$$$| $$  \\__/");
            Console.WriteLine(
                "| $$    $$| $$  | $$| $$_____/| $$ | $$ | $$| $$  | $$| $$/$$ $$| $$  | $$| $$_____/| $$  | $$| $$_____/| $$      ");
            Console.WriteLine(
                "|  $$$$$$/| $$  | $$|  $$$$$$$|  $$$$$/$$$$/|  $$$$$$$|  $$$$$$/|  $$$$$$/|  $$$$$$$|  $$$$$$/|  $$$$$$$| $$      ");
            Console.WriteLine(
                " \\______/ |__/  |__/ \\_______/ \\_____/\\___/  \\____  $$ \\____ $$$ \\______/  \\_______/ \\______/  \\_______/|__/      ");
            Console.WriteLine(
                "                                             /$$  | $$      \\__/                                                  ");
            Console.WriteLine(
                "                                            |  $$$$$$/                                                            ");
            Console.WriteLine(
                "                                             \\______/                                                             ");
            Console.WriteLine(
                "                                                                                                                  ");
            Console.ResetColor();

            Logger.Info($"ChewyQueuer Version {Assembly.GetExecutingAssembly().GetName().Version} started!");

            if (!Process.GetProcessesByName(LeagueClientProcessName).Any())
            {
                Logger.Error("Please start the Leauge of Legends client and log in.");
                return;
            }

            await this.CheckImageDirectory();
            this.LoadImages();

            if (this.Images.Count == 0)
            {
                Logger.Warn("There are no usable images in the images directory!");
                await DownloadImages();
            }

            Win32Imports.BringWindowToFront(LeagueClientProcessName);
            this.Random = new Random(Environment.TickCount);

            while (true)
            {
                await this.ProcessEvents();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Downloads the images.
        /// </summary>
        /// <returns></returns>
        private static async Task DownloadImages()
        {
            try
            {
                Logger.Info("No images detected, downloading images.zip");

                var webClient = new WebClient();
                const string FileName = "images.zip";

                await webClient.DownloadFileTaskAsync("https://s.chewy.pw/cdn/images.zip", FileName);
                ZipFile.ExtractToDirectory(FileName, ImageDirectory);
                File.Delete(FileName);
            }
            catch (Exception e)
            {
                Logger.Fatal(e, "An error occured attempting to download images.");
            }
        }

        /// <summary>
        ///     Checks the image directory.
        /// </summary>
        /// <returns></returns>
        private async Task CheckImageDirectory()
        {
            try
            {
                if (!Directory.Exists(ImageDirectory))
                {
                    Directory.CreateDirectory(ImageDirectory);
                    await DownloadImages();
                }
            }
            catch (Exception e)
            {
                Logger.Fatal(
                    e, 
                    "Could not access or create the image directory. Are you running this program as admin?");
            }
        }

        /// <summary>
        ///     Loads the images.
        /// </summary>
        private void LoadImages()
        {
            try
            {
                foreach (var file in Directory.GetFiles(ImageDirectory))
                {
                    Logger.Info($"Loading {file}");

                    try
                    {
                        this.Images.Add(new Image<Bgr, byte>(new Bitmap(Image.FromFile(file))));
                    }
                    catch (Exception e)
                    {
                        Logger.Warn(e, $"Error while trying to load {file}. Skipping it.");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Fatal(
                    e, 
                    "Could not access or create the image directory. Are you running this program as admin?");
            }
        }

        /// <summary>
        ///     Processes the events.
        /// </summary>
        /// <returns></returns>
        private async Task ProcessEvents()
        {
            var leagueProcess = Process.GetProcessesByName(LeagueGameProcessName).FirstOrDefault();

            if (leagueProcess != null)
            {
                Logger.Info("Waiting for League to exit...");
                leagueProcess.WaitForExit();
            }

            try
            {
                using (var bitmap = Win32Imports.CaptureScreen())
                using (var source = new Image<Bgr, byte>(bitmap))
                {
                    foreach (var template in this.Images)
                    {
                        using (var result = source.MatchTemplate(template, TemplateMatchingType.CcoeffNormed))
                        {
                            double[] minValues, maxValues;
                            Point[] minLocations, maxLocations;

                            result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);
                            Logger.ConditionalDebug($"Confidence: {maxValues[0]}");

                            if (maxValues[0] < 0.8)
                            {
                                continue;
                            }

                            var match = new Rectangle(maxLocations[0], template.Size);
                            var randomPoint = new Point(
                                this.Random.Next(match.Left, match.Right), 
                                this.Random.Next(match.Top, match.Bottom));

                            Win32Imports.MoveMouse(randomPoint);
                            Win32Imports.LeftClickMouse(randomPoint);

                            await Task.Delay(250);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            await Task.Delay(100);
        }

        #endregion
    }
}