// Video Chat Helper
// ���ߣ������� (Shepherd0619)
// ��Ҫ���������ƵƵ��ͨ��
using System.Collections.Generic;
using UnityEngine;
using Agora.Rtc;
using UnityEngine.Serialization;
using System;
using UnityEngine.Android;

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

    public Dictionary<uint, VideoChatTexture> RealtimeVideos = new Dictionary<uint, VideoChatTexture>();

    public static VideoChatHelper Instance;

    public ChannelMediaOptions Options;

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

    #region �������
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

        SetCameraDevice();
        SetAudioDevice();

        RtcEngine.EnableAudio();
        RtcEngine.EnableVideo();
        VideoEncoderConfiguration config = new VideoEncoderConfiguration();
        config.dimensions = new VideoDimensions(720, 400);
        config.frameRate = 15;
        config.bitrate = 0;
        RtcEngine.SetVideoEncoderConfiguration(config);
        RtcEngine.SetChannelProfile(CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_COMMUNICATION);
        RtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);

        Debug.Log("[VideoChatHelper]�����ʼ����ɣ�");
    }

    

    /// <summary>
    /// ��������
    /// </summary>
    public void RTC_Destroy()
    {
        if (RtcEngine == null) return;
        RtcEngine.LeaveChannel();
        RtcEngine.InitEventHandler(null);
        RtcEngine.Dispose();
        Debug.Log("[VideoChatHelper]������RTC����Ƶͨ�����棡");
    }

    private void OnDestroy()
    {
        RTC_Destroy();
    }
    #endregion

    #region �豸����
    /// <summary>
    /// ָ��Ĭ���������ȷ��ϵͳ������ʶ������ͷ
    /// </summary>
    public void SetCameraDevice()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        IVideoDeviceManager mgr = RtcEngine.GetVideoDeviceManager();
        DeviceInfo[] result = mgr.EnumerateVideoDevices();
        if (result.Length == 0)
        {
            Debug.LogError("[VideoChatHelper]δ�ܼ�⵽�豸�ϵ�����ͷ��");
        }
        else
        {
            mgr.SetDevice(result[0].deviceId);
            Debug.Log("[VideoChatHelper]" + "�ѳɹ��Զ�����Ĭ������ͷ��" + mgr.EnumerateVideoDevices()[0].deviceName);
        }
#endif
    }

    /// <summary>
    /// ָ��Ĭ����Ƶ�豸����ȷ��ϵͳ������ʶ����˷�
    /// </summary>
    public void SetAudioDevice()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        IAudioDeviceManager mgr = RtcEngine.GetAudioDeviceManager();
        DeviceInfo[] result = mgr.EnumeratePlaybackDevices();
        if (result.Length == 0)
        {
            Debug.LogError("[VideoChatHelper]δ�ܼ�⵽�豸�ϵ���Ƶ�豸��");
        }
        else
        {
            mgr.SetPlaybackDevice(result[0].deviceId);
            Debug.Log("[VideoChatHelper]" + "�ѳɹ��Զ�����Ĭ����Ƶ�豸��" + mgr.EnumeratePlaybackDevices()[0].deviceName);
        }
