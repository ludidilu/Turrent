namespace Turrent_lib
{
    public struct BattleDeadVO
    {
        public bool isMine;

        public int uid;

        public BattleDeadVO(bool _isMine, int _uid)
        {
            isMine = _isMine;

            uid = _uid;
        }
    }
}
