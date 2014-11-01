#region Using Statements
using System;
using System.Collections.Generic;
#endregion

namespace Soldin.Net
{
    class PacketGenerator
    {
        #region Constants

        // List hashes.
        const uint Hash_List_Characters        = 0x393CAF2B;
        const uint Hash_List_CharacterLicenses = 0x393C883E;
        const uint Hash_List_Equipment         = 0x393CD166;
        const uint Hash_List_StageLicenses     = 0x393CE8F3;
        const uint Hash_List_Squares           = 0x393CDC25;

        // Object hashes.
        const uint Hash_Obj_Character          = 0xBC715362;
        const uint Hash_Obj_NewCharacter       = 0xBC713DDA;
        const uint Hash_Obj_Square             = 0xA7AD5362;
        const uint Hash_Obj_Equipment          = 0x13D35362;

        // Date/time hashes.
        const uint Hash_DateTime_Connected     = 0x1769F8F2;
        const uint Hash_DateTime_LastPlayed    = 0x17691AC9;

        #endregion

        #region Methods

        /// <summary>
        /// Generates a handshake packet for the specified session.
        /// </summary>
        /// <param name="cryptoKey">Encryption key.</param>
        /// <param name="ipAddress">IP address of the client.</param>
        /// <returns>Handshake packet.</returns>
        public static Packet Handshake( uint cryptoKey, string ipAddress )
        {
            /*
             * Result Codes:
             *  0 - OK
             *  1 - Incorrect client version. (this forces the client to close)
             */
            return new Packet( PacketID.MSG_HANDSHAKE )
                .WriteUInt32( 0 )
                .WriteDateTime( DateTime.Now, Hash_DateTime_Connected )
                .WriteUTF8( ipAddress )
                .WriteUInt32( cryptoKey );
        }
                
        /// <summary>
        /// Generates a characterlist packet for the specified account.
        /// </summary>
        /// <param name="characters">List of characters.</param>
        /// <returns>Characterlist packet.</returns>
        public static Packet CharacterList( List<Character> characters )
        {
            var packet = new Packet( PacketID.MSG_CHARACTERLIST )
                .WriteUInt32( Hash_List_Characters )
                .WriteInt32( characters.Count ); // Character Count.

            uint nextCharacterHash = Hash_Obj_Character;
            foreach ( var character in characters )
            {
                packet.WriteUInt32( nextCharacterHash )
                    .WriteUTF16( character.Name )
                    .WriteUInt32( 0 )
                    .WriteUInt32( 0 )
                    .WriteUInt32( 0 )
                    .WriteInt32( character.Class )
                    .WriteInt16( character.StageLevel )
                    .WriteInt32( character.StageExperience )
                    .WriteInt16( character.PvpLevel )
                    .WriteInt32( character.PvpExperience )
                    .WriteInt16( character.WarLevel )
                    .WriteInt32( character.WarExperience )
                    .WriteInt16( character.RebirthLevel )
                    .WriteInt16( character.RebirthCount )
                    .WriteDateTime( character.LastLoggedOn, Hash_DateTime_LastPlayed )
                    .WriteUInt32( Hash_List_Equipment )
                    .WriteInt32( character.Equipment.Count );

                // Add the equipment of the character.
                uint nextItemHash = Hash_Obj_Equipment;
                foreach ( var item in character.Equipment )
                {
                    packet
                        .WriteUInt32( nextItemHash )
                        .WriteUInt32( item.Hash )
                        .WriteUInt32( 0x613E5DCA )
                        .WriteUInt32( 0 )
                        .WriteUInt32( 0xE4AAE5C9 )
                        .WriteUInt32( 0 )
                        .WriteUInt32( 0 )
                        .WriteUInt32( 0 );

                    nextItemHash++;
                }

                // TODO: Properly implement this stage licenses stuff.
                // Add the stage licenses of the character.
                packet
                    .WriteUInt32( Hash_List_StageLicenses )
                    .WriteInt32( 1 )
                    .WriteUInt32( 1386107746 )
                    .WriteInt32( 9919617 )
                    .WriteInt16( 0 )
                    .WriteByte( 1 )
                    .WriteUInt32( 960237690 )
                    .WriteInt32( 0 );

                packet
                    .WriteInt32( 0 )
                    .WriteInt32( 0 )
                    .WriteInt32( 0 )
                    .WriteInt32( 0 )
                    .WriteInt16( 0 );

                nextCharacterHash++;
            }
            return packet;
        }

