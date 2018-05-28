namespace Turrent_lib
{
    public struct BattleDeadVO
    {
        public bool isMine;

        public int pos;

        public BattleDeadVO(bool _isMine, int _pos)
        {
            isMine = _isMine;

            pos = _pos;
        }
    }
}
