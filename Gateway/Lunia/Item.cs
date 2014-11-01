namespace Soldin
{
    class Item
    {
        #region Properties

        /// <summary>
        /// Gets or sets the hash of the item.
        /// </summary>
        public uint Hash { get; set; }

        /// <summary>
        /// Gets or sets the item quantity.
        /// </summary>
        public byte Quantity { get; set; }

        /// <summary>
        /// Gets or sets the index of the bag the item is stored in.
        /// </summary>
        public byte Bag { get; set; }

        /// <summary>
        /// Gets or sets the position of the item within the bag its stored in.
        /// </summary>
        public byte Position { get; set; }

        #endregion
    }
}
