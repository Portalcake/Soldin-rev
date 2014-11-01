#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
#endregion

namespace Soldin.Net
{
    class Connection
    {
        #region Variables

        Socket          mSocket;
        Session         mSession;
        AsyncCallback   mReceiveCallback;
        AsyncCallback   mSendCallback;
        MessageCallback mHandlePacketCallback;
        byte[]          mBuffer = new byte[4096];
        int             mOffset;
        object          mLock = new object();
        List<byte>      mOutgoingData = new List<byte>();
        bool            mSending = false;
        Encryption      mCryptor;

        #endregion

        #region Delegates

        delegate void MessageCallback( PacketReader packet );

        #endregion

        #region Properties

        /// <summary>
        /// Gets the session associated with the <see cref="Connection"/>.
        /// </summary>
        public Session Session { get { return mSession; } }

        /// <summary>
        /// Gets the underlying <see cref="Socket"/> of the <see cref="Connection"/>.
        /// </summary>
        public Socket Socket { get { return mSocket; } }

        /// <summary>
        /// Gets the IP address of the remote host.
        /// </summary>
        public string IP { get { return ((IPEndPoint)mSocket.RemoteEndPoint).Address.ToString(); } }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> class.
        /// </summary>
        /// <param name="socket"></param>
        public Connection( Socket socket )
        {
            mSocket = socket;

            mSendCallback    = new AsyncCallback( OnDataSent );
            mReceiveCallback = new AsyncCallback( OnDataReceived );

            // Create a session for this connection.
            mSession = SessionManager.Create( this );
            if ( mSession == null )
            {
                socket.Shutdown( SocketShutdown.Both );
                socket.Close();

                return;
            }
            mHandlePacketCallback = new MessageCallback( mSession.Handle );
            
            // Start receiving data from the remote host.
            mSocket.BeginReceive( mBuffer, 0, mBuffer.Length, SocketFlags.None, mReceiveCallback, null );
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the encryption for the <see cref="Connection"/>.
        /// </summary>
        /// <param name="cryptor"></param>
        public void SetEncryption( Encryption cryptor )
        {
            mCryptor = cryptor;
        }

        /// <summary>
        /// Sends the specified packet to the client.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        public void Send( Packet packet )
        {
            if ( packet == null )
                return;

            Send( packet.ToArray() );
        }

        /// <summary>
        /// Sends the specified data to the client.
        /// </summary>
        /// <param name="data">The data to send.</param>
        public void Send( byte[] data )
        {
            PacketState state = null;
            lock ( mLock )
            {
                // TODO: Make this log to file (and conditional).
                ConsoleHelper.WriteHex( "Sent (" + data.Length.ToString() + " bytes):", data, 0, data.Length, ConsoleColor.DarkBlue );

                mOutgoingData.AddRange( data );
                if ( !mSending )
                {
                    state = new PacketState
                    {
                        Data = mOutgoingData.ToArray(),
                        Offset = 0
                    };
                    mOutgoingData.Clear();

                    mSocket.BeginSend( state.Data, 0, state.Data.Length, SocketFlags.None, mSendCallback, state );

                    mSending = true;
                }
                else return;
            }
        }

        /// <summary>
        /// Closes this <see cref="Connection"/>.
        /// </summary>
        public void Close()
        {
            // Terminate the session associated with this connection.
            SessionManager.Destroy( mSession );

            // Close the socket.
            if ( mSocket != null )
            {
                mSocket.Shutdown( SocketShutdown.Both );
                mSocket.Close();
            }
            mSocket = null;
        }

        /// <summary>
        /// Processes the data received from the remote host.
        /// </summary>
        /// <param name="result"></param>
        void OnDataReceived( IAsyncResult result )
        {
            try
            {
                int bytesReceived = mSocket.EndReceive( result );
                if ( bytesReceived < 1 )
                {
                    Close();

                    return;
                }

                if ( mCryptor != null )
                {
                    mCryptor.Decrypt( mBuffer, mOffset, bytesReceived );
                }

                // TODO: Make this logging conditional.
                ConsoleHelper.WriteHex( "Received (" + bytesReceived.ToString() + " bytes, offset " + mOffset.ToString() + "):", mBuffer, mOffset, bytesReceived, ConsoleColor.DarkGreen );

                mOffset += bytesReceived;

                int curPos = 0;
                while ( mOffset >= ( curPos + 2 ) )
                {
                    int messageSize = ( ( mBuffer[curPos + 1] << 8 ) | mBuffer[curPos] );
                    if ( messageSize >= mBuffer.Length )
                        messageSize = mBuffer[curPos];

                    if ( mOffset >= ( curPos + messageSize ) )
                    {
                        ushort messageId = (ushort)( ( mBuffer[curPos + 5] << 8 ) | mBuffer[curPos + 4] );

                        var message = new PacketReader( messageId, mBuffer, curPos + 6, messageSize - 6 );
                        if ( mHandlePacketCallback != null )
                        {
                            mHandlePacketCallback( message );
                        }

                        curPos += messageSize;
                    }
                    else break;
                }

                if ( curPos > 0 )
                {
                    int bytesLeft = mOffset - curPos;
                    if ( bytesLeft > 0 )
                    {
                        Buffer.BlockCopy( mBuffer, curPos, mBuffer, 0, bytesLeft );
                    }
                    mOffset -= curPos;
                }

                mSocket.BeginReceive( mBuffer, mOffset, mBuffer.Length - mOffset, SocketFlags.None, mReceiveCallback, null );
            }
            catch { Close(); }
        }

        /// <summary>
        /// Sends outgoing data to the client or stops sending of there is no outgoing data available.
        /// </summary>
        /// <param name="result"></param>
        void OnDataSent( IAsyncResult result )
        {
            try
            {
                PacketState state = (PacketState)result.AsyncState;
                int sent = mSocket.EndSend( result );
                state.Offset += sent;

                // Make sure everything is being sent.
                int bytesLeft = ( state.Data.Length - sent );
                if ( bytesLeft > 0 )
                {
                    state.Offset += sent;

                    mSocket.BeginSend( state.Data, state.Offset, bytesLeft, SocketFlags.None, mSendCallback, state );

                    return;
                }

                // Begin sending the next packet or stop sending if theres no outgoing data available.
                lock ( mLock )
                {
                    if ( mOutgoingData.Count == 0 ) mSending = false;
                    else
                    {
                        state.Data = mOutgoingData.ToArray();
                        state.Offset = 0;
                        mOutgoingData.Clear();

                        mSocket.BeginSend( state.Data, 0, state.Data.Length, SocketFlags.None, mSendCallback, state );
                    }
                }
            }
            catch { Close(); }
        }

        #endregion
    }
}
