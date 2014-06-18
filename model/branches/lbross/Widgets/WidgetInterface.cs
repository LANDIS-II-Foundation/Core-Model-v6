using System;
using System.IO;

using Edu.Wisc.Forest.Flel.Util;
using log4net;

namespace Widgets
{
    /// <summary>
    /// The user interface for Landis.
    /// </summary>
    //public static class UI
    public class WidgetInterface
        : Landis.Core.IUserInterface
    {
        private static TextWriter writer;
        private static ILog log;

        //---------------------------------------------------------------------

        static WidgetInterface()
        {
            writer = TextWriter.Null;
            log = LogManager.GetLogger("Landis");
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Access the user interface as a text writer.
        /// </summary>
        /// <remarks>
        /// Default value is TextWriter.Null.
        /// </remarks>

        public TextWriter TextWriter
        {
            get
            {
                return writer;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Use System.IO.TextWriter.Null instead of null");
                writer = value;
            }
        }

        //---------------------------------------------------------------------

        public TextWriter ConsoleOut
        {
            get
            {
                return writer;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Writes a line terminator to the user interface and the application
        /// log.
        /// </summary>

        public void WriteLine()
        {
            writer.WriteLine();
            log.Info(string.Empty);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Writes a string followed by a line terminator to the user
        /// interface and the application log.
        /// </summary>

        public void WriteLine(string text)
        {
            writer.WriteLine(text);
            log.Info(text);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Writes a formatted string to the user interface and the application
        /// log, using the same semantics as System.String.Format.  A line
        /// terminator is written after the formatted string.
        /// </summary>

        public void WriteLine(string format, params object[] args)
        {
            writer.WriteLine(format, args);
            log.Info(string.Format(format, args));
        }

        //---------------------------------------------------------------------

        public ProgressBar CreateProgressMeter(int totalWorkUnits)
        {
            return new ProgressBar((uint)totalWorkUnits, System.Console.Out);
        }

    }
}
