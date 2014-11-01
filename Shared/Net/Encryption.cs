namespace Soldin.Net
{
    public class Encryption
    {
        #region Variables

        static byte[] mDecrypt;
        uint          mKey;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the decoding table.
        /// </summary>
        static Encryption()
        {
            mDecrypt = new byte[65536];
            for ( int i = 0 ; i < 256 ; i++ )
            {
                byte t1 = (byte)( ( i * 73 ) ^ 21 );
                byte t2 = (byte)( ( t1 * 73 ) ^ 21 );

                for ( int j = 0 ; j < 256 ; j++ )
                {
                    byte val = (byte)( ( ( ( j * 73 ) ^ 21 ) + t2 ) ^ 23 );

                    mDecrypt[( t1 << 8 ) | val] = (byte)j;
                }
            }
            return;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Encryption"/> class.
        /// </summary>
        /// <param name="pKey">The encryption key.</param>
        public Encryption( uint pKey )
        {
            mKey = pKey;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Decrypts the contents of the specified byte-array.
        /// </summary>
        /// <param name="data">The data to decode.</param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public void Decrypt( byte[] data, int index, int count )
        {
            for ( int i = index ; i < index + count ; i++ )
            {
                byte offset = (byte)( ( ( mKey++ ) & 0xFF ) + 4 );
                for ( byte j = 1 ; j < 4 ; ++j )
                {
                    offset += (byte)( ( ( ( mKey >> ( j * 8 ) ) & 0xFF ) * 73 ) ^ 21 );
                }
                data[i] = mDecrypt[( offset << 8 ) | data[i]];
            }
        }

        #endregion
    }
}
