using System;
using System.IO;
using Landis.Utilities;

namespace Landis.Core
{
    /// <summary>
    /// The user interface for Landis.
    /// </summary>
    //public static class UI
    public interface IUserInterface
    {
        /// <summary>
        /// Access the user interface as a text writer.
        /// </summary>
        /// <remarks>
        /// Default value is TextWriter.Null.
        /// </remarks>
        TextWriter TextWriter
        {
            get;
            set;
        }

        //---------------------------------------------------------------------

        TextWriter ConsoleOut
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Writes a line terminator to the user interface and the application
        /// log.
        /// </summary>
        void WriteLine();
        
        //---------------------------------------------------------------------

        /// <summary>
        /// Writes a string followed by a line terminator to the user
        /// interface and the application log.
        /// </summary>
        void WriteLine(string text);
        
        //---------------------------------------------------------------------

        /// <summary>
        /// Writes a formatted string to the user interface and the application
        /// log, using the same semantics as System.String.Format.  A line
        /// terminator is written after the formatted string.
        /// </summary>
        void WriteLine(string format, params object[] args);

        //---------------------------------------------------------------------
        
        ProgressBar CreateProgressMeter(int totalWorkUnits);

        //---------------------------------------------------------------------
    }
}
