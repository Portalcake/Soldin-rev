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
        const uint Hash_List_Skills            = 0x393C1A96;
        const uint Hash_List_QuickSlots        = 0x393CC9EC;

        // Object hashes.
        const uint Hash_Obj_Character          = 0xBC715362;
        const uint Hash_Obj_NewCharacter       = 0xBC713DDA;
        const uint Hash_Obj_Square             = 0xA7AD5362;
        const uint Hash_Obj_Equipment          = 0x13D35362;
        const uint Hash_Obj_Bag                = 0xDAA95362;

        // Date/time hashes.
        const uint Hash_DateTime_Connected     = 0x1769F8F2;
        const uint Hash_DateTime_LastPlayed    = 0x17691AC9;
        const uint Hash_DateTime_BagExpiration = 0x1769D5AF;

        #endregion

        #region Methods

        /// <summary>
        /// Generates the encryption key packet.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Packet EncryptionKey( uint key )
        {
            return new Packet( PacketID.MSG_ENCRYPTIONKEY )
                .WriteUInt32( key );
        }

        public static Packet LoadSquare( string characterName, uint stageGroupHash, uint stageHash )
        {
            return new Packet( 0xE9FD )
                .WriteUTF16( characterName )
                .WriteUInt32( stageGroupHash )
                .WriteUInt32( stageHash )
                .WriteUInt16( 0 )
                .WriteByte( 0 )
                .WriteUInt32( 0x393C107A )
                .WriteUInt32( 0 );
        }

        public static Packet LoadSquare2( string characterName, uint stageGroupHash, uint stageHash )
        {
            return new Packet( 0x7260 )
                .WriteUTF16( characterName )
                .WriteUInt32( stageGroupHash )
                .WriteUInt32( stageHash )
                .WriteUInt16( 0 )
                .WriteByte( 0 )
                .WriteUInt32( 0x393C107A )
                .WriteUInt32( 0 );
        }

        /// <summary>
        /// Generates a character information packet for the specified character.
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static Packet CharacterInfo( Character character )
        {
            return new Packet( PacketID.MSG_CHARACTER_INFO )
                .WriteInt32( character.Class )
                .WriteInt16( character.StageLevel ) 
                .WriteInt32( character.StageExperience )
                .WriteInt16( character.PvpLevel )
                .WriteInt32( character.PvpExperience )
                .WriteInt16( character.WarLevel )
                .WriteInt32( character.WarExperience )
                .WriteInt16( character.RebirthLevel )
                .WriteInt16( character.RebirthCount )
                .WriteInt32( character.BagMoney )
                .WriteInt16( 0 )
                .WriteInt16( 0 )
                .WriteInt16( 0 ) // Lives?
                .WriteInt16( character.SkillPoints ) // Skill Points
                .WriteInt16( 0 ) // Rebirth Skill Points?
                .WriteInt16( 0 ) // Extra Skill Points?
                .WriteInt32( character.PvpRating )
                .WriteInt16( 0 )
                .WriteByte( 0 )
                .WriteInt32( 0 )
                .WriteFloat( 0.0078125f )
                .WriteInt32( 0 );
        }

        /// <summary>
        /// Generates a character update packet.
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static Packet CharacterUpdate( Character character )
        {
            return new Packet( PacketID.MSG_CHARACTER_UPDATE )
                    .WriteUInt32( 1000 ) // Id
                    .WriteFloat( character.Position.X )
                    .WriteFloat( character.Position.Y )
                    .WriteFloat( character.Position.Z )
                    .WriteFloat( character.Rotation.X )
                    .WriteFloat( character.Rotation.Y )
                    .WriteFloat( character.Rotation.Z )
                    .WriteFloat( character.Health )
                    .WriteFloat( character.Mana )
                    .WriteUInt32( 1000 ) // Id
                    .WriteUInt32( 0 ) // ?
                    .WriteUInt32( 134217728 )
                    .WriteUInt32( 0 ); // ?
        }

        /// <summary>
        /// Generates a character details packet.
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static Packet CharacterDetails( Character character )
        {
            var packet = new Packet( PacketID.MSG_CHARACTER_DETAILS )
                .WriteUInt32( 1000 )
                .WriteInt32( character.Class )
                .WriteUTF16( character.Name )
                .WriteInt16( character.StageLevel )
                .WriteInt16( character.PvpLevel )
                .WriteInt16( character.WarLevel )
                .WriteInt16( character.RebirthLevel )
                .WriteInt16( character.RebirthCount )
                .WriteInt32( character.PvpRating )
                .WriteInt32( 0 ) // ?
                .WriteInt32( 10 ) // Title ?
                .WriteInt16( 0 ) // ?
                .WriteInt32( character.AchievementPoints )
                .WriteInt16( 0 ) // ?
                .WriteFloat( character.Position.X )
                .WriteFloat( character.Position.Y )
                .WriteFloat( character.Position.Z )
                .WriteFloat( character.Rotation.X )
                .WriteFloat( character.Rotation.Y )
                .WriteFloat( character.Rotation.Z )
                .WriteFloat( character.Health )
                .WriteInt16( character.Team )

                .WriteUInt32( 0x393CD166 )
                .WriteInt32( character.Equipment.Count );

            uint nextItemHash = 0x80D15362;
            foreach ( var item in character.Equipment )
            {
                packet
                    .WriteUInt32( nextItemHash )
                    .WriteInt16( item.Position )
                    .WriteUInt32( item.Hash )
                    .WriteUInt32( 0xE4AAE5C9 )
                    .WriteUInt32( 0 )
                    .WriteUInt32( 0 )
                    .WriteUInt32( 0x001FC802 );

                nextItemHash++;
            }

            packet
                .WriteUInt32( 0x393C9E59 ).WriteUInt32( 0 )
                .WriteUInt32( 0x393C1276 ).WriteUInt32( 0 )
                .WriteByte( 1 )
                .WriteUInt32( 0x393C61D4 ).WriteUInt32( 0 )
                .WriteUInt32( 0 )
                .WriteUInt16( 16 )
                .WriteDateTime( DateTime.Now, 0x1769D555 )
                .WriteUInt32( 1 )
                .WriteFloat( 1 );

            

            return packet;
        }

        public static Packet Bags( Character character )
        {
            return new Packet( 0xFA8D )
                .WriteUInt32( 0x393CE1CC )
                .WriteUInt32( 0 )
                .WriteUInt32( 0x393CCDE9 )
                .WriteInt32( 0 );
        }

        /// <summary>
        /// Generates a packet that sets the current position, direction and action of a character.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Packet CharacterMove( Character character, uint action )
        {
            return new Packet( PacketID.MSG_CHARACTER_MOVE )
                .WriteUInt32( 1000 )
                .WriteUInt32( action )
                .WriteFloat( character.Position.X )
                .WriteFloat( character.Position.Y )
                .WriteFloat( character.Position.Z )
                .WriteFloat( character.Rotation.X )
                .WriteFloat( character.Rotation.Y )
                .WriteFloat( character.Rotation.Z )
                .WriteInt32( 0 );
        }


        /// <summary>
        /// Generates a skills list packet.
        /// </summary>
        /// <returns></returns>
        public static Packet Skills()
        {
            // TODO: Fully implement this.
            return new Packet( PacketID.MSG_SKILLS )
                .WriteUInt32( Hash_List_Skills )
                .WriteInt32( 0 );
        }

        /// <summary>
        /// Generates a quickslots packet.
        /// </summary>
        /// <returns></returns>
        public static Packet QuickSlots()
        {
            // TODO: Fully implement this.
            return new Packet( PacketID.MSG_QUICKSLOTS )
                .WriteUInt32( Hash_List_QuickSlots )
                .WriteInt32( 0 );
        }

        /// <summary>
        /// Generates a packet that reports the loading progress back to the client.
        /// </summary>
        /// <param name="characterName"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static Packet LoadingProgress( string characterName, float progress )
        {
            return new Packet( PacketID.MSG_LOADING )
                .WriteUTF16( characterName )
                .WriteFloat( progress );
        }

        #endregion
    }
}
