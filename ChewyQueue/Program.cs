namespace ChewyQueue
{
    using System;
    using System.Threading.Tasks;

    using ChewyQueue.Core;

    using NLog;

    /// <summary>
    ///     The program.
    /// </summary>
    internal class Program
    {
        #region Static Fields

        /// <summary>
        ///     The logger
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        /// <summary>
        ///     Sends unhandled exceptions to airbrake.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs" /> instance containing the event data.</param>
        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            var aggregateException = exception as AggregateException;

            if (aggregateException != null)
            {
                foreach (var ex in aggregateException.InnerExceptions)
                {
                    Logger.Fatal(ex);
                }
            }
            else
            {
                Logger.Fatal(exception);
            }
        }

        /// <summary>
        ///     The main entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            Task.Run(new ChewyQueue().Start).Wait();

            Console.Write("\nPress any key to exit.");
            Console.ReadKey(true);
        }

        #endregion
    }
}