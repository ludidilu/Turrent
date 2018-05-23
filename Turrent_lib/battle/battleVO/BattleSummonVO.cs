namespace Turrent_lib
{
    public struct BattleSummonVO
    {
        public bool isMine;

        public int uid;

        public int pos;

        public BattleSummonVO(bool _isMine, int _uid, int _pos)
        {
            isMine = _isMine;

            uid = _uid;

            pos = _pos;
        }
    }
}
