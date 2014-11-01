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
            connStrBuilder.Server          = Server.Settings.GetString(    "gateway.mysql.host",        "localhost" );
            connStrBuilder.Port            = (uint)Server.Settings.GetInt( "gateway.mysql.port",        3306 ); ;
            connStrBuilder.UserID          = Server.Settings.GetString(    "gateway.mysql.username",    "root" );
            connStrBuilder.Password        = Server.Settings.GetString(    "gateway.mysql.password",    "" );
            connStrBuilder.Database        = Server.Settings.GetString(    "gateway.mysql.database",    "soldinrev" );
            connStrBuilder.MinimumPoolSize = (uint)Server.Settings.GetInt( "gateway.mysql.minpoolsize", 5 );
            connStrBuilder.MaximumPoolSize = (uint)Server.Settings.GetInt( "gateway.mysql.maxpoolsize", 50 );
            connStrBuilder.Pooling         = true;

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

                    command.CommandText = "SELECT `Character` FROM `character_licenses` WHERE `Account` = ?Name";
                    using ( var reader = command.ExecuteReader() )
                    {
                        account.CharacterLicenses = new List<int>();
                        while ( reader.Read() )
                        {
                            account.CharacterLicenses.Add( reader.GetInt32( 0 ) );
                        }
                    }

                    account.Characters = GetCharacters( account.Name );
                    return account;
                }
            }
        }

        public void SetSecondaryPassword( string accountName, string secondaryPassword )
        {
            using ( var conn = new MySqlConnection( mConnStr ) )
            {
                conn.Open();
                using ( var command = conn.CreateCommand() )
                {
                    command.CommandText = "UPDATE `accounts` SET `SecondaryPassword` = ?SecondaryPassword WHERE `Name` = ?Name";
                    command.Parameters.AddWithValue( "?SecondaryPassword", secondaryPassword );
                    command.Parameters.AddWithValue( "?Name", accountName );

                    command.ExecuteNonQuery();
                }
            }
        }

        public void SetKeybindings( string accountName, string keybindings )
        {
            using ( var conn = new MySqlConnection( mConnStr ) )
            {
                conn.Open();
                using ( var command = conn.CreateCommand() )
                {
                    command.CommandText = "INSERT INTO `keybindings` (`Account`, `Data`) VALUES (?Account, ?Data) ON DUPLICATE KEY UPDATE `Data` = ?Data;";
                    command.Parameters.AddWithValue( "?Account", accountName );
                    command.Parameters.AddWithValue( "?Data", keybindings );

                    command.ExecuteNonQuery();
                }
            }
        }

        public string GetKeybindings( string accountName )
        {
            using ( var conn = new MySqlConnection( mConnStr ) )
            {
                conn.Open();
                using ( var command = conn.CreateCommand() )
                {
                    command.CommandText = "SELECT `Data` FROM `keybindings` WHERE `Account` = ?Account";
                    command.Parameters.AddWithValue( "?Account", accountName );

                    return (string)command.ExecuteScalar();
                }
            }
        }

        #endregion

        #region Methods: Characters

        public List<Character> GetCharacters( string accountName )
        {
            List<Character> list = new List<Character>();
            using ( var conn = new MySqlConnection( mConnStr ) )
            {
                conn.Open();
                using ( var command = conn.CreateCommand() )
                {
                    command.CommandText = "SELECT * FROM `characters` WHERE `Account` = ?Account";
                    command.Parameters.AddWithValue( "?Account", accountName );

                    using ( var reader = command.ExecuteReader() )
                    {
                        while ( reader.Read() )
                        {
                            Character character = new Character();
                            character.Account     = accountName;
                            character.Name   = reader.GetString(1);
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

                            list.Add( character );
                        }
                    }
                }
            }
            return list;
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

        public bool IsCharacterNameAvailable( string characterName )
        {
            using ( var conn = new MySqlConnection( mConnStr ) )
            {
                conn.Open();
                using ( var command = conn.CreateCommand() )
                {
                    command.CommandText = "SELECT COUNT(*) FROM `characters` WHERE `Name` = ?Name";
                    command.Parameters.AddWithValue( "?Name", characterName );

                    return (Int64)command.ExecuteScalar() == 0;
                }
            }
        }

        public bool CreateCharacter( string accountName, string characterName, short characterClass, out Character character )
        {
            character = new Character
            {
                Account         = accountName,
                Name            = characterName,
                Class           = characterClass,
                StageLevel      = 1,
                StageExperience = 0,
                PvpLevel        = 1,
                PvpExperience   = 0,
                WarLevel        = 1,
                WarExperience   = 0,
                CreatedOn       = DateTime.Now,
                LastLoggedOn    = DateTime.MinValue,
                SkillPoints     = 0,
                BagCount        = 0,
                BagMoney        = 0,
                BankBagCount    = 0,
                BankMoney       = 0,
                RebirthLevel    = 1,
                RebirthCount    = 0
            };

            using ( var conn = new MySqlConnection( mConnStr ) )
            {
                conn.Open();
                using ( var command = conn.CreateCommand() )
                {
                    command.CommandText = "INSERT INTO `characters` (`Account`, `Name`, `Class`, `CreatedOn`, `LastLoggedOn`) VALUES (?Account, ?Name, ?Class, ?CreatedOn, ?LastLoggedOn)";
                    command.Parameters.AddWithValue( "?Account", accountName );
                    command.Parameters.AddWithValue( "?Name", characterName );
                    command.Parameters.AddWithValue( "?Class", characterClass );
                    command.Parameters.AddWithValue( "?CreatedOn", character.CreatedOn );
                    command.Parameters.AddWithValue( "?LastLoggedOn", character.LastLoggedOn );

                    return ( command.ExecuteNonQuery() == 1 );
                }
            }
        }

        public void DeleteCharacter( string characterName )
        {
            using ( var conn = new MySqlConnection( mConnStr ) )
            {
                conn.Open();
                using ( var command = conn.CreateCommand() )
                {
                    command.CommandText = "DELETE FROM `characters`  WHERE `Name` = ?Name";
                    command.Parameters.AddWithValue( "?Name", characterName );

                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region Methods: Squares

        public List<Square> GetSquareList()
        {
            List<Square> list = new List<Square>();
            using ( var conn = new MySqlConnection( mConnStr ) )
            {
                conn.Open();
                using ( var command = conn.CreateCommand() )
                {
                    command.CommandText = "SELECT * FROM `squares`";
                    using ( var reader = command.ExecuteReader() )
                    {
                        while ( reader.Read() )
                        {
                            list.Add( new Square
                            {
                                Id       = reader.GetInt32( 0 ),
                                Name     = reader.GetString( 1 ),
                                Status   = reader.GetInt32( 2 ),
                                Type     = (SquareType)reader.GetInt32( 3 ),
                                Capacity = reader.GetInt32( 4 ),
                                Unknown  = 0x0330A106
                            } );
                        }
                    }
                }
            }
            return list;
        }

        public Square GetSquare( string squareName )
        {
            using ( var conn = new MySqlConnection( mConnStr ) )
            {
                conn.Open();
                using ( var command = conn.CreateCommand() )
                {
                    command.CommandText = "SELECT * FROM `squares` WHERE `Name` = ?Name";
                    command.Parameters.AddWithValue( "?Name", squareName );

                    using ( var reader = command.ExecuteReader() )
                    {
                        if ( !reader.HasRows || !reader.Read() )
                            return null;

                        return new Square
                        {
                            Id       = reader.GetInt32( 0 ),
                            Name     = reader.GetString( 1 ),
                            Status   = reader.GetInt32( 2 ),
                            Type     = (SquareType)reader.GetInt32( 3 ),
                            Capacity = reader.GetInt32( 4 ),
                            IP       = reader.GetString( 5 ),
                            Port     = reader.GetInt16( 6 ),
                            Unknown  = 0x0330A106
                        };
                    }
                }
            }
        }

        #endregion

        #region Methods: Sessions

        public void CreateSession( string sessionKey )
        {
            using ( var conn = new MySqlConnection( mConnStr ) )
            {
                conn.Open();
                using ( var command = conn.CreateCommand() )
                {
                    command.CommandText = "INSERT INTO `sessions` (`Key`) VALUES (?Key)";
                    command.Parameters.AddWithValue( "?Key", sessionKey );

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteSession( string sessionKey )
        {
            using ( var conn = new MySqlConnection( mConnStr ) )
            {
                conn.Open();
                using ( var command = conn.CreateCommand() )
                {
                    command.CommandText = "DELETE FROM `sessions`  WHERE `Key` = ?Key";
                    command.Parameters.AddWithValue( "?Key", sessionKey );

                    command.ExecuteNonQuery();
                }
            }
        }

        public void AssignAccountToSession( string sessionKey, string accountName )
        {
            using ( var conn = new MySqlConnection( mConnStr ) )
            {
                conn.Open();
                using ( var command = conn.CreateCommand() )
                {
                    command.CommandText = "UPDATE `sessions` SET `Account` = ?Account WHERE `Key` = ?Key";
                    command.Parameters.AddWithValue( "?Account", accountName );
                    command.Parameters.AddWithValue( "?Key", sessionKey );

                    command.ExecuteNonQuery();
                }
            }
        }

        public void AssignCharacterToSession( string sessionKey, string characterName )
        {
            using ( var conn = new MySqlConnection( mConnStr ) )
            {
                conn.Open();
                using ( var command = conn.CreateCommand() )
                {
                    command.CommandText = "UPDATE `sessions` SET `Character` = ?Character WHERE `Key` = ?Key";
                    command.Parameters.AddWithValue( "?Character", characterName );
                    command.Parameters.AddWithValue( "?Key", sessionKey );

                    command.ExecuteNonQuery();
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

        public void ClearSessions()
        {
            using ( var conn = new MySqlConnection( mConnStr ) )
            {
                conn.Open();
                using ( var command = conn.CreateCommand() )
                {
                    command.CommandText = "TRUNCATE TABLE `sessions`";
                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion
    }
}
