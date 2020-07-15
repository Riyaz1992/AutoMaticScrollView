using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class Data : MonoBehaviour
{
    #region Member variables

    // Public Methods
    public GameObject countryPrefab;
    public GameObject parent;

    // Private Methods
    [SerializeField]
    private DataContainer dataContainer;
    
    // Events 
    public delegate void DataFetch();
    public static DataFetch OnDataFetchComplete;

    #endregion
   
    private void Start()
    {
        FetchData();
    }

    #region FetchData

    private void FetchData()
    {
        dataContainer = JsonUtility.FromJson<DataContainer>(LoadJson());
        for (int i = 0; i < dataContainer.Data.Count; i++)
        {
            int _i = i;
            GameObject newButton = Instantiate(countryPrefab) as GameObject;
            newButton.transform.SetParent(parent.transform, false);
            StartCoroutine(DownloadImage(dataContainer.Data[i].imageurl, newButton.GetComponent<RawImage>()));
            newButton.GetComponent<Button>().onClick.AddListener(delegate { StartCoroutine(GoToWeb(_i)); });
        }
        OnDataFetchComplete?.Invoke();
    }

    #endregion

    #region Helper

    private string LoadJson()
    {
        TextAsset file = Resources.Load("Data") as TextAsset;
        return file.text;
    }

    private IEnumerator DownloadImage(string MediaUrl, RawImage YourRawImage)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            YourRawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }

    private IEnumerator GoToWeb(int imageNo)
    {
        yield return new WaitForSeconds(1);
        string url = dataContainer.Data[imageNo].weburl;
        Application.OpenURL(url);
    }

    #endregion
}



[System.Serializable]
public class DataContainer
{
    public List<DataObjects> Data;
}


[System.Serializable]
public class DataObjects
{
    public string imageurl;
    public string weburl;
}