        /// <summary>
        /// Generates a login packet for the specified session.
        /// </summary>
        /// <param name="result">Login result code.</param>
        /// <param name="accountName"></param>
        /// <returns>Login packet.</returns>
        public static Packet Login( int result, string accountName )
        {
            /*
             * Result Codes:
             *  0 - OK
             *  1 - This user does not exist.
             *  2 - Incorrect password.
             *  3 - This account was deleted.
             *  4 - Your account has been blocked.
             *  5 - You are already connected to the lobby.
             *  6 - You are already connected to the game server.
             *  7 - Unknown log-in error.
             */
            return new Packet( PacketID.MSG_LOGIN )
                .WriteInt32( result )
                .WriteUTF16( accountName )
                .WriteUInt32( 0 );
        }

        /// <summary>
        /// Generates a character licenses packet.
        /// </summary>
        /// <param name="maxCharacters">Maximum amount of characters.</param>
        /// <param name="licenses">List containing character licenses.</param>
        /// <returns>CharacterLicenses packet.</returns>
        public static Packet CharacterLicenses( int maxCharacters, List<int> licenses )
        {
            var packet = new Packet( PacketID.MSG_CHARACTERLICENSES )
                .WriteInt32( maxCharacters )
                .WriteUInt32( Hash_List_CharacterLicenses );

            packet.WriteInt32( licenses.Count );
            foreach ( int license in licenses )
            {
                packet.WriteInt32( license );
            }
            return packet;
        }

        /// <summary>
        /// Generates a packet that informs the client a character has been created.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        public static Packet CharacterCreate( int result, Character character )
        {
            /*
             * Result Codes:
             *  0 - OK
             *  1 - The character name is already in use.
             *  2 - This name cannot be used because it's deleted.
             *  3 - Incorrect user name.
             *  4 - Failed to create a character.
             *  5 - Failed to create a character.
             *  6 - You cannot create character while server migration request period.
             *  7 - Failed to create a character.
             */
            var packet = new Packet( PacketID.MSG_CHARACTER_CREATE )
                .WriteInt32( result )
                .WriteUInt32( Hash_Obj_NewCharacter );

            if ( character != null )
            {
                packet
                    .WriteUTF16( character.Name )
                    .WriteInt32( 0 )
                    .WriteInt32( 0 )
                    .WriteInt32( 0 )
                    .WriteInt32( character.Class )
                    .WriteInt16( character.StageLevel )
                    .WriteInt32( character.StageExperience )
                    .WriteInt16( character.PvpLevel )
                    .WriteInt32( character.PvpExperience )
                    .WriteInt16( character.WarLevel )
                    .WriteInt32( character.WarExperience )
                    .WriteInt16( character.RebirthLevel )
                    .WriteInt16( character.RebirthCount )
                    .WriteDateTime( character.LastLoggedOn, Hash_DateTime_LastPlayed )
                    .WriteUInt32( Hash_List_Equipment )
                    .WriteInt32( 0 )
                    .WriteUInt32( Hash_List_StageLicenses )
                    .WriteInt32( 0 )
                    .WriteInt32( 0 )
                    .WriteInt32( 0 )
                    .WriteInt32( 0 )
                    .WriteInt32( 0 )
                    .WriteInt16( 0 );
            }
            else
            {
                packet
                    .WriteUTF16( "" )
                    .WriteInt32( 0 )
                    .WriteInt32( 0 )
                    .WriteInt32( 0 )
                    .WriteInt32( 1 )
                    .WriteInt16( 1 )
                    .WriteInt32( 0 )
                    .WriteInt16( 1 )
                    .WriteInt32( 0 )
                    .WriteInt16( 1 )
                    .WriteInt32( 0 )
                    .WriteInt16( 0 )
                    .WriteInt16( 0 )
                    .WriteDateTime( DateTime.MinValue, Hash_DateTime_LastPlayed )
                    .WriteUInt32( Hash_List_Equipment )
                    .WriteInt32( 0 )
                    .WriteUInt32( Hash_List_StageLicenses )
                    .WriteInt32( 0 )
                    .WriteInt32( 0 )
                    .WriteInt32( 0 )
                    .WriteInt32( 0 )
                    .WriteInt32( 0 )
                    .WriteInt16( 0 );
            }

            return packet;
        }

        /// <summary>
        /// Generates the packet 
        /// </summary>
        /// <param name="result">Result code.</param>
        /// <param name="characterName">Name of the deleted character.</param>
        /// <returns></returns>
        public static Packet CharacterDelete( int result, string characterName )
        {
            /*
             * Result Codes:
             *  0 - OK
             *  1 - Character does not exist.
             *  2 - Guild master cannot delete character.
             *  3 - Guild master cannot delete character. Leave guild first.
             *  4 - It's wrong identification number. You cannot delete your character
             */
            return new Packet( PacketID.MSG_CHARACTER_DELETE )
                .WriteInt32( result )
                .WriteUTF16( characterName );
        }

        /// <summary>
        /// Selects the specified character.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="characterName">Name of the selected character.</param>
        /// <returns></returns>
        public static Packet CharacterSelect( int result, string characterName )
        {
            return new Packet( PacketID.MSG_CHARACTER_SELECT )
                .WriteInt32( result )
                .WriteUTF16( characterName )
                .WriteInt32( 0 );
        }

