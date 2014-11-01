#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
#endregion

namespace Soldin
{
    public class Configuration
    {
        #region Variables

        Dictionary<string,string> mData =new Dictionary<string, string>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initialzes a new instance of the <see cref="Configuration"/> class and loads 
        /// settings from the specified file.
        /// </summary>
        /// <param name="fileName"></param>
        public Configuration( string fileName )
        {
            using ( var reader = File.OpenText( fileName ) )
            {
                while ( !reader.EndOfStream )
                {
                    string line = reader.ReadLine().Trim();
                    if ( string.IsNullOrEmpty( line ) )
                        continue;

                    if ( line[0] == '#' || line.StartsWith( "//" ) )
                        continue;

                    int pos = line.IndexOf( '=' );
                    if ( pos != -1 )
                    {
                        string key = line.Substring( 0, pos ).Trim();
                        string value = line.Substring( pos + 1 ).Trim();

                        mData.Add( key, value );
                    }
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the integer value associated with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public int GetInt( string key, int def = 0 )
        {
            if ( !mData.ContainsKey( key ) )
                return def;

            int result = 0;
            if ( int.TryParse( mData[key], out result ) )
                return result;

            return 0;
        }

        /// <summary>
        /// Gets the unsigned integer value associated with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public uint GetUInt( string key, uint def = 0 )
        {
            if ( !mData.ContainsKey( key ) )
                return def;

            uint result = 0;
            if ( uint.TryParse( mData[key], out result ) )
                return result;

            return 0;
        }

        /// <summary>
        /// Gets the floating-point value associated with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public float GetFloat( string key, float def = 0 )
        {
            if ( !mData.ContainsKey( key ) )
                return def;

            float result = 0;
            if ( float.TryParse( mData[key], out result ) )
                return result;

            return 0;
        }

        /// <summary>
        /// Gets the boolean value associated with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public bool GetBool( string key, bool def = false )
        {
            if (!mData.ContainsKey(key))
                return def;

            string value = mData[key].ToLower();
            if ( value == "true" || value == "1" || value == "yes" || value == "y" )
                return true;

            return false;
        }

        /// <summary>
        /// Gets the string value associated with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public string GetString( string key, string def = "" )
        {
            if ( !mData.ContainsKey( key ) )
                return def;

            return mData[key];
        }

        #endregion
    }
}
