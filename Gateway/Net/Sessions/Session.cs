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
        /// Gets the ID of the session.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the session key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets the underlying connection of the session.
        /// </summary>
        public Connection Connection { get; set; }

        /// <summary>
        /// Gets the account associated with the session. If this is not set the client has not logged in.
        /// </summary>
        public Account Account { get; set; }

        /// <summary>
        /// Gets the character selected by the client.
        /// </summary>
        public Character Character { get; set; }

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
            Key        = DateTime.Now.Ticks.ToString() + BitConverter.ToInt32( ( (System.Net.IPEndPoint)conn.Socket.RemoteEndPoint ).Address.GetAddressBytes(), 0 );

            // Create the session in the database.
            Server.Database.CreateSession( Key );

            Server.Log.Info( "[{0}] Client {1} has connected.", Id, Connection.IP );
        }

        #endregion

        #region Methods

        /// <summary>
        /// Terminates the session.
        /// </summary>
        /// <remarks>A session gets terminated after its connection has been closed or lost.</remarks>
        public void Terminate()
        {
            Server.Database.DeleteSession( Key );

            Server.Log.Info( "[{0}] Client {1} has disconnected.", Id, Connection.IP );
        }

        /// <summary>
        /// Sends the specified <see cref="Packet"/> to the client.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        public void Send( Packet packet )
        {
            Connection.Send( packet );
        }

        /// <summary>
        /// Handles the specified packet.
        /// </summary>
        /// <param name="packet">The packet the handle.</param>
        public void Handle( PacketReader packet )
        {
            switch ( packet.Id )
            {
                case PacketID.MSG_HANDSHAKE:          OnHandshake( packet );               break;
                case PacketID.MSG_LOGIN:              OnLogin( packet );                   break;
                case PacketID.MSG_CHARACTER_CREATE:   OnCreateCharacter( packet );         break;
                case PacketID.MSG_CHARACTER_DELETE:   OnDeleteCharacter( packet );         break;
                case PacketID.MSG_CHARACTER_SELECT:   OnSelectCharacter( packet );         break;
                case PacketID.MSG_CHARACTER_DESELECT: OnDeselectCharacter( packet );       break;
                case PacketID.MSG_SQUARE_LIST:        OnGetSquareList( packet );           break;
                case PacketID.MSG_SQUARE_SELECT:      OnSquareSelect( packet );            break;
                case PacketID.MSG_DISCONNECT:         OnDisconnect( packet );              break;
                case PacketID.MSG_PING:               /* Do Nothing*/                      break;
                case PacketID.MSG_SETKEYBINDINGS:     OnSetKeybindings( packet );          break;

                // Secondary password related stuff.
                case PacketID.MSG_2NDPASSWD_LOGIN:    OnSecondaryLogin( packet );          break;
                case PacketID.MSG_2NDPASSWD_CREATE:   OnCreateSecondaryPassword( packet ); break;
                case PacketID.MSG_2NDPASSWD_CHANGE:   OnChangeSecondaryPassword( packet ); break;
                case PacketID.MSG_2NDPASSWD_REMOVE:   OnRemoveSecondaryPassword( packet ); break;

                case 0x3E1B: // Unknown
                    {
                        string accountName = packet.ReadUTF16();
                        /* This packet seems to parrot the account name we send in the login packet back to us. 
                         */
                    }
                    break;

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
