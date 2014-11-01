#region Using Statements
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Soldin.Storage;
using Soldin.Net;
#endregion

namespace Soldin
{
    static class Server
    {
        #region Properties

        /// <summary>
        /// Gets the database.
        /// </summary>
        public static Database Database { get; private set; }

        /// <summary>
        /// Gets the random number generator of the server.
        /// </summary>
        public static Random Random { get { return mRandom; } }

        /// <summary>
        /// Gets the server log.
        /// </summary>
        public static Log Log { get; private set; }

        /// <summary>
        /// Gets the server configuration.
        /// </summary>
        public static Configuration Settings { get; private set; }

        /// <summary>
        /// Gets the square stage.
        /// </summary>
        public static Stage Square { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating tcp traffic should be logged.
        /// </summary>
        public static bool TcpLogging { get; private set; }

        #endregion

        #region Variables

        static Socket mSocket;
        static bool   mListening;
        static int    mPort;
        static Random mRandom = new Random();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class that listens on the specified port.
        /// </summary>
        /// <param name="port"></param>
        public static void Run()
        {
            // Open the log file.
            Log = new Log( "stage.log" );

            // Open the configuration file.
            Settings = new Configuration( "stage.cfg" );

            // Initialize the connection with the database.
            Database = new Database();
            if ( !Database.Initialize() )
            {
                Console.ReadKey();

                return;
            }

            // Load the square.
            bool isSquare = Settings.GetBool( "stage.square" );
            if ( isSquare )
            {
                Square = new Stage(
                    Settings.GetInt( "stage.square.maxconn", 100 ),
                    Settings.GetUInt( "stage.square.stagegrouphash", 1386103375 ),
                    Settings.GetUInt( "stage.square.stagehash", 53518598 ) );
            }

            // Load settings.
            TcpLogging = Settings.GetBool( "stage.tcp.log" );

            // Initialize the listen socket.
            mPort = Settings.GetInt( "stage.tcp.port" );
            mSocket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
            mSocket.Bind( new IPEndPoint( IPAddress.Any, mPort ) );
            mSocket.Listen( 32 );

            mListening = true;
            new Thread( _ =>
            {
                Log.Info( "Now accepting connections on port {0}.", mPort );
                while ( mListening )
                {
                    new Connection( mSocket.Accept() );
                }
            } ).Start();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Stops this <see cref="Server"/>.
        /// </summary>
        public static void Stop()
        {
            mListening = false;
        }

        #endregion
    }
}
