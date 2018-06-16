namespace Turrent_lib
{
    public interface IUnitSDS
    {
        int[] GetRow();
        int[] GetPos();
        ITurrentSDS[] GetTurrent();
        int GetHp();
        int GetCost();
        int GetCd();
        int GetLiveTime();
        int[] GetAuras();
    }
}
