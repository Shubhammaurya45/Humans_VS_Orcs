using UnityEngine;

public class SingletonManager<T> : MonoBehaviour where T: MonoBehaviour
{
    private static T instance;
    public static T Instance{get { return instance;}}

    protected virtual void Awake()
    {
        if (instance !=null && instance==this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance= this as T;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
            instance = null;
    }
}
