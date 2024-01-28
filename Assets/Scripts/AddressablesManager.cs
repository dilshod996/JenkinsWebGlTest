
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UI;

public class AddressablesManager : MonoBehaviour
{
    // Singleton instance
    private static AddressablesManager instance;
    public static AddressablesManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("AddressablesManager").AddComponent<AddressablesManager>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }



    public bool addressablesdownloadfinish = false;

    float DownLoadSize = 0f;
    string AddressablesMessageText = string.Empty;
    private AsyncOperationHandle AssetDownCheckHandler;
    public Dictionary<string, List<object>> DictRunMemoryObject = new Dictionary<string, List<object>>();
    private AsyncOperationHandle<IList<IResourceLocation>> LocationHandlerCheck;
    private AsyncOperationHandle RunMemoryHandler;
    public Dictionary<string, List<AssetReference>> DictRunMemoryAssetReference = new Dictionary<string, List<AssetReference>>();
    public Dictionary<string, List<string>> DictLocation = new Dictionary<string, List<string>>();
    private List<IResourceLocation> Locations;

    private Dictionary<string, List<IResourceLocation>> DictIResourceLocation = new Dictionary<string, List<IResourceLocation>>();
    private int AllAddressableAssetDownLoadCount = 0;

    private void Start()
    {
        //CheckAndUpdateAddressables();
    }



    void DownloadGroup(string groupName)
    {
        // Perform the download operation for the specified group
        AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(groupName);

        // Set up a callback for when the download completes
        downloadHandle.Completed += handle =>
        {
            // Check if download was successful
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"Addressable group '{groupName}' download completed successfully.");
            }
            else
            {
                Debug.LogError($"Addressable group '{groupName}' download failed: " + handle.OperationException);
            }

            // Release the download handle
            Addressables.Release(handle);
        };
    }
    public IEnumerator AddressableDownLoadSizeCheck(List<string> label, Action<float, List<string>> success = null, Action<string> fail = null)
    {
        AllAddressableAssetDownLoadCount = label.Count;
        long bundleByteSize = 0;// = (long)downSizeHandle.Result;
        List<string> needDownloadLabelList = new List<string>();
        for (int i = 0; i < label.Count; i++)
        {
            int tmpInt = i;
            AsyncOperationHandle<long> getDownloadSize = Addressables.GetDownloadSizeAsync(label[tmpInt]);
            yield return getDownloadSize;

            if (getDownloadSize.Status.Equals(AsyncOperationStatus.Failed))
            {
                AddressablesMessageText = string.Format("[AddressableManager] AddressableDownLoadSizeCheck {0} is Failed", label[tmpInt]);
                fail?.Invoke(AddressablesMessageText);
                yield break;
            }
            else if (getDownloadSize.Status.Equals(AsyncOperationStatus.Succeeded))
            {
                if (getDownloadSize.Result > 0)
                {
                    needDownloadLabelList.Add(label[tmpInt]);
                    bundleByteSize += getDownloadSize.Result;
                }
            }
            //Addressables.Release(getDownloadSize);
        }
        DownLoadSize = (bundleByteSize / 1024f) / 1024f;

        success?.Invoke(DownLoadSize, needDownloadLabelList);
    }
    public IEnumerator AddressableDownLoad(List<string> label, Action success = null, Action<string> fail = null)
    {
        Debug.Log(label.Count);
        for (int i = 0; i < label.Count; i++)
        {
            Debug.Log("###" + label[i]);
            //AssetDownCheckHandler = Addressables.DownloadDependenciesAsync(label[i]);
            AsyncOperationHandle downHandle = Addressables.DownloadDependenciesAsync(label[i]);
            AssetDownCheckHandler = downHandle;
            //AssetDownHandler = Addressables.DownloadDependenciesAsync(label[i]);
            //DownLoadPercent = downHandle.PercentComplete * 100;
            yield return downHandle;

            if (downHandle.Status.Equals(AsyncOperationStatus.Failed))
            {
                AddressablesMessageText = string.Format("[AddressableManager] AddressableDownLoad {0} is Failed", label[i]);
                Addressables.Release(downHandle);
                fail?.Invoke(AddressablesMessageText);
                yield break;
            }
        }

        success?.Invoke();
    }
    public IEnumerator LabelListLocationsAsync(List<string> labels, List<bool> runMemeory, Action success = null, Action<string> fail = null)
    {
        AsyncOperationHandle<IList<IResourceLocation>> LocationHandler;
        if (labels.Count.Equals(runMemeory.Count))
        {

            for (int i = 0; i < labels.Count; i++)
            {
                if (!DictLocation.ContainsKey(labels[i]))
                {
                    LocationHandler = Addressables.LoadResourceLocationsAsync(labels[i]);
                    LocationHandlerCheck = LocationHandler;
                    yield return LocationHandler;

                    if (LocationHandler.Status.Equals(AsyncOperationStatus.Failed))
                    {
                        AddressablesMessageText = string.Format("[AddressableManager] LabelLocationsAsync {0} is Failed", labels[i]);
                        Addressables.Release(LocationHandler);
                        fail?.Invoke(AddressablesMessageText);
                        yield break;
                    }
                    else if (LocationHandler.Status.Equals(AsyncOperationStatus.Succeeded))
                    {
                        Locations = new List<IResourceLocation>(LocationHandler.Result);
                        Addressables.Release(LocationHandler);

                        List<string> locationStringList = new List<string>();
                        for (int j = 0; j < Locations.Count; j++)
                        {
                            locationStringList.Add(Locations[j].PrimaryKey.ToString());
                        }
                        DictLocation.Add(labels[i], locationStringList);
                        DictIResourceLocation.Add(labels[i], Locations);
                    }
                    //Addressables.Release(LocationHandler);
                }
                else
                {
                    LocationHandler = Addressables.LoadResourceLocationsAsync(labels[i]);
                    LocationHandlerCheck = LocationHandler;
                    //CheckPercent = LocationHandler.PercentComplete * 100;
                    yield return LocationHandler;

                    if (LocationHandler.Status.Equals(AsyncOperationStatus.Failed))
                    {
                        AddressablesMessageText = string.Format("[AddressableManager] LabelLocationsAsync {0} is Failed", labels[i]);
                        Addressables.Release(LocationHandler);
                        fail?.Invoke(AddressablesMessageText);
              
                        yield break;
                    }
                    else if (LocationHandler.Status.Equals(AsyncOperationStatus.Succeeded))
                    {
                        Locations = new List<IResourceLocation>(LocationHandler.Result);
                        Addressables.Release(LocationHandler);

                        for (int j = 0; j < Locations.Count; j++)
                        {
                            if (!DictLocation[labels[i]].Exists(str => str.Equals(Locations[j].PrimaryKey.ToString())))
                            {
                                DictLocation[labels[i]].Add(Locations[j].PrimaryKey.ToString());
                                DictIResourceLocation[labels[i]].Add(Locations[j]);
                            }
                        }
                    }
                    //Addressables.Release(LocationHandler);
                }
            }

        }
        else
        {
            AddressablesMessageText = string.Format(
                "[AddressableManager] Get LabelListLocations Failed, LabelCount is not correct runMemoryCount. " +
                "labelCount : {0}, runMemoryColunt : {1}", labels.Count, runMemeory.Count);
            fail?.Invoke(AddressablesMessageText);
            yield break;
        }

        yield return null;

        success?.Invoke();
    }

}