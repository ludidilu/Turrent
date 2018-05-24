using UnityEngine;
using UnityEngine.UI;

public class BattleCell : BattleClickGo
{
    [SerializeField]
    private Image hintImg;

    public void SetHintVisible(bool _visible)
    {
        hintImg.gameObject.SetActive(_visible);
    }
}
