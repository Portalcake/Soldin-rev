#region Using Statements
using System.Collections.Generic;
using Soldin.Net;
#endregion

namespace Soldin
{
    static class SessionManager
    {
        #region Variables

        static object                   mLock = new object();
        static int                      mNextId = 0;
        static Dictionary<int, Session> mSessions = new Dictionary<int, Session>();

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new session for the specified <see cref="Connection"/>.
        /// </summary>
        /// <param name="conn">The underlying connection of the new session.</param>
        /// <returns></returns>
        public static Session Create( Connection conn )
        {
            Session session = null;
            lock ( mLock )
            {
                session = new Session( mNextId++, conn );

                mSessions.Add( session.Id, session );
            }
            return session;
        }

        /// <summary>
        /// Destroys the specified session.
        /// </summary>
        /// <param name="session">The session to destroy.</param>
        public static void Destroy( Session session )
        {
            lock ( mLock )
            {
                if ( !mSessions.ContainsKey( session.Id ) )
                    return;

                session.Terminate();

                mSessions.Remove( session.Id );
            }
        }

        #endregion
    }
}
