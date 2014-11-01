#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
#endregion

namespace Soldin.Utilities
{
    public class CFPEntry
    {
        #region Variables

        List<CFPEntry> mEntries = new List<CFPEntry>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the entry.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the entry.
        /// </summary>
        public ushort Type { get; set; }

        /// <summary>
        /// Gets a value indicating wether the entry is a directory.
        /// </summary>
        public bool IsDirectory { get { return Type == 0x5044; } }

        /// <summary>
        /// Gets or sets the 1st parameter of the entry.
        /// </summary>
        public uint Param1 { get; set; }

        /// <summary>
        /// Gets or sets the 2nd parameter of the entry.
        /// </summary>
        public uint Param2 { get; set; }

        /// <summary>
        /// Gets a list of subentries of the entry.
        /// </summary>
        public List<CFPEntry> Entries { get { return mEntries; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CFPEntry"/> class.
        /// </summary>
        /// <param name="reader"></param>
        public CFPEntry( BinaryReader reader )
        {
            Type = reader.ReadUInt16();

            Param1 = reader.ReadUInt32(); // Offset | DirectoryCount
            Param2 = reader.ReadUInt32(); // Size   | FileCount

            // Read the name of the entry from the file.
            ushort nameLen = reader.ReadUInt16();
            if ( nameLen > 0 )
            {
                Name = Encoding.Unicode.GetString( reader.ReadBytes( nameLen * 2 ) );
            }
            else Name = string.Empty;

            // If entry is a directory read its subdirectories and files.
            if ( IsDirectory )
            {
                uint subEntries = Param1 + Param2;
                for ( uint i = 0; i < subEntries; i++ )
                {
                    mEntries.Add( new CFPEntry( reader ) );
                }
            }
        }

        #endregion
    }
}
