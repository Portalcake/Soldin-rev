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
        #region Methods: General

        /// <summary>
        /// Client is trying to authenticate.
        /// </summary>
        /// <param name="packet"></param>
        void OnAuthenticate( PacketReader packet )
        {
            string sessionKey = packet.ReadUTF16();
            int    unknown    = packet.ReadInt32();
            string language   = packet.ReadUTF16();

            // Load account and character.
            Account   = Server.Database.GetSessionAccount( sessionKey );
            Character = Server.Database.GetSessionCharacter( sessionKey );

            Send( PacketGenerator.LoadSquare( Character.Name, Server.Square.StageGroupHash, Server.Square.StageHash ) );
            Send( PacketGenerator.CharacterInfo( Character ) );
            Send( PacketGenerator.Bags( Character ) );

            //Send( new Packet( 0x2CF3 ).WriteUInt32( 0x393CEE31 ).WriteUInt32( 0 ) ); // Inventory / Items
            //Send( new Packet( 0x2CF3 ).WriteUInt32( 0x393CEE31 ).WriteUInt32( 0 ) ); // Inventory / Items
            //Send( new Packet( 0x7414 ).WriteUInt32( 0x393C1A96 ).WriteUInt32( 0 ) ); // Skills
            //Send( new Packet( 0x0E0F ).WriteUInt32( 0x393CC9EC ).WriteUInt32( 0 ) ); // Quickslots

            //Send( new Packet( 0x19B6 ).WriteUInt32( 0xCE8E8CAD ).WriteUInt32( 0 ).WriteUInt32( 0x2AA541C1 ).WriteUInt32( 0x3EC38789 ).WriteUInt32( 0x393CD472 ).WriteUInt32( 0 ) );
            //Send( new Packet( 0xD2ED ).WriteUInt32( 0 ).WriteUInt32( 0x393C13EA ).WriteUInt32( 0 ) );

            //Send( new Packet( 0xFD30 ).WriteUInt32( 0x393C11A3 ).WriteUInt32( 0 ) ); // Pets

            // Client starts loading after the next packet.
            Send( PacketGenerator.LoadSquare2( Character.Name, Server.Square.StageGroupHash, Server.Square.StageHash ) );
        }

        /// <summary>
        /// Client is loading.
        /// </summary>
        /// <param name="packet"></param>
        void OnLoading( PacketReader packet )
        {
            float progress = packet.ReadFloat();

            Send( PacketGenerator.LoadingProgress( Character.Name, progress ) );

            // When done loading begin sending character information.
            if ( progress == 1 )
            {
                int stageId = Server.Square.Add( this );
                if ( stageId == -1 )
                {
                    // TODO: Figure out how to gracefully disconnect the client by telling it the stage is full.
                }
            }
        }

        #endregion

        #region Methods: Characters

        /// <summary>
        /// Character is trying to perform a action.
        /// </summary>
        /// <param name="packet"></param>
        void OnCharacterAction( PacketReader packet )
        {
            if ( Stage == null )
                return;

            const uint ACT_WALK = 0;
            const uint ACT_DASH = 1;
            const uint ACT_IDLE = 2;

            uint action    = packet.ReadUInt32();
            uint direction = packet.ReadUInt32();

            // TODO: Implement the dash move and proper movement for all 8 directions.

            if ( action == ACT_WALK ) // Walk
            {
                Character.LastAction = DateTime.Now;
                switch ( direction )
                {
                    case 6:
                        Character.Rotation.X = 1;
                        Character.Rotation.Y = 0;
                        Character.Rotation.Z = 0;
                        break;

                    case 4:
                        Character.Rotation.X = -1;
                        Character.Rotation.Y = 0;
                        Character.Rotation.Z = 0;
                        break;

                    case 2:
                        Character.Rotation.X = 0;
                        Character.Rotation.Y = 0;
                        Character.Rotation.Z = -1;
                        break;

                    case 8:
                        Character.Rotation.X = 0;
                        Character.Rotation.Y = 0;
                        Character.Rotation.Z = 1;
                        break;
                }

                Character.Direction = direction;
                Stage.Send( PacketGenerator.CharacterMove( Character, 0x00004E0D ) );
            }
            else if ( action == ACT_DASH ) // Dash
            {
                // TODO: Implement dashing.
            }
            else if ( action == ACT_IDLE ) // Stop Movement
            {
                TimeSpan span = DateTime.Now - Character.LastAction;
                switch ( Character.Direction )
                {
                    case 6: Character.Position.X = (Single)( Character.Position.X + span.TotalMilliseconds * 0.0425 ); break;
                    case 4: Character.Position.X = (Single)( Character.Position.X - span.TotalMilliseconds * 0.0425 ); break;
                    case 2: Character.Position.Z = (Single)( Character.Position.Z - span.TotalMilliseconds * 0.0425 ); break;
                    case 8: Character.Position.Z = (Single)( Character.Position.Z + span.TotalMilliseconds * 0.0425 ); break;
                }
                Stage.Send( PacketGenerator.CharacterMove( Character, 0x01327338 ) );
            }
        }

        #endregion
    }
}
