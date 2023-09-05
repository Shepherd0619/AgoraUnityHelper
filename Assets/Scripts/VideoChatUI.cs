using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class VideoChatUI : MonoBehaviour
{
    public Transform Content;
    public Dictionary<uint, RawImage> keyValuePairs = new Dictionary<uint, RawImage>();
    public TMP_InputField AppID;
    public TMP_InputField Token;
    public TMP_InputField ChannelName;

    // Start is called before the first frame update
    void Start()
    {
        VideoChatHelper.Instance.OnVideoTextureCreated = (id) =>
        {
            GameObject go = new GameObject("RawImage_uid_" + id);
            go.transform.parent = Content;
            RawImage img = go.AddComponent<RawImage>();
            img.uvRect = new Rect(img.uvRect.x, img.uvRect.y, -1f, -1f);
            keyValuePairs.Add(id, img);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
        };

        VideoChatHelper.Instance.OnVideoTextureDestroyed = (id) =>
        {
            Destroy(keyValuePairs[id].gameObject);
            keyValuePairs.Remove(id);
        };
    }

    // Update is called once per frame
    void Update()
    {
        foreach(KeyValuePair<uint,RawImage> pair in keyValuePairs)
        {
            pair.Value.texture = VideoChatHelper.Instance.RealtimeVideos[pair.Key].Texture;
            pair.Value.SetNativeSize();
            //VideoChatHelper.Instance.RealtimeVideos[pair.Key].Attach();
        }
    }

    public void UI_Initialize()
    {
        VideoChatHelper.Instance.appID = AppID.text;
        VideoChatHelper.Instance.token = Token.text;
        VideoChatHelper.Instance.channelName = ChannelName.text;
        VideoChatHelper.Instance.RTC_Initialize();
    }
}
