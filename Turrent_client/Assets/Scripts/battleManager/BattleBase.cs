using UnityEngine;
using UnityEngine.UI;

public class BattleBase : MonoBehaviour
{
    [SerializeField]
    private Text hp;

    public void SetHp(int _hp)
    {
        hp.text = _hp.ToString();
    }
}
