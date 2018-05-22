using System.Collections.Generic;
using Turrent_lib;

public partial class TurrentSDS : CsvBase, ITurrentSDS
{
    public int cd;
    public string[] attackTargetPos;
    public string[] attackDamagePos;
    public int attackGap;
    public int attackDamage;

    private KeyValuePair<int, int>[] attackTargetPosFix;
    private KeyValuePair<int, int>[][] attackDamagePosFix;

    public int GetCd()
    {
        return cd;
    }

    public KeyValuePair<int, int>[] GetAttackTargetPos()
    {
        if (attackTargetPosFix == null)
        {
            attackTargetPosFix = new KeyValuePair<int, int>[attackTargetPos.Length];

            for (int i = 0; i < attackTargetPos.Length; i++)
            {
                string str = attackTargetPos[i];

                string[] strArr = str.Split(':');

                attackTargetPosFix[i] = new KeyValuePair<int, int>(int.Parse(strArr[0]), int.Parse(strArr[1]));
            }
        }

        return attackTargetPosFix;
    }

    public KeyValuePair<int, int>[][] GetAttackDamagePos()
    {
        if (attackDamagePosFix == null)
        {
            attackDamagePosFix = new KeyValuePair<int, int>[attackDamagePos.Length][];

            for (int i = 0; i < attackDamagePos.Length; i++)
            {
                string str = attackDamagePos[i];

                string[] strArr = str.Split('&');

                KeyValuePair<int, int>[] arr = new KeyValuePair<int, int>[strArr.Length];

                for (int m = 0; m < strArr.Length; m++)
                {
                    string str2 = strArr[m];

                    string[] strArr2 = str2.Split(':');

                    arr[m] = new KeyValuePair<int, int>(int.Parse(strArr2[0]), int.Parse(strArr2[1]));
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