#endif
    }

    /// <summary>
    /// �ֶ�ָ��ĳ��Ϊ��Ƶ�豸
    /// </summary>
    /// <param name="device">��Ƶ�豸����֪���ĸ�����mgr.EnumeratePlaybackDevices()���ص����飩</param>
    public void SetAudioDevice(DeviceInfo device)
    {
        IAudioDeviceManager mgr = RtcEngine.GetAudioDeviceManager();
        mgr.SetPlaybackDevice(device.deviceId);
        Debug.Log("[VideoChatHelper]���ֶ�������Ƶ�豸��"+device.deviceName);
    }

    /// <summary>
    /// �ֶ�ָ��ĳ��Ϊ����ͷ�豸
    /// </summary>
    /// <param name="device"></param>
    public void SetCameraDevice(DeviceInfo device)
    {
        IVideoDeviceManager mgr = RtcEngine.GetVideoDeviceManager();
        mgr.SetDevice(device.deviceName);
        Debug.Log("[VideoChatHelper]���ֶ���������ͷ��" + device.deviceName);
    }

    /// <summary>
    /// ����ͷ����˷翪��
    /// </summary>
    /// <param name="mic">��˷翪��</param>
    /// <param name="cam">����ͷ����</param>
    public void PublishSettings(bool mic, bool cam)
    {
        Options.publishMicrophoneTrack.SetValue(mic);
        Options.publishCameraTrack.SetValue(cam);
        RtcEngine.UpdateChannelMediaOptions(Options);
        Debug.Log("[VideoChatHelper]��˷翪�ظ��£�" + mic);
        Debug.Log("[VideoChatHelper]����ͷ���ظ��£�" + cam);
    }

    public void StartAudioPublish()
    {
        Options.publishMicrophoneTrack.SetValue(true);
        RtcEngine.UpdateChannelMediaOptions(Options);
        Debug.Log("[VideoChatHelper]��˷翪�ظ��£�" + true);
    }

    public void StopAudioPublish()
    {
        Options.publishMicrophoneTrack.SetValue(false);
        RtcEngine.UpdateChannelMediaOptions(Options);
        Debug.Log("[VideoChatHelper]��˷翪�ظ��£�" + false);
    }

    public void StartCameraPublish()
    {
        Options.publishCameraTrack.SetValue(true);
        RtcEngine.UpdateChannelMediaOptions(Options);
        Debug.Log("[VideoChatHelper]����ͷ���ظ��£�" + true);
    }

    public void StopCameraPublish()
    {
        Options.publishCameraTrack.SetValue(false);
        RtcEngine.UpdateChannelMediaOptions(Options);
        Debug.Log("[VideoChatHelper]����ͷ���ظ��£�" + false);
    }
    #endregion

    #region Ƶ��
    /// <summary>
    /// ����Ƶ��
    /// </summary>
    public void RTC_JoinChannel()
    {
        //�����������û���Ƶ����
        foreach (KeyValuePair<uint, VideoChatTexture> pair in VideoChatHelper.Instance.RealtimeVideos)
        {
            VideoChatHelper.Instance.OnVideoTextureDestroyed.Invoke(pair.Key);
            Destroy(pair.Value.gameObject);

        }
        VideoChatHelper.Instance.RealtimeVideos.Clear();
        RtcEngine.JoinChannel(token, channelName);
        //��������µ���Ƶ����
        GameObject go = new GameObject("VideoChatTexture_uid_" + 0);
        go.transform.parent = VideoChatHelper.Instance.transform;
        VideoChatTexture txt = go.AddComponent<VideoChatTexture>();
        txt.SetVideoStreamIdentity(0, VideoChatHelper.Instance.channelName, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_CAMERA_PRIMARY, VIDEO_OBSERVER_FRAME_TYPE.FRAME_TYPE_RGBA);
        txt.EnableVideoFrameWithIdentity();
        VideoChatHelper.Instance.RealtimeVideos.Add(0, txt);
        VideoChatHelper.Instance.OnVideoTextureCreated.Invoke(0);
        Options = new ChannelMediaOptions();
        Options.publishMicrophoneTrack.SetValue(true);
        Options.publishCameraTrack.SetValue(true);
        var nRet = RtcEngine.UpdateChannelMediaOptions(Options);
        Debug.Log("[VideoChatHelper]����Ƶ����" + channelName);
    }

    /// <summary>
    /// ����Ƶ��
    /// </summary>
    /// <param name="name">Ƶ������</param>
    public void RTC_JoinChannel(string name)
    {
        //�����������û���Ƶ����
        foreach (KeyValuePair<uint, VideoChatTexture> pair in VideoChatHelper.Instance.RealtimeVideos)
        {
            VideoChatHelper.Instance.OnVideoTextureDestroyed.Invoke(pair.Key);
            Destroy(pair.Value.gameObject);

        }
        VideoChatHelper.Instance.RealtimeVideos.Clear();
        channelName = name;
        RtcEngine.JoinChannel(token, name);
        //��������µ���Ƶ����
        GameObject go = new GameObject("VideoChatTexture_uid_" + 0);
        go.transform.parent = VideoChatHelper.Instance.transform;
        VideoChatTexture txt = go.AddComponent<VideoChatTexture>();
        txt.SetVideoStreamIdentity(0, VideoChatHelper.Instance.channelName, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_CAMERA_PRIMARY, VIDEO_OBSERVER_FRAME_TYPE.FRAME_TYPE_RGBA);
        txt.EnableVideoFrameWithIdentity();
        VideoChatHelper.Instance.RealtimeVideos.Add(0, txt);
        VideoChatHelper.Instance.OnVideoTextureCreated.Invoke(0);
        Options = new ChannelMediaOptions();
        Options.publishMicrophoneTrack.SetValue(true);
        Options.publishCameraTrack.SetValue(true);
        var nRet = RtcEngine.UpdateChannelMediaOptions(Options);
        Debug.Log("[VideoChatHelper]����Ƶ����" + channelName);
    }

    /// <summary>
    /// �˳�Ƶ��
    /// </summary>
    public void RTC_LeaveChannel()
    {
        Options = new ChannelMediaOptions();
        Options.publishMicrophoneTrack.SetValue(false);
        Options.publishCameraTrack.SetValue(false);
        var nRet = RtcEngine.UpdateChannelMediaOptions(Options);
        RtcEngine.LeaveChannel();
    }
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
        }

        public override void OnRejoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            Debug.Log("[VideoChatHelper]�ѳɹ�����Ƶ����");
        }

        public override void OnLeaveChannel(RtcConnection connection, RtcStats stats)
        {
            Debug.Log("[VideoChatHelper]���˳�Ƶ����");
            //�����������û���Ƶ����
            foreach(KeyValuePair<uint, VideoChatTexture> pair in VideoChatHelper.Instance.RealtimeVideos)
            {
                VideoChatHelper.Instance.OnVideoTextureDestroyed.Invoke(pair.Key);
                Destroy(pair.Value.gameObject);

            }
            VideoChatHelper.Instance.RealtimeVideos.Clear();
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
            txt.SetVideoStreamIdentity(uid, VideoChatHelper.Instance.channelName, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE, VIDEO_OBSERVER_FRAME_TYPE.FRAME_TYPE_RGBA);
            txt.EnableVideoFrameWithIdentity();
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