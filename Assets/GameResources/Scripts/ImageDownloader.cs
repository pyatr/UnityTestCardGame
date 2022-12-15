using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ImageDownloader : MonoBehaviour
{
    private const string RANDOM_IMAGE_URL = "https://picsum.photos";

    public struct ImageDownloadContainer
    {
        public string MediaUrl;
        public Action<Texture2D> OnDownloadFinished;
    }

    [SerializeField]
    private Texture2D dummyImage;

    public void DownloadImage(ImageDownloadContainer container)
    {
        StartCoroutine(DownloadImageCoroutine(container));
    }

    public void GetRandomImage(Vector2Int dimensions, Action<Texture2D> OnDownloadFinished)
    {
        ImageDownloadContainer container;
        container.MediaUrl = $"{RANDOM_IMAGE_URL}/{dimensions.x}/{dimensions.y}";
        container.OnDownloadFinished = OnDownloadFinished;
        DownloadImage(container);
    }

    private IEnumerator DownloadImageCoroutine(ImageDownloadContainer container)
    {
        Texture2D texture = dummyImage;
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(container.MediaUrl);
        Debug.Log($"Requesting image from {container.MediaUrl}");
        yield return request.SendWebRequest();
        Debug.Log($"Finished downloading from {container.MediaUrl}");
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            Debug.LogError(request.error);
        else
            texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        container.OnDownloadFinished.Invoke(texture);
    }
}
