using UnityEngine;

public class UserInfo : MonoBehaviour
{
    private static UserInfo instance;

    public static UserInfo Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("UserInfo").AddComponent<UserInfo>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }


    public string UserName { get; private set; }


    public void SetUserName(string name)
    {
        UserName = name;
    }
}
