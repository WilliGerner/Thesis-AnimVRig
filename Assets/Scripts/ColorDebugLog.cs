using UnityEngine;

public class ColorDebugLog : MonoBehaviour
{
    private static ColorDebugLog _instance;

    public static ColorDebugLog Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject debugLogObject = new GameObject("ColorDebugLog");
                _instance = debugLogObject.AddComponent<ColorDebugLog>();
                DontDestroyOnLoad(debugLogObject);
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Log(string message, Color color)
    {
        string hexColor = ColorUtility.ToHtmlStringRGB(color);
        Debug.Log($"<color=#{hexColor}>{message}</color>");
    }
}
