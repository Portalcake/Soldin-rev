namespace Soldin
{
    class Square
    {
        #region Properties

        /// <summary>
        /// Gets or sets the ID of the square.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the square.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the status of the square.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets the type of the square.
        /// </summary>
        public SquareType Type { get; set; }

        /// <summary>
        /// Gets or sets the capacity of the square.
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// Gets or sets the IP address of the square.
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Gets or sets the port of the square.
        /// </summary>
        public short Port { get; set; }

        #endregion

        // TODO: Figure out what this value does.
        public uint Unknown { get; set; }
    }

    /// <summary>
    /// Identifies the type of a square.
    /// </summary>
    enum SquareType { Regular = 1, Myth = 11, PVP = 12 }
}
 