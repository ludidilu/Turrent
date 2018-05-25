using UnityEngine;
using UnityEngine.UI;

public class TurrentCreaterCell : BattleClickGo
{
    public enum State
    {
        NULL,
        TARGET,
        SPLASH
    }

    [SerializeField]
    private Image targetHint;

    [SerializeField]
    private Image splashHint;

    public int x;

    public int y;

    public State state;

    public void SetState(State _state)
    {
        state = _state;

        if (state == State.TARGET)
        {
            targetHint.gameObject.SetActive(true);

            splashHint.gameObject.SetActive(false);
        }
        else if (state == State.SPLASH)
        {
            targetHint.gameObject.SetActive(false);

            splashHint.gameObject.SetActive(true);
        }
        else
        {
            targetHint.gameObject.SetActive(false);

            splashHint.gameObject.SetActive(false);
        }
    }
}