        /// <summary>
        /// Generates the packet that tells the client to return to the character selection screen.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Packet CharacterDeselect( int result )
        {
            return new Packet( PacketID.MSG_CHARACTER_DESELECT ).WriteInt32( result );
        }

        /// <summary>
        /// Generates the squarelist packet.
        /// </summary>
        /// <returns></returns>
        public static Packet SquareList( List<Square> squares )
        {
            var packet = new Packet( PacketID.MSG_SQUARE_LIST )
                .WriteInt32( 0 )
                .WriteUInt32( Hash_List_Squares )
                .WriteInt32( squares.Count );

            uint nextSquareHash = Hash_Obj_Square;
            foreach ( var square in squares )
            {
                packet
                    .WriteUInt32( nextSquareHash )
                    .WriteUTF16( square.Name )
                    .WriteInt32( square.Status )
                    .WriteUInt32( square.Unknown )
                    .WriteInt32( (int)square.Type )
                    .WriteInt32( square.Capacity )
                    .WriteInt32( square.Id );

                nextSquareHash++;
            }
            return packet;
        }

        /// <summary>
        /// Gets a packet that will make the client connect to a square.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="squareIp">The IP of the square.</param>
        /// <param name="squarePort">The port of the square.</param>
        /// <param name="sessionKey">The key of the client its session.</param>
        /// <returns></returns>
        public static Packet SquareConnect( int result, string squareIp, short squarePort, string sessionKey )
        {
            /*
             * Result Codes:
             *  0 - OK
             *  2 - There is a room using the same name.
             *  3 - Failed to find the stage bearing the same name.
             *  4 - Failed to find the stage.
             *  5 - You cannot access the stage.
             *  6 - You cannot enter the stage because its full.
             *  7 - You are not authorized to access.
             *  8 - Unable to find server.
             *  9 - Unknown error.
             */
            return new Packet( PacketID.MSG_SQUARE_CONNECT )
                .WriteInt32( result )
                .WriteUTF8( squareIp )
                .WriteInt16( squarePort )
                .WriteUTF8( sessionKey );
        }

        /// <summary>
        /// Generates a packet that tells the client wether a secondary password is required,
        /// how much attemps at the secondary password they have left etc...
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="attemptsFailed"></param>
        /// <param name="attemptsExceeded"></param>
        /// <returns></returns>
        public static Packet SecondaryPassword( bool enabled, int attemptsFailed, bool attemptsExceeded )
        {
            return new Packet( PacketID.MSG_2NDPASSWD )
                .WriteBool( enabled )
                .WriteInt32( attemptsFailed )
                .WriteUTF16( DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss" ) )
                .WriteBool( attemptsExceeded )
                .WriteUInt32( 0 );
        }

        /// <summary>
        /// Generates a packet that tells the client the secondary password was accepted.
        /// </summary>
        /// <returns></returns>
        public static Packet SecondaryPasswordResult( int result )
        {
            /*
             * Result Codes:
             *  1 - OK
             *  2 - DO NOT USE - Causes client to crash.
             *  3 - Maximum number of attempts exceeded.
             */
            return new Packet( PacketID.MSG_2NDPASSWD_OK )
                .WriteInt32( result );
        }

        /// <summary>
        /// Gets a packet that tells the client a secondary password has been set.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Packet SecondaryPasswordCreated( int result )
        {
            /*
             * Result Codes:
             *  0 - OK
             *  1 - Currently you are using 2nd Password.
             *  2 - Validation of 2nd Password has failed.
             */
            return new Packet( PacketID.MSG_2NDPASSWD_CREATED )
                .WriteInt32( result );
        }

        /// <summary>
        /// Generates a packet that tells the client the secondary password has been changed.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Packet SecondaryPasswordChanged( int result )
        {
            /*
             * Result Codes:
             *  0 - OK
             *  1 - Validation of 2nd Password has failed.
             */
            return new Packet( PacketID.MSG_2NDPASSWD_CHANGED )
                .WriteInt32( result );
        }

        /// <summary>
        /// Generates a packet that tells the client the secondary password has been removed.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Packet SecondaryPasswordRemoved( int result )
        {
            /*
             * Result Codes:
             *  0 - OK
             *  1 - Validation of 2nd Password has failed.
             */
            return new Packet( PacketID.MSG_2NDPASSWD_REMOVED )
                .WriteInt32( result );
        }

        /// <summary>
        /// Generates a packet that tells the client its keybindings.
        /// </summary>
        /// <param name="keybindings"></param>
        /// <returns></returns>
        public static Packet Keybindings( string keybindings )
        {
            return new Packet( PacketID.MSG_KEYBINDINGS )
                .WriteUTF16( keybindings );
        }

        #endregion
    }
}
