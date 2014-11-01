namespace Soldin.Net
{
    static class PacketID
    {
        #region Constants

        public const ushort MSG_HANDSHAKE          = 0xAA41;
        public const ushort MSG_LOGIN              = 0xBA09;
        public const ushort MSG_CHARACTERLICENSES  = 0x022C;
        public const ushort MSG_CHARACTERLIST      = 0xDC2C;
        public const ushort MSG_DISCONNECT         = 0xDD83;
        public const ushort MSG_CHARACTER_CREATE   = 0xC96E;
        public const ushort MSG_CHARACTER_DELETE   = 0x899F;
        public const ushort MSG_CHARACTER_SELECT   = 0x356E;
        public const ushort MSG_CHARACTER_DESELECT = 0xF2AD;
        public const ushort MSG_SQUARE_LIST        = 0x3A10;
        public const ushort MSG_SQUARE_SELECT      = 0x7AE9;
        public const ushort MSG_SQUARE_CONNECT     = 0xB98B;
        public const ushort MSG_ACHIEVEMENTSERVER  = 0x02A4;
        public const ushort MSG_PING               = 0x482F;
        public const ushort MSG_KEYBINDINGS        = 0xF518;
        public const ushort MSG_SETKEYBINDINGS     = 0xF0EF;
        public const ushort MSG_2NDPASSWD          = 0xAE98;
        public const ushort MSG_2NDPASSWD_CREATE   = 0xD9EE;
        public const ushort MSG_2NDPASSWD_CREATED  = 0x1DD9;
        public const ushort MSG_2NDPASSWD_LOGIN    = 0x0035;
        public const ushort MSG_2NDPASSWD_OK       = 0x33F4;
        public const ushort MSG_2NDPASSWD_REMOVE   = 0x305D;
        public const ushort MSG_2NDPASSWD_REMOVED  = 0x4F0A;
        public const ushort MSG_2NDPASSWD_CHANGE   = 0x0D2C;
        public const ushort MSG_2NDPASSWD_CHANGED  = 0xA19B;

        #endregion
    }
}
