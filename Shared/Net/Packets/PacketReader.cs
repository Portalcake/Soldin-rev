#region Using Statements
using System;
using System.Text;
#endregion

namespace Soldin.Net
{
    public class PacketReader
    {
        #region Variables

        byte[] mData;
        int    mOffset;
        ushort mId;
        int    mSize;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ID of this <see cref="PacketReader"/>.
        /// </summary>
        public ushort Id { get { return mId; } }

        /// <summary>
        /// Gets the size of this <see cref="PacketReader"/>.
        /// </summary>
        public int Size { get { return mSize; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketReader"/> class.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="payload"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public PacketReader( ushort id, byte[] payload, int index, int count )
        {
            mId     = id;
            mData   = payload;
            mOffset = index;
            mSize   = count;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reads a single byte from the packet.
        /// </summary>
        /// <returns>A single byte.</returns>
        public byte ReadByte()
        {
            return mData[mOffset++];
        }

        /// <summary>
        /// Reads the specified amount of bytes from the packet.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public byte[] ReadBytes( int count )
        {
            byte[] data = new byte[count];
            Buffer.BlockCopy( mData, mOffset, data, 0, count );

            mOffset += count;

            return data;
        }

        /// <summary>
        /// Reads a 16-bit integer from the packet.
        /// </summary>
        /// <returns>A 16-bit integer.</returns>
        public int ReadInt16()
        {
            int result = ( mData[mOffset + 1] << 8 ) | mData[mOffset];
            mOffset += 2;

            return result;
        }

        /// <summary>
        /// Reads a unsigned 16-bit integer from the packet.
        /// </summary>
        /// <returns></returns>
        public uint ReadUInt16()
        {
            return (uint)ReadInt16();
        }

        /// <summary>
        /// Reads a 32-bit integer from the packet.
        /// </summary>
        /// <returns>A 32-bit integer.</returns>
        public int ReadInt32()
        {
            int result = ( mData[mOffset + 3] << 24 ) | ( mData[mOffset + 2] << 16 ) | ( mData[mOffset + 1] << 8 ) | mData[mOffset];
            mOffset += 4;

            return result;
        }

        /// <summary>
        /// Reads a unsigned 32-bit integer from the packet.
        /// </summary>
        /// <returns></returns>
        public uint ReadUInt32()
        {
            return (uint)ReadInt32();
        }

        /// <summary>
        /// Reads a UTF-8 encoded string from the packet.
        /// </summary>
        /// <returns>A string.</returns>
        public string ReadUTF8()
        {
            int size = ReadInt16();
            if ( size > 0 )
            {
                string result = Encoding.UTF8.GetString( mData, mOffset, size - 1 );

                mOffset += size;

                return result;
            }
            return String.Empty;
        }

        /// <summary>
        /// Reads a UTF-16 encoded string from the packet.
        /// </summary>
        /// <returns></returns>
        public string ReadUTF16()
        {
            int size = ReadInt16();
            if ( size > 0 )
            {
                size *= 2;

                string result = Encoding.Unicode.GetString( mData, mOffset, size - 2 );

                mOffset += size;

                return result;
            }
            return String.Empty;
        }

        /// <summary>
        /// Safely reads a UTF-8 encoded string from the packet.
        /// </summary>
        /// <remarks>
        /// The client has a tendency to malform certain UTF-16 strings by inserting random garbage
        /// in the 2nd byte of certain characters. This method allows for safe reading of these strings
        /// by only reading for first byte of each character.
        /// </remarks>
        /// <returns></returns>
        public string ReadUTF16Safe()
        {
            int size = ReadInt16();
            if ( size > 0 )
            {
                byte[] data = new byte[size];
                for ( int i = 0 ; i < size ; i++ )
                {
                    data[i] = mData[mOffset + i * 2];
                }

                string result = Encoding.UTF8.GetString( data, 0, size - 1 );

                mOffset += ( size * 2 );

                return result;
            }
            return String.Empty;
        }

        /// <summary>
        /// Reads a floating-point number from the packet.
        /// </summary>
        /// <returns></returns>
        public float ReadFloat()
        {
            float result = BitConverter.ToSingle( mData, mOffset );
            mOffset += 4;

            return result;
        }

        #endregion
    }
}
