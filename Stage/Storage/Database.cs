#region Using Statements
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
#endregion

namespace Soldin.Storage
{
    class Database
    {
        #region Variables

        string mConnStr;

        #endregion

        #region Methods: Initialization

        public bool Initialize()
        {
            var connStrBuilder = new MySqlConnectionStringBuilder();
            connStrBuilder.Server          = Server.Settings.GetString( "stage.mysql.host", "localhost" );
            connStrBuilder.Port            = (uint)Server.Settings.GetInt( "stage.mysql.port", 3306 ); ;
            connStrBuilder.UserID          = Server.Settings.GetString( "stage.mysql.username", "root" );
            connStrBuilder.Password        = Server.Settings.GetString( "stage.mysql.password", "" );
            connStrBuilder.Database        = Server.Settings.GetString( "stage.mysql.database", "soldinrev" );
            connStrBuilder.MinimumPoolSize = (uint)Server.Settings.GetInt( "stage.mysql.minpoolsize", 5 );
            connStrBuilder.MaximumPoolSize = (uint)Server.Settings.GetInt( "stage.mysql.maxpoolsize", 50 );
            connStrBuilder.Pooling = true;

            mConnStr = connStrBuilder.ToString();
            using ( var conn = new MySqlConnection( mConnStr ) )
            {
                try
                {
                    conn.Open();

                    Server.Log.Info( "Succesfully connected to database (MySQL {0}).", conn.ServerVersion );
                }
                catch ( MySqlException ex )
                {
                    Server.Log.Error( "Failed to connect to database. Reason: {0}", ex.Message );

                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Methods: Accounts

        public Account GetAccount( string accountName )
        {
            using ( var conn = new MySqlConnection( mConnStr ) )
            {
                conn.Open();
                using ( var command = conn.CreateCommand() )
                {
                    Account account;

                    command.CommandText = "SELECT * FROM `accounts` WHERE `Name` = ?Name";
                    command.Parameters.AddWithValue( "?Name", accountName );
                    using ( var reader = command.ExecuteReader() )
                    {
                        if ( !reader.HasRows || !reader.Read() )
                            return null;

                        account = new Account
                        {
                            Name              = reader.GetString( 0 ),
                            Password          = reader.GetString( 1 ),
                            CreatedOn         = ( !reader.IsDBNull( 2 ) ? reader.GetDateTime( 2 ) : DateTime.MinValue ),
                            LastLoginOn       = ( !reader.IsDBNull( 3 ) ? reader.GetDateTime( 3 ) : DateTime.MinValue ),
                            MaxCharacters     = reader.GetInt32( 5 ),
                            SecondaryPassword = reader.IsDBNull( 6 ) ? null : reader.GetString( 6 )
                        };
                    }

                    return account;
                }
            }
        }

        #endregion

        #region Methods: Characters

        public Character GetCharacter( string characterName )
        {
            Character character;
            using ( var conn = new MySqlConnection( mConnStr ) )
            {
                conn.Open();
                using ( var command = conn.CreateCommand() )
                {
                    command.CommandText = "SELECT * FROM `characters` WHERE `Name` = ?Name";
                    command.Parameters.AddWithValue("?Name", characterName);

                    using ( var reader = command.ExecuteReader() )
                    {
                        if (!reader.HasRows || !reader.Read())
                            return null;
                        
                        character = new Character();
                        character.Account         = reader.GetString(0);
                        character.Name            = reader.GetString(1);
                        character.Class           = reader.GetInt16( 2 );
                        character.StageLevel      = reader.GetInt16( 3 );
                        character.StageExperience = reader.GetInt32( 4 );
                        character.PvpLevel        = reader.GetInt16( 5 );
                        character.PvpExperience   = reader.GetInt32( 6 );
                        character.WarLevel        = reader.GetInt16( 7 );
                        character.WarExperience   = reader.GetInt32( 8 );
                        character.CreatedOn       = reader.GetDateTime( 9 );
                        character.LastLoggedOn    = reader.GetDateTime( 10 );
                        character.SkillPoints     = reader.GetInt16( 11 );
                        character.BagCount        = reader.GetInt16( 12 );
                        character.BagMoney        = reader.GetInt32( 13 );
                        character.BankBagCount    = reader.GetInt16( 14 );
                        character.BankMoney       = reader.GetInt32( 15 );
                        character.RebirthLevel    = reader.GetInt16( 16 );
                        character.RebirthCount    = reader.GetInt16( 17 );

                        character.Equipment = GetCharacterEquipment( character.Name );

                        // TODO: GetCharacterBags
                        // TODO: GetCharacterInventory
                        // TODO: GetCharacterSkills
                        // TODO: GetCahracterQuickbar

                        return character;
                    }
                }
            }
        }

        public List<Item> GetCharacterEquipment( string characterName )
        {
            List<Item> list = new List<Item>();
            using ( var conn = new MySqlConnection( mConnStr ) )
            {
                conn.Open();
                using ( var command = conn.CreateCommand() )
                {
                    command.CommandText = "SELECT * FROM `items` WHERE `Character` = ?Character AND `Bag` = 99";
                    command.Parameters.AddWithValue( "?Character", characterName );

                    using ( var reader = command.ExecuteReader() )
                    {
                        while ( reader.Read() )
                        {
                            Item item = new Item();
                            item.Hash     = reader.GetUInt32( 1 );
                            item.Quantity = reader.GetByte( 2 );
                            item.Bag      = reader.GetByte( 3 );
                            item.Position = reader.GetByte( 4 );

                            list.Add( item );
                        }
                    }
                }
            }
            return list;
        }

        #endregion

        #region Methods: Sessions

        public Account GetSessionAccount( string sessionKey )
        {
            using (var conn = new MySqlConnection(mConnStr))
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT `Account` FROM `sessions` WHERE `Key` = ?Key";
                    command.Parameters.AddWithValue("?Key", sessionKey);

                    string accountName = (string)command.ExecuteScalar();
                    if (accountName == null)
                        return null;

                    return GetAccount(accountName);
                }
            }
        }

        public Character GetSessionCharacter(string sessionKey)
        {
            using (var conn = new MySqlConnection(mConnStr))
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT `Character` FROM `sessions` WHERE `Key` = ?Key";
                    command.Parameters.AddWithValue("?Key", sessionKey);

                    string characterName = (string)command.ExecuteScalar();
                    if (characterName == null)
                        return null;

                    return GetCharacter(characterName);
                }
            }
        }

        public bool IsAccountOnline( string accountName )
        {
            using ( var conn = new MySqlConnection( mConnStr ) )
            {
                conn.Open();
                using ( var command = conn.CreateCommand() )
                {
                    command.CommandText = "SELECT COUNT(*) FROM `sessions` WHERE `Account` = ?Account";
                    command.Parameters.AddWithValue( "?Account", accountName );

                    return (Int64)command.ExecuteScalar() == 1;
                }
            }
        }

        public bool IsCharacterOnline( string characterName )
        {
            using ( var conn = new MySqlConnection( mConnStr ) )
            {
                conn.Open();
                using ( var command = conn.CreateCommand() )
                {
                    command.CommandText = "SELECT COUNT(*) FROM `sessions` WHERE `Character` = ?Character";
                    command.Parameters.AddWithValue( "?Character", characterName );

                    return (Int64)command.ExecuteScalar() == 1;
                }
            }
        }

        #endregion
    }
}
