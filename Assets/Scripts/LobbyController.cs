using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    public Image AddressableImage;
    public Text UserName;
    private string ImageLabel = "Dady";
    // Start is called before the first frame update
    void Start()
    {
        LoadAssetAsync();
    }
    private void LoadAssetAsync()
    {
        Addressables.LoadAssetAsync<Sprite>(ImageLabel).Completed += OnAssetLoaded;
    }

    private void OnAssetLoaded(AsyncOperationHandle<Sprite> op)
    {
        if (op.Status == AsyncOperationStatus.Succeeded)
        {
            AddressableImage.sprite = op.Result;
            UserName.text =string.Format("Hello {0} Nice you have",  UserInfo.Instance.UserName);

        }
        else
        {
            Debug.LogError($"error to load spr. Error: {op.OperationException}");
        }
    }

}
