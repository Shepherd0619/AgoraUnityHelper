// Video Chat Helper
// ���ߣ������� (Shepherd0619)
// ��Ҫ���������ƵƵ��ͨ��
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Agora.Rtc;
using UnityEngine.Serialization;
using UnityEngine.Networking.Types;
using Agora_RTC_Plugin.API_Example.Examples.Basic.JoinChannelVideo;
using Agora_RTC_Plugin.API_Example;
using UnityEngine.UI;
using System;

public class VideoChatHelper : MonoBehaviour
{
    [Header("���ű���Ҫ���������ƵƵ��ͨ����ֻ���RawImage����������")]
    [Header("��������")]
    [FormerlySerializedAs("APP_ID")]
    [SerializeField]
    public string appID = "";

    [FormerlySerializedAs("TOKEN")]
    [SerializeField]
    public string token = "";

    [FormerlySerializedAs("Ƶ������")]
    [SerializeField]
    public string channelName = "";

    private IRtcEngine RtcEngine;

    private Dictionary<uint, VideoChatTexture> RealtimeVideos = new Dictionary<uint, VideoChatTexture>();

    public static VideoChatHelper Instance;

    #region Action�¼�
    //��Ƶ���洴�����ʱ�ص�
    public Action<uint> OnVideoTextureCreated;
    public Action<uint> OnVideoTextureDestroyed;
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
		if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
		{                 
			Permission.RequestUserPermission(Permission.Microphone);
		}
#endif
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
		if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
		{                 
			Permission.RequestUserPermission(Permission.Camera);
		}
#endif
    }

    /// <summary>
    /// ��ʼ������
    /// </summary>
    public void RTC_Initialize()
    {
        if (string.IsNullOrWhiteSpace(appID) || string.IsNullOrWhiteSpace(token))
        {
            Debug.LogError("[VideoChatHelper]AppID��Tokenδ���á�");
            return;
        }

        RtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
        UserEventHandler handler = new UserEventHandler(this);
        RtcEngineContext context = new RtcEngineContext(appID, 0,
                                    CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING,
                                    AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT, AREA_CODE.AREA_CODE_GLOB);
        int result = RtcEngine.Initialize(context);
        if (result < 0)
        {
            switch (result)
            {
                case -1:
                    {
                        Debug.LogError("[VideoChatHelper]��ʼ��ʧ�ܣ���ȷ�ϻ����Ƿ���ȷ����");
                        return;
                    }
                case -2:
                    {
                        Debug.LogError("[VideoChatHelper]��ʼ��ʧ�ܣ������Ƿ�");
                        return;
                    }
                case -7:
                    {
                        Debug.LogError("[VideoChatHelper]��ʼ��ʧ�ܣ�SDKδ������ʼ��");
                        return;
                    }
                case -22:
                    {
                        Debug.LogError("[VideoChatHelper]��ʼ��ʧ�ܣ�ϵͳ��Դ����");
                        return;
                    }
                case -101:
                    {
                        Debug.LogError("[VideoChatHelper]��ʼ��ʧ�ܣ�AppID��Ч");
                        return;
                    }
                default:
                    {
                        Debug.LogError("[VideoChatHelper]��ʼ��ʧ�ܣ�ԭ��δ֪");
                        return;
                    }
            }
        }

        RtcEngine.InitEventHandler(handler);

        Debug.Log("[VideoChatHelper]�����ʼ����ɣ�");
    }

    /// <summary>
    /// ����Ƶ��
    /// </summary>
    public void RTC_JoinChannel()
    {
        RtcEngine.JoinChannel(token, channelName);
    }

    /// <summary>
    /// ����Ƶ��
    /// </summary>
    /// <param name="name">Ƶ������</param>
    public void RTC_JoinChannel(string name)
    {
        RtcEngine.JoinChannel(token, name);
    }

    /// <summary>
    /// ��������
    /// </summary>
    public void RTC_Destroy()
    {
        if (RtcEngine == null) return;
        RtcEngine.InitEventHandler(null);
        RtcEngine.LeaveChannel();
        RtcEngine.Dispose();
        Debug.Log("[VideoChatHelper]������RTC����Ƶͨ�����棡");
    }

    private void OnDestroy()
    {
        RTC_Destroy();
    }

    #region -- Video Render UI Logic ---
    /*
    internal static void MakeVideoView(uint uid, string channelId = "")
    {
        var go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            return; // reuse
        }

        // create a GameObject and assign to this new user
        var videoSurface = MakeImageSurface(uid.ToString());
        if (ReferenceEquals(videoSurface, null)) return;
        // configure videoSurface
        if (uid == 0)
        {
            videoSurface.SetForUser(uid, channelId);
        }
        else
        {
            videoSurface.SetForUser(uid, channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
        }

        videoSurface.OnTextureSizeModify += (int width, int height) =>
        {
            float scale = (float)height / (float)width;
            videoSurface.transform.localScale = new Vector3(-5, 5 * scale, 1);
            Debug.Log("OnTextureSizeModify: " + width + "  " + height);
        };

        videoSurface.SetEnable(true);
    }
    */
