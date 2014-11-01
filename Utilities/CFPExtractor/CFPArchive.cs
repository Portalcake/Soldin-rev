#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
#endregion

namespace Soldin.Utilities
{
    public class CFPArchive
    {
        #region Imports

        [DllImport( "LZMA.dll", CallingConvention = CallingConvention.StdCall )]
        public static extern int LzmaUncompress( byte[] dest, ref int destLen, byte[] src, ref int srcLen, byte[] props, int propSize );

        #endregion

        #region Variables

        string   mPath;
        CFPEntry mRootEntry;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a list of entries.
        /// </summary>
        public List<CFPEntry> Entries { get { return mRootEntry.Entries; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Initalizes a new instance of the <see cref="CFPArchive"/> class. 
        /// </summary>
        /// <param name="archivePath">Path of the archive to load.</param>
        public CFPArchive( string archivePath )
        {
            mPath = archivePath;
            if ( !File.Exists( mPath ) )
                throw new FileNotFoundException( mPath );

            using ( FileStream stream = new FileStream( mPath, FileMode.Open, FileAccess.Read ) )
            using ( BinaryReader reader = new BinaryReader( stream ) )
            {
                ushort tag = reader.ReadUInt16();
                ulong magic = reader.ReadUInt64();

                if ( tag != 0x4150 )
                    return; // Invalid Archive Header.

                stream.Position = reader.ReadInt64();
                mRootEntry = new CFPEntry( reader );
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Extracts the specified entry to the specified location.
        /// </summary>
        /// <param name="entry">The entry to extract.</param>
        public byte[] Extract( CFPEntry entry )
        {
            using ( FileStream stream = new FileStream( mPath, FileMode.Open, FileAccess.Read ) )
            using ( BinaryReader reader = new BinaryReader( stream ) )
            {
                stream.Position = entry.Param1;

                int compressedSize   = reader.ReadInt32();
                int uncompressedSize = reader.ReadInt32();

                byte[] props        = reader.ReadBytes( 5 );
                byte[] compressed   = reader.ReadBytes( compressedSize );
                byte[] uncompressed = new byte[uncompressedSize];

                int result = LzmaUncompress( uncompressed, ref uncompressedSize, compressed, ref compressedSize, props, 5 );
                if ( result != 0 )
                    throw new Exception( "" );

                return uncompressed;
            }
        }

        #endregion
    }
}
