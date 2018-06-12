namespace Turrent_lib
{
    public interface IUnitSDS
    {
        int[] GetRow();
        int[] GetPos();
        ITurrentSDS[] GetTurrent();
        int GetHp();
        int GetCost();
        int GetLiveTime();
        int[] GetAura();
    }
}
