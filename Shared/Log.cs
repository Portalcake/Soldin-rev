#region Using Statements
using System;
using System.IO;
#endregion

namespace Soldin
{
    public class Log
    {
        #region Variables

        StreamWriter mStream;

        #endregion

        #region Constructor

        /// <summary>
        /// Opens the log file for writing.
        /// </summary>
        public Log(string fileName)
        {
            mStream = new StreamWriter( File.Open( fileName, FileMode.Append, FileAccess.Write, FileShare.Read ) );
            mStream.AutoFlush = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Writes a informational message to the log.
        /// </summary>
        /// <param name="message">The message to write to the log.</param>
        /// <param name="args"></param>
        public void Info( string message, params object[] args )
        {
            Write( LogLevel.Info, message, args );
        }

        /// <summary>
        /// Writes a error message to the log.
        /// </summary>
        /// <param name="message">The message to write to the log.</param>
        /// <param name="args"></param>
        public void Error( string message, params object[] args )
        {
            Write( LogLevel.Error, message, args );
        }

        /// <summary>
        /// Writes a warning message to the log.
        /// </summary>
        /// <param name="message">The message to write to the log.</param>
        /// <param name="args"></param>
        public void Warning( string message, params object[] args )
        {
            Write( LogLevel.Warning, message, args );
        }

        /// <summary>
        /// Writes a notice to the log.
        /// </summary>
        /// <param name="message">The message to write to the log.</param>
        /// <param name="args"></param>
        public void Notice( string message, params object[] args )
        {
            Write( LogLevel.Notice, message, args );
        }

        /// <summary>
        /// Writes the specified message to the log.
        /// </summary>
        /// <param name="level">The log level of the message.</param>
        /// <param name="message">The message to write to the log.</param>
        /// <param name="args"></param>
        void Write( LogLevel level, string message, params object[] args )
        {
            string timestamp = "[" + DateTime.Now.ToString( "s" ) + "] ";

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write( timestamp );

            switch ( level )
            {
                case LogLevel.Error: Console.ForegroundColor = ConsoleColor.Red; break;
                case LogLevel.Notice: Console.ForegroundColor = ConsoleColor.White; break;
                case LogLevel.Info: Console.ForegroundColor = ConsoleColor.Gray; break;
                case LogLevel.Warning: Console.ForegroundColor = ConsoleColor.Yellow; break;
            }

            string entry =  String.Format( message, args );
            Console.WriteLine( entry );

            mStream.WriteLine( timestamp + entry );
            
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        #endregion

        #region Enums

        /// <summary>
        /// Identifies the level of a log message.
        /// </summary>
        enum LogLevel
        {
            Info,
            Notice,
            Warning,
            Error
        }

        #endregion
    }
}
