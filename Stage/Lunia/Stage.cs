#region Using Statements
using System;
using System.Linq;
using Soldin.Net;
#endregion

namespace Soldin
{
    class Stage
    {
        #region Variables

        Session[] mSessions;
        int       mMaxCharacters;
        object    mLock = new object();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the stage group hash of the stage.
        /// </summary>
        public uint StageGroupHash { get; private set; }

        /// <summary>
        /// Gets the stage hash of the stage.
        /// </summary>
        public uint StageHash { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Stage"/> class.
        /// </summary>
        /// <param name="maxCharacters"></param>
        /// <param name="stageGroupHash"></param>
        /// <param name="stageHash"></param>
        public Stage( int maxCharacters, uint stageGroupHash, uint stageHash )
        {
            mMaxCharacters = maxCharacters;
            mSessions      = new Session[mMaxCharacters];
            StageGroupHash = stageGroupHash;
            StageHash      = stageHash;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specified session to the stage.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public int Add( Session session )
        {
            if ( mSessions.Contains( session ) )
                return -1;

            int virtualId = 0;
            lock ( mLock )
            {
                for ( int i = 0; i < mMaxCharacters; i++ )
                {
                    if ( mSessions[i] != null )
                        continue;

                    virtualId = i;
                    mSessions[i] = session;

                    session.Character.VirtualID = 1000 + i;
                    session.Stage = this;

                    break;
                }
            }

            if ( virtualId != -1 )
                Synchronize( session );

            return virtualId;
        }

        /// <summary>
        /// Removes the specified session from the stage.
        /// </summary>
        /// <param name="session"></param>
        public void Remove( Session session )
        {
            lock ( mLock )
            {
                for ( int i = 0; i < mMaxCharacters; i++ )
                {
                    if ( mSessions[i] == session )
                    {
                        mSessions[i] = null;

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Sends the specified packet to all characters in this stage.
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="filter"></param>
        public void Send( Packet packet )
        {
            foreach ( var session in mSessions )
            {
                if ( session == null )
                    continue;

                session.Send( packet );
            }
        }

        /// <summary>
        /// Synchronizes the specified character with everyone in the stage.
        /// </summary>
        /// <param name="session"></param>
        void Synchronize( Session session )
        {
            // Send the character details to all connected clients.
            Send( PacketGenerator.CharacterDetails( session.Character ) );
            Send( PacketGenerator.CharacterUpdate( session.Character ) );

            // TODO: Send objects, npc's, etc to the specified session.
        }

        #endregion
    }
}
