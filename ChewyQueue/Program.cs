namespace ChewyQueue
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        #region Methods

        /// <summary>
        /// The main entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            Task.Factory.StartNew(new ChewyQueue().Start).Wait();
        }

        #endregion
    }
}