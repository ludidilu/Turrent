using Turrent_lib;

public partial class UnitSDS : CsvBase, IUnitSDS
{
    public int[] row;
    public int[] pos;
    public int[] turrent;
    public int hp;
    public int cost;
    public bool isSkill;

    private ITurrentSDS[] turrentFix;

    public int[] GetRow()
    {
        return row;
    }

    public int[] GetPos()
    {
        return pos;
    }

    public ITurrentSDS[] GetTurrent()
    {
        if (turrentFix == null)
        {
            turrentFix = new ITurrentSDS[turrent.Length];

            for (int i = 0; i < turrent.Length; i++)
            {
                turrentFix[i] = StaticData.GetData<TurrentSDS>(turrent[i]);
            }
        }

        return turrentFix;
    }

    public int GetHp()
    {
        return hp;
    }

    public int GetCost()
    {
        return cost;
    }

    public bool GetIsSkill()
    {
        return isSkill;
    }
}
