using Turrent_lib;

public partial class TurrentSDS : CsvBase, ITurrentSDS
{
    public int cd;
    public int[] attackTargetPos;
    public string[] attackDamagePos;
    public int attackGap;
    public int attackDamage;

    private int[][] attackDamagePosFix;

    public int GetCd()
    {
        return cd;
    }

    public int[] GetAttackTargetPos()
    {
        return attackTargetPos;
    }

    public int[][] GetAttackDamagePos()
    {
        if (attackDamagePosFix == null)
        {
            attackDamagePosFix = new int[attackDamagePos.Length][];

            for (int i = 0; i < attackDamagePos.Length; i++)
            {
                string str = attackDamagePos[i];

                string[] strArr = str.Split('&');

                int[] arr = new int[strArr.Length];

                for (int m = 0; m < strArr.Length; m++)
                {
                    arr[m] = int.Parse(strArr[m]);
                }

                attackDamagePosFix[i] = arr;
            }
        }

        return attackDamagePosFix;
    }

    public int GetAttackGap()
    {
        return attackGap;
    }

    public int GetAttackDamage()
    {
        return attackDamage;
    }
}
