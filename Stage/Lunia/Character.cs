#region Using Statements
using System;
using System.Collections.Generic;
#endregion

namespace Soldin
{
    class Character
    {
        #region Properties

        /// <summary>
        /// Gets or sets the virtual ID of the character.
        /// </summary>
        public int VirtualID { get; set; }

        /// <summary>
        /// Gets or sets the name of the account the character belongs to.
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// Gets or sets the name of the character.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the class of the character.
        /// </summary>
        public short Class { get; set; }

        /// <summary>
        /// Gets or sets the stage level of the character.
        /// </summary>
        public short StageLevel { get; set; }

        /// <summary>
        /// Gets or sets the stage experience of the character.
        /// </summary>
        public int StageExperience { get; set; }

        /// <summary>
        /// Gets or sets the PVP level of the character.
        /// </summary>
        public short PvpLevel { get; set; }

        /// <summary>
        /// Gets or sets the pvp experience of the character.
        /// </summary>
        public int PvpExperience { get; set; }

        /// <summary>
        /// Gets or sets the war level of the character.
        /// </summary>
        public short WarLevel { get; set; }

        /// <summary>
        /// Gets or sets the war experience of the character.
        /// </summary>
        public int WarExperience { get; set; }

        /// <summary>
        /// Gets or sets the date on which the character was created.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the date on which the character last logged in.
        /// </summary>
        public DateTime LastLoggedOn { get; set; }

        /// <summary>
        /// Gets or sets the number of unspend skillpoints the character has left.
        /// </summary>
        public short SkillPoints { get; set; }

        /// <summary>
        /// Gets or sets the amount of bags the character has.
        /// </summary>
        public short BagCount { get; set; }

        /// <summary>
        /// Gets or sets the amount of money the character has in its bag.
        /// </summary>
        public int BagMoney { get; set; }

        /// <summary>
        /// Gets or sets the amount of bag slots the character has in the bank.
        /// </summary>
        public short BankBagCount { get; set; }

        /// <summary>
        /// Gets or sets the amount of money the character has stored in the bank.
        /// </summary>
        public int BankMoney { get; set; }

        /// <summary>
        /// Gets or sets the rebirth level of the character.
        /// </summary>
        public short RebirthLevel { get; set; }

        /// <summary>
        /// Gets or sets the number of times the character has been reborn.
        /// </summary>
        public short RebirthCount { get; set; }

        /// <summary>
        /// Gets or sets the list of equipment the character is wearing.
        /// </summary>
        public List<Item> Equipment { get; set; }

        #endregion

        public int PvpRating = 1500;
        public Vector3D Position;
        public Vector3D Rotation;
        public DateTime LastAction;
        public uint Direction = 0;
        public float Health = 500;
        public float Mana = 500;
        public short Team = 1;
        public int AchievementPoints = 0;

        public Character()
        {
            Position = new Vector3D { X = 1200, Y = 0, Z = 610 };
            Rotation = new Vector3D { X = 0, Y = 0, Z = -1 };
        }
    }


    class Vector3D
    {
        public float X, Y, Z;
    }
}
