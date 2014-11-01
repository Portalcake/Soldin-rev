#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Soldin.Net;
using Soldin.Storage;
#endregion

namespace Soldin
{
    partial class Session
    {
        #region Methods

        /// <summary>
        /// Client is performing a handshake.
        /// </summary>
        /// <param name="packet"></param>
        void OnHandshake( PacketReader packet )
        {
            // Generate a random key for the encryption. Key has to be 1 or higher. If the 
            // key is 0 it causes the client to malform certain unicode encoded strings beyond repair.
            uint key = 2789743391; // (uint)( Server.Random.Next() | 1 );

            Connection.SetEncryption( new Encryption( key ) );

            Send( PacketGenerator.Handshake( key, Connection.IP ) );
        }

        /// <summary>
        /// Client attempts to login.
        /// </summary>
        /// <param name="packet"></param>
        void OnLogin( PacketReader packet )
        {
            string accountName = packet.ReadUTF16Safe();
            string password = packet.ReadUTF8();

            Console.WriteLine( password );

            // Check if this account is already logged in.
            if ( Server.Database.IsAccountOnline( accountName ) )
            {
                Send( PacketGenerator.Login( 5, "" ) ); // Tell client account is already connected to lobby.

                return;
            }

            // Load the account.
            Account = Server.Database.GetAccount( accountName );
            if ( Account == null )
            {
                Send( PacketGenerator.Login( 1, "" ) ); // Tell client there is no account with the given name.

                return;
            }
            Server.Database.AssignAccountToSession( Key, Account.Name );

            // Get the keybindings associated with this account.
            string keybindings = Server.Database.GetKeybindings( accountName );

            // Send all the required data to the client to complete the login.
            Send( PacketGenerator.Login( 0, accountName ) );
            if ( keybindings != null )
            {
                Send( PacketGenerator.Keybindings( keybindings ) );
            }
            Send( PacketGenerator.CharacterLicenses( Account.MaxCharacters, Account.CharacterLicenses ) );
            Send( PacketGenerator.CharacterList( Account.Characters ) );
            Send( PacketGenerator.SecondaryPassword( Account.SecondaryPassword != null, 0, false ) );
        }

        /// <summary>
        /// Client attempts to create a new character.
        /// </summary>
        /// <param name="packet"></param>
        void OnCreateCharacter( PacketReader packet )
        {
            if ( !IsAuthenticated ) return;

            string characterName = packet.ReadUTF16();
            int characterClass = packet.ReadInt32();

            if ( Server.Database.IsCharacterNameAvailable( characterName ) )
            {
                Character newCharacter;
                if ( Server.Database.CreateCharacter( Account.Name, characterName, (short)characterClass, out newCharacter ) )
                {
                    Account.Characters.Add( newCharacter );

                    Send( PacketGenerator.CharacterCreate( 0, newCharacter ) );
                }
                else Send( PacketGenerator.CharacterCreate( 4, null ) );
            }
            else Send( PacketGenerator.CharacterCreate( 1, null ) );
        }

        /// <summary>
        /// Client attempts to delete a character.
        /// </summary>
        /// <param name="packet"></param>
        void OnDeleteCharacter( PacketReader packet )
        {
            if ( !IsAuthenticated ) return;
            
            // Attempt to the delete the character.
            string characterName = packet.ReadUTF16();
            foreach ( var character in Account.Characters )
            {
                if ( character.Name == characterName )
                {
                    Server.Database.DeleteCharacter( characterName );

                    Send( PacketGenerator.CharacterDelete( 0, characterName ) );

                    return;
                }
            }
            Send( PacketGenerator.CharacterDelete( 1, characterName ) );
        }

        /// <summary>
        /// Client wants to select a character and go to the square list.
        /// </summary>
        /// <param name="packet"></param>
        void OnSelectCharacter( PacketReader packet )
        {
            if ( !IsAuthenticated ) return;

            string characterName = packet.ReadUTF16();
            foreach ( var character in Account.Characters )
            {
                if ( character.Name == characterName )
                {
                    Character = character;

                    Server.Database.AssignCharacterToSession( Key, characterName );

                    Send( PacketGenerator.CharacterSelect( 0, characterName ) );
                    return;
                }
            }
            Send( PacketGenerator.CharacterSelect( 1, characterName ) );
        }

