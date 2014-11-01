using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Soldin.Utilities
{
    class Program
    {
        static void Main( string[] args )
        {
            var archive = new CFPArchive(@"D:\Allm Lunia\Database\Database.cfp");
            foreach ( var entry in archive.Entries )
            {
                List( entry, 0 );
            }

            Console.ReadKey();
        }

        static void List( CFPEntry entry, int depth )
        {
            Console.WriteLine( "".PadLeft( depth * 2 ) + entry.Name );
            if ( entry.IsDirectory )
            {
                foreach ( var child in entry.Entries )
                {
                    List( child, depth + 1 );
                }
            }
        }
    }
}
