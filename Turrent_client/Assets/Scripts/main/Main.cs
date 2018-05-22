using UnityEngine;
using System;
using gameObjectFactory;

public class Main : MonoBehaviour
{
    [SerializeField]
    private Transform root;

    [SerializeField]
    private Transform mask;

    private const string PATH_FIX = "Assets/Resource/prefab/game/{0}.prefab";

    public const int layerIndex = 100;

    void Awake()
    {
        Application.targetFrameRate = 60;

        //Turrent_lib.Log.Init(Debug.Log);

        Connection.Log.Init(Debug.Log);

        UIManager.Instance.Init(root, mask, LoadUi);

        ResourceLoader.Load(LoadOver);
    }

    private void LoadOver()
    {
        UIManager.Instance.ShowInRoot<BattleEntrance>(0, layerIndex);
    }

    private void LoadUi(Type _type, Action<GameObject> _callBack)
    {
        GameObjectFactory.Instance.GetGameObject(string.Format(PATH_FIX, _type.Name), _callBack);
    }
}