        /// <summary>
        /// Client wants to deselect the character and return to the character selection screen.
        /// </summary>
        /// <param name="packet"></param>
        void OnDeselectCharacter( PacketReader packet )
        {
            if ( !IsAuthenticated ) return;

            Character = null;
            Server.Database.AssignCharacterToSession( Key, null );

            Send( PacketGenerator.CharacterDeselect( 0 ) );
        }

        /// <summary>
        /// Client is requesting the list of squares.
        /// </summary>
        /// <param name="packet"></param>
        void OnGetSquareList( PacketReader packet )
        {
            //Send( PacketGenerator.AchievementServer() );
            //Send( PacketGenerator.FriendsInfo() );
            Send( PacketGenerator.SquareList( Server.Database.GetSquareList() ) );
        }

        /// <summary>
        /// Client has selected a square.
        /// </summary>
        /// <param name="packet"></param>
        void OnSquareSelect( PacketReader packet )
        {
            if ( !IsAuthenticated ) return;

            Square square = Server.Database.GetSquare( packet.ReadUTF16() );
            if ( square != null )
            {
                Send( PacketGenerator.SquareConnect( 0, square.IP, square.Port, Key ) );
            }
            else Send( PacketGenerator.SquareConnect( 4, "", 0, "" ) );
        }

        /// <summary>
        /// Client is disconnecting.
        /// </summary>
        /// <param name="packet"></param>
        void OnDisconnect( PacketReader packet )
        {
            Connection.Close();
        }

        /// <summary>
        /// Client has changed its keybindings.
        /// </summary>
        /// <param name="packet"></param>
        void OnSetKeybindings( PacketReader packet )
        {
            if ( !IsAuthenticated ) return;

            string accountName = packet.ReadUTF16Safe();
            string keybindings = packet.ReadUTF16Safe();

            Server.Database.SetKeybindings( Account.Name, keybindings );
        }

        #endregion

        #region Methods: Secondary Password

        /// <summary>
        /// Client has entered their secondary password.
        /// </summary>
        /// <param name="packet"></param>
        void OnSecondaryLogin( PacketReader packet )
        {
            string accountName = packet.ReadUTF16();
            string password    = packet.ReadUTF16Safe();

            if ( Account.SecondaryPassword != null && Account.SecondaryPassword != password )
            {
                //if ( FailedLoginAttemps == 3 )
                //{
                //    PacketGenerator.SecondaryPasswordResult( 3 );
                //}

                Send( PacketGenerator.SecondaryPassword( true, 0, false ) );
            }
            else Send( PacketGenerator.SecondaryPasswordResult( 0 ) );
        }

        /// <summary>
        /// Client wants to set a secondary password.
        /// </summary>
        /// <param name="packet"></param>
        void OnCreateSecondaryPassword( PacketReader packet )
        {
            if ( !IsAuthenticated ) return;

            string accountName = packet.ReadUTF16();
            string password    = packet.ReadUTF16Safe();

            int result = 1;
            if ( Account.SecondaryPassword == null )
            {
                Account.SecondaryPassword = password;

                Server.Database.SetSecondaryPassword( accountName, password );

                result = 0;
            }

            Send( PacketGenerator.SecondaryPasswordCreated( result ) );
        }

        /// <summary>
        /// Clients wnats to change their secondary password.
        /// </summary>
        /// <param name="packet"></param>
        void OnChangeSecondaryPassword( PacketReader packet )
        {
            if ( !IsAuthenticated ) return;

            string accountName = packet.ReadUTF16();
            string oldPassword = packet.ReadUTF16Safe();
            string newPassword = packet.ReadUTF16Safe();

            int result = 1;
            if ( Account.SecondaryPassword == oldPassword )
            {
                Server.Database.SetSecondaryPassword( Account.Name, newPassword );
                Account.SecondaryPassword = newPassword;

                result = 0;
            }
            Send( PacketGenerator.SecondaryPasswordChanged( result ) );
        }

        /// <summary>
        /// Client wants to remove their secondary password.
        /// </summary>
        /// <param name="packet"></param>
        void OnRemoveSecondaryPassword( PacketReader packet )
        {
            if ( !IsAuthenticated ) return;

            string accountName = packet.ReadUTF16();
            string password    = packet.ReadUTF16Safe();

            int result = 1;
            if ( Account.SecondaryPassword == password )
            {
                Server.Database.SetSecondaryPassword( Account.Name, null );

                result = 0;
            }
            Send( PacketGenerator.SecondaryPasswordRemoved( result ) );
        }

        #endregion
    }
}
