using UnityEngine;

public class Singleton<sgt> : MonoBehaviour where sgt : Component
{
    private static sgt _instance;

    public static sgt Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = typeof(sgt).Name;

                //do not show object in the hierarchy
                //obj.hideFlags = HideFlags.HideAndDontSave;
                _instance = obj.AddComponent<sgt>();
            }
            return _instance;
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}

//singleton pattern that is persistent throught different scenes
public class SingletonPersistent<sgt> : MonoBehaviour where sgt : Component
{
    public static sgt Instance
    {
        get;
        private set;
    }

    public virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as sgt;
            //DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }
}
