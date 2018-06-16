using System.Collections.Generic;
using Turrent_lib;

public partial class TurrentSDS : CsvBase, ITurrentSDS
{
    public int cd;
    public string[] attackTargetPos;
    public string[] attackSplashPos;
    public int attackGap;
    public int attackDamage;
    public int attackDamageType;
    public bool canAttackBase;

    public int attackDamageAdd;
    public int attackDamageAddMax;
    public int attackDamageAddGap;

    private KeyValuePair<int, int>[][] attackTargetPosFix;
    private KeyValuePair<int, int>[][] attackSplashPosFix;

    public int GetCd()
    {
        return cd;
    }

    public KeyValuePair<int, int>[][] GetAttackTargetPos()
    {
        if (attackTargetPosFix == null)
        {
            attackTargetPosFix = new KeyValuePair<int, int>[attackTargetPos.Length][];

            for (int i = 0; i < attackTargetPos.Length; i++)
            {
                string str = attackTargetPos[i];

                string[] strArr;

                if (string.IsNullOrEmpty(str))
                {
                    strArr = new string[0];
                }
                else
                {
                    strArr = str.Split('&');
                }

                KeyValuePair<int, int>[] arr = new KeyValuePair<int, int>[strArr.Length];

                for (int m = 0; m < strArr.Length; m++)
                {
                    string str2 = strArr[m];

                    string[] strArr2 = str2.Split(':');

                    arr[m] = new KeyValuePair<int, int>(int.Parse(strArr2[0]), int.Parse(strArr2[1]));
                }

                attackTargetPosFix[i] = arr;
            }
        }

        return attackTargetPosFix;
    }

    public KeyValuePair<int, int>[][] GetAttackSplashPos()
    {
        if (attackSplashPosFix == null)
        {
            attackSplashPosFix = new KeyValuePair<int, int>[attackSplashPos.Length][];

            for (int i = 0; i < attackSplashPos.Length; i++)
            {
                string str = attackSplashPos[i];

                string[] strArr;

                if (string.IsNullOrEmpty(str))
                {
                    strArr = new string[0];
                }
                else
                {
                    strArr = str.Split('&');
                }

                KeyValuePair<int, int>[] arr = new KeyValuePair<int, int>[strArr.Length];

                for (int m = 0; m < strArr.Length; m++)
                {
                    string str2 = strArr[m];

                    string[] strArr2 = str2.Split(':');

                    arr[m] = new KeyValuePair<int, int>(int.Parse(strArr2[0]), int.Parse(strArr2[1]));
                }

                attackSplashPosFix[i] = arr;
            }
        }

        return attackSplashPosFix;
    }

    public int GetAttackGap()
    {
        return attackGap;
    }

    public int GetAttackDamage()
    {
        return attackDamage;
    }

    public DamageType GetAttackDamageType()
    {
        return (DamageType)attackDamageType;
    }

    public bool GetCanAttackBase()
    {
        return canAttackBase;
    }

    public int GetAttackDamageAdd()
    {
        return attackDamageAdd;
    }

    public int GetAttackDamageAddMax()
    {
        return attackDamageAddMax;
    }

    public int GetAttackDamageAddGap()
    {
        return attackDamageAddGap;
    }
}
