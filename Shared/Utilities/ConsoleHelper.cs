using System;
using System.Text;

namespace Soldin
{
    public static class ConsoleHelper
    {
        public static void WriteHex( string title, byte[] data, int index, int count, ConsoleColor color )
        {

            ConsoleColor oldColor = Console.ForegroundColor;

            StringBuilder hex = new StringBuilder();
            StringBuilder text = new StringBuilder();

            Console.WriteLine( title );

            Console.ForegroundColor = color;
            for ( int i = index ; i < index + count ; )
            {
                hex.Clear();
                text.Clear();

                int j;
                for ( j = 0 ; j < 12 ; j++, i++ )
                {
                    if ( i >= count )
                        break;

                    hex.AppendFormat( "{0:X2} ", data[i] );
                    if ( data[i] >= 32 && data[i] <= 128 )
                    {
                        text.AppendFormat( "{0} ", (char)data[i] );
                    }
                    else text.Append( ". " );
                }
                Console.Write( hex.ToString() + "| ".PadLeft( ( ( 12 - j ) * 3 ) + 2 ) + text.ToString() + "\n" );
            }
            Console.Write( "\n" );
            Console.ForegroundColor = oldColor;
        }
    }
}
