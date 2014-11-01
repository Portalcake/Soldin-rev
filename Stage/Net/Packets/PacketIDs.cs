namespace Soldin.Net
{
    static class PacketID
    {
        #region Constants

        public const ushort MSG_ENCRYPTIONKEY     = 0x5E71;
        public const ushort MSG_AUTHENTICATE      = 0x7260;
        public const ushort MSG_SKILLS            = 0x7414;
        public const ushort MSG_QUICKSLOTS        = 0x0E0F;
        public const ushort MSG_LOADING           = 0x81B6;

        public const ushort MSG_CHARACTER_INFO    = 0x3DDA;
        public const ushort MSG_CHARACTER_MOVE    = 0x4228;
        public const ushort MSG_CHARACTER_ACTION  = 0xE28D;
        public const ushort MSG_CHARACTER_UPDATE  = 0x13E4;
        public const ushort MSG_CHARACTER_DETAILS = 0x831F;

        #endregion
    }
}
