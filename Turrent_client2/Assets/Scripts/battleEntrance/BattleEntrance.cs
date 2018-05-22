public class BattleEntrance : UIPanel
{



    public void Online()
    {
        UIManager.Instance.ShowInParent<BattleOnline>(1, uid);
    }

    public void Local()
    {
    }

    public void PlayRecord()
    {
    }
}