#endregion

#region �¼�
internal class UserEventHandler : IRtcEngineEventHandler
    {
        private readonly VideoChatHelper _videoSample;

        internal UserEventHandler(VideoChatHelper videoSample)
        {
            _videoSample = videoSample;
        }

        public override void OnError(int err, string msg)
        {
            Debug.LogError("[VideoChatHelper]API��������" + string.Format("OnError err: {0}, msg: {1}", err, msg));
        }

        public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            int build = 0;
            Debug.Log("[VideoChatHelper]�ɹ�����Ƶ����");
            Debug.Log(string.Format("sdk version: ${0}",
                _videoSample.RtcEngine.GetVersion(ref build)));
            Debug.Log(string.Format("sdk build: ${0}",
              build));
            Debug.Log(
                string.Format("OnJoinChannelSuccess channelName: {0}, uid: {1}, elapsed: {2}",
                                connection.channelId, connection.localUid, elapsed));

            //��������µ���Ƶ����
            GameObject go = new GameObject("VideoChatTexture_uid_" + connection.localUid);
            go.transform.parent = VideoChatHelper.Instance.transform;
            VideoChatTexture txt = go.AddComponent<VideoChatTexture>();
            txt.SetVideoStreamIdentity(connection.localUid, VideoChatHelper.Instance.channelName, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_CAMERA_PRIMARY, VIDEO_OBSERVER_FRAME_TYPE.FRAME_TYPE_RGBA);
            VideoChatHelper.Instance.RealtimeVideos.Add(connection.localUid, txt);
            VideoChatHelper.Instance.OnVideoTextureCreated.Invoke(connection.localUid);
        }

        public override void OnRejoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            Debug.Log("[VideoChatHelper]�ѳɹ�����Ƶ����");
        }

        public override void OnLeaveChannel(RtcConnection connection, RtcStats stats)
        {
            Debug.Log("[VideoChatHelper]���˳�Ƶ����");
        }

        public override void OnClientRoleChanged(RtcConnection connection, CLIENT_ROLE_TYPE oldRole, CLIENT_ROLE_TYPE newRole, ClientRoleOptions newRoleOptions)
        {
            Debug.Log("[VideoChatHelper]�û���ɫ�����仯��ԭ���Ľ�ɫ��"+oldRole.ToString()+"���½�ɫ��"+newRole.ToString());
        }

        public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
        {
            Debug.Log("[VideoChatHelper]���µ��û�����Ƶ����" + string.Format("uid: ${0} elapsed: ${1}", uid, elapsed));
            //��������µ���Ƶ����
            GameObject go = new GameObject("VideoChatTexture_uid_" + uid);
            go.transform.parent = VideoChatHelper.Instance.transform;
            VideoChatTexture txt = go.AddComponent<VideoChatTexture>();
            txt.SetVideoStreamIdentity(uid, VideoChatHelper.Instance.channelName, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_CAMERA_PRIMARY, VIDEO_OBSERVER_FRAME_TYPE.FRAME_TYPE_RGBA);
            VideoChatHelper.Instance.RealtimeVideos.Add(uid, txt);
            VideoChatHelper.Instance.OnVideoTextureCreated.Invoke(uid);
        }

        public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            Debug.Log("[VideoChatHelper]���û��˳�Ƶ����" + string.Format("OnUserOffLine uid: ${0}, reason: ${1}", uid,
                (int)reason));
            //���������ٸ��û���Ƶ����
            Destroy(VideoChatHelper.Instance.RealtimeVideos[uid].gameObject);
            VideoChatHelper.Instance.RealtimeVideos.Remove(uid);
            VideoChatHelper.Instance.OnVideoTextureDestroyed.Invoke(uid);
        }

        public override void OnUplinkNetworkInfoUpdated(UplinkNetworkInfo info)
        {
            Debug.Log("[VideoChatHelper]�������籨�ĸ��£�OnUplinkNetworkInfoUpdated");
        }

        public override void OnDownlinkNetworkInfoUpdated(DownlinkNetworkInfo info)
        {
            Debug.Log("[VideoChatHelper]�������籨�ĸ��£�OnDownlinkNetworkInfoUpdated");
        }
    }
    #endregion
}
