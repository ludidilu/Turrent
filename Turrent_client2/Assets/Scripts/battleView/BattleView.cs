using UnityEngine;
using gameObjectFactory;
using superFunction;

public class BattleView : UIPanel
{
    private static GameObject m_battleManagerEventGo;

    public static GameObject battleManagerEventGo
    {
        get
        {
            if (m_battleManagerEventGo == null)
            {
                m_battleManagerEventGo = new GameObject("BattleManagerEventGo");
            }

            return m_battleManagerEventGo;
        }
    }

    private BattleManager battleManager;

    public override void OnEnter()
    {
        if (battleManager == null)
        {
            GameObject go = GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/battle/BattleManager.prefab", null);

            battleManager = go.GetComponent<BattleManager>();

            battleManager.Init(battleManagerEventGo);

            SuperFunction.Instance.AddEventListener(battleManagerEventGo, BattleManager.BATTLE_QUIT, BattleQuit);
        }

        battleManager.RequestRefreshData();
    }

    private void BattleQuit(int _index)
    {
        UIManager.Instance.Hide(uid);
    }
}