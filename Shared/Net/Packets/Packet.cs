#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Soldin.Net
{
    public class Packet
    {
        #region Variables

        List<byte> mData = new List<byte>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Packet"/> class.
        /// </summary>
        /// <param name="id">ID of the packet.</param>
        public Packet( ushort id )
        {
            mData.Add( 0xE0 ); 
            mData.Add( 0x55 );
            mData.AddRange( BitConverter.GetBytes( id ) );
        }

        #endregion

        #region Methods

        /// <summary>
        /// Writes a single byte to the packet.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Packet WriteByte( byte value )
        {
            mData.Add( value );
            return this;
        }

        /// <summary>
        /// Writes a bool to the packet.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Packet WriteBool( bool value )
        {
            WriteByte( value ? (byte)1 : (byte)0 );
            return this;
        }

        /// <summary>
        /// Writes a 16-bit integer to the packet.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Packet WriteInt16( short value )
        {
            mData.AddRange( BitConverter.GetBytes( value ) );
            return this;
        }

        /// <summary>
        /// Writes a unsigned 16-bit integer to the packet.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Packet WriteUInt16( ushort value )
        {
            mData.AddRange( BitConverter.GetBytes( value ) );
            return this;
        }

        /// <summary>
        /// Writes a 32-bit integer to the packet.
        /// </summary>
        /// <param name="pValue"></param>
        /// <returns></returns>
        public Packet WriteInt32( int pValue )
        {
            mData.AddRange( BitConverter.GetBytes( pValue ) );
            return this;
        }

        /// <summary>
        /// Writes a unsigned 32-bit integer to the packet.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Packet WriteUInt32( uint value )
        {
            mData.AddRange( BitConverter.GetBytes( value ) );
            return this;
        }

        /// <summary>
        /// Writes a floating-point number to the packet.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Packet WriteFloat( float value )
        {
            mData.AddRange( BitConverter.GetBytes( value ) );
            return this;
        }

        /// <summary>
        /// Writes a UTF-8 encoded string to the packet.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Packet WriteUTF8( string value )
        {
            WriteInt16( (short)( value.Length + 1 ) );
            if ( value.Length > 0 )
            {
                mData.AddRange( Encoding.UTF8.GetBytes( value ) );
            }
            WriteByte( 0 );
            return this;
        }

        /// <summary>
        /// Writes a UTF-16 encoded string to the packet.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Packet WriteUTF16( string value )
        {
            WriteInt16( (short)( value.Length + 1 ) );
            if ( value.Length > 0 )
            {
                mData.AddRange( Encoding.Unicode.GetBytes( value ) );
            }
            WriteInt16( 0 );
            return this;
        }

        /// <summary>
        /// Writes a date/time stamp to the packet.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public Packet WriteDateTime( DateTime dateTime, uint hash )
        {
            WriteUInt32( hash );
            WriteInt16( (short)dateTime.Year );
            WriteInt16( (short)dateTime.Month );
            WriteInt16( (short)dateTime.Day );
            WriteInt16( (short)dateTime.Hour );
            WriteInt16( (short)dateTime.Minute );
            WriteInt16( (short)dateTime.Second );
            WriteInt16( 0 );
            return this;
        }

        /// <summary>
        /// Returns the packet as a byte-array.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            mData.InsertRange( 0, BitConverter.GetBytes( (short)( mData.Count + 2 ) ) );

            return mData.ToArray();
        }

        #endregion
    }
}
