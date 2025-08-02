using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component 
{
    private static T instance;

    public static bool HasInstance => instance != null;
    public static T Instance 
    {
        get {
            return HasInstance ? instance : null;
        }
    }

    public static T ForceInstance
    {
        get {
            if (instance == null) {
                instance = FindAnyObjectByType<T>();
                if (instance == null) {
                    var go = new GameObject(typeof(T).Name + " Auto-Generated");
                    instance = go.AddComponent<T>();
                }
            }

            return instance;
        }
    }

    /// <summary>
    /// Make sure to call base.Awake() in override if you need awake.
    /// </summary>
    protected virtual void Awake() {
        InitializeSingleton();
    }

    protected virtual void InitializeSingleton() {
        if (!Application.isPlaying) return;

        instance = this as T;
    }
}
