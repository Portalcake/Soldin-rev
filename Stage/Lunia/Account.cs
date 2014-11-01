#region Using Statements
using System;
using System.Collections.Generic;
#endregion

namespace Soldin
{
    class Account
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the account.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the password of the account.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the date on which the account was created.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the date on which the account last logged in.
        /// </summary>
        public DateTime LastLoginOn { get; set; }

        /// <summary>
        /// Gets or sets the character limit of this account.
        /// </summary>
        public int MaxCharacters { get; set; }

        /// <summary>
        /// Gets or sets the secondary password of the account.
        /// </summary>
        public string SecondaryPassword { get; set; }

        #endregion
    }
}
