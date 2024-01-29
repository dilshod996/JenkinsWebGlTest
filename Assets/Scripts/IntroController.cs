using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.Collections;
using System.Collections.Generic;

public class IntroController : MonoBehaviour
{
    public InputField userNameInputField;
    public Button JoinButton;
    public GameObject PopUp;
    public Text addressableText;
    public Button DownloadAddressables;

    // Addressable groups

    private const string SceneGroup = "Scenes";
    private const string ImagesGroup = "ProfileImage";
    List<string> addressableGroups = new List<string>
        {
            SceneGroup,
            ImagesGroup
        };

    List<string> allGetResourceLabelLocation = new List<string>();
    List<bool> allGetResourceRunMemoryList = new List<bool>();
    List<string> downBundleListall = new List<string>();



    private void Start()
    {
        addressableText.text = "First download data";
        DownloadAddressables.onClick.AddListener(DownloadAdd);
        UserInfo.Instance.SetUserName("");
        JoinButton.onClick.AddListener(OnJoinButtonClick);
        AddressableDownload();
    }

    public void OnJoinButtonClick()
    {
        string userName = userNameInputField.text;


        if (!string.IsNullOrEmpty(userName))
        {
            UserInfo.Instance.SetUserName(userName);
            LoadSceneAsync("Lobby");
        }
        else
        {
            userNameInputField.image.color = Color.yellow;
        }


    }

    private void LoadSceneAsync(string sceneLabel)
    {
        Addressables.LoadSceneAsync(sceneLabel).Completed += OnSceneLoadComplete;
    }

    private void OnSceneLoadComplete(AsyncOperationHandle<SceneInstance> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Scene loaded successfully.");
        }
        else
        {
            Debug.LogError("Failed to load scene: " + obj.OperationException);
        }
    }
    public void DownloadAdd()
    {
        Debug.Log("started");
        addressableText.text = "Downloading...";
        DownloadAddressables.gameObject.SetActive(false);
        StartCoroutine(AddressablesManager.Instance.AddressableDownLoad(downBundleListall, () =>
        {

            StartCoroutine(AddressablesManager.Instance.LabelListLocationsAsync(allGetResourceLabelLocation, allGetResourceRunMemoryList, () =>
            {
                Debug.Log("Finish download");
                PopUp.SetActive(false);
            },
            (errorText) =>
            {
                Debug.Log("error download");
            }));
        }));

    }
    void AddressableDownload()
    {

        allGetResourceLabelLocation.Add(SceneGroup);
        allGetResourceRunMemoryList.Add(false);

        allGetResourceLabelLocation.Add(ImagesGroup);
        allGetResourceRunMemoryList.Add(false);

        StartCoroutine(AddressablesManager.Instance.AddressableDownLoadSizeCheck(addressableGroups, (bundleSize, downBundleList) =>
        {
            if (bundleSize > 0)
            {
                Debug.Log("Please Download addressables");
                downBundleListall = downBundleList;
                PopUp.SetActive(true);
                if (PopUp.activeSelf)
                {
                    addressableText.text = string.Format("Data\r\n<b>{0:0} mb</b> have to download", bundleSize);


                }
                
            }

        }));

    }

}
