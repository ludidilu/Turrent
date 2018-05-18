namespace Turrent_lib
{
    public interface ITurrentSDS
    {
        int GetCd();
        int[] GetAttackTargetPos();
        int[][] GetAttackDamagePos();
        int GetAttackGap();
        int GetAttackDamage();
    }
}
