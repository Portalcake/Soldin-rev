#region Using Statements
using System;
using System.Collections.Generic;
using Soldin.Net;
#endregion

namespace Soldin
{
    partial class Session
    {
        #region Properties

        /// <summary>
        /// Gets the ID of the <see cref="Session"/>.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets or sets the session key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets the connection of this <see cref="Session"/>.
        /// </summary>
        public Connection Connection { get; private set; }

        /// <summary>
        /// Gets the account associated with this <see cref="Session"/>. 
        /// If this is not set the client has not logged in.
        /// </summary>
        public Account Account { get; private set; }

        /// <summary>
        /// Gets the character selected by the client.
        /// </summary>
        public Character Character { get; private set; }

        /// <summary>
        /// Gets or sets the stage the client is on.
        /// </summary>
        public Stage Stage { get; set; }

        /// <summary>
        /// Gets a value indicating wether the session is authenicated.
        /// </summary>
        public bool IsAuthenticated { get { return ( Account != null ); } }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class for the specified <see cref="Connection"/>.
        /// </summary>
        /// <param name="id">ID of the session.</param>
        /// <param name="conn">Underlying connection of the session.</param>
        public Session( int id, Connection conn )
        {
            Id         = id;
            Connection = conn;

            Server.Log.Info( "[{0}] Client {1} has connected.", Id, Connection.IP );

            uint key = 2789743391;

            Connection.SetEncryption( new Encryption( key ) );
            Send( PacketGenerator.EncryptionKey( key ) );
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sends the specified <see cref="Packet"/> to the client.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        public void Send( Packet packet )
        {
            if ( Connection != null )
            {
                Connection.Send( packet );
            }
        }

        /// <summary>
        /// Destroys the <see cref="Session"/>.
        /// </summary>
        public void Destroy()
        {
            Server.Log.Info( "[{0}] Client {1} has disconnected.", Id, Connection.IP );
        }

        /// <summary>
        /// Handles the specified packet.
        /// </summary>
        /// <param name="packet">The packet the handle.</param>
        public void Handle( PacketReader packet )
        {
            switch ( packet.Id )
            {
                case PacketID.MSG_AUTHENTICATE:     OnAuthenticate( packet ); break;
                case PacketID.MSG_LOADING:          OnLoading( packet ); break;
                case PacketID.MSG_CHARACTER_ACTION: OnCharacterAction( packet ); break;

                default:
                    string fileName = string.Format( "{0:X}.bin", packet.Id );
                    System.IO.File.WriteAllBytes( fileName, packet.ReadBytes( packet.Size ) );

                    Server.Log.Notice( "[{0}] Unhandled packet (ID: {1:X}, Size: {2} bytes).", Id, packet.Id, packet.Size );
                    break;
            }
        }

        #endregion
    }
}
