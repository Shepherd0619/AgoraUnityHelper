// Video Chat Helper
// 作者：朱梓瑞 (Shepherd0619)
// 主要方便调用视频频道通话
using System.Collections.Generic;
using UnityEngine;
using Agora.Rtc;
using UnityEngine.Serialization;
using System;
using UnityEngine.Android;

public class VideoChatHelper : MonoBehaviour
{
    [Header("本脚本主要方便调用视频频道通话，只输出RawImage，方便万用")]
    [Header("基本配置")]
    [FormerlySerializedAs("APP_ID")]
    [SerializeField]
    public string appID = "";

    [FormerlySerializedAs("TOKEN")]
    [SerializeField]
    public string token = "";

    [FormerlySerializedAs("频道名称")]
    [SerializeField]
    public string channelName = "";

    private IRtcEngine RtcEngine;

    public Dictionary<uint, VideoChatTexture> RealtimeVideos = new Dictionary<uint, VideoChatTexture>();

    public static VideoChatHelper Instance;

    public ChannelMediaOptions Options;

    #region Action事件
    //视频画面创建完毕时回调
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

    #region 引擎核心
    /// <summary>
    /// 初始化引擎
    /// </summary>
    public void RTC_Initialize()
    {
        if (string.IsNullOrWhiteSpace(appID) || string.IsNullOrWhiteSpace(token))
        {
            Debug.LogError("[VideoChatHelper]AppID或Token未配置。");
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
                        Debug.LogError("[VideoChatHelper]初始化失败！请确认环境是否正确配置");
                        return;
                    }
                case -2:
                    {
                        Debug.LogError("[VideoChatHelper]初始化失败！参数非法");
                        return;
                    }
                case -7:
                    {
                        Debug.LogError("[VideoChatHelper]初始化失败！SDK未正常初始化");
                        return;
                    }
                case -22:
                    {
                        Debug.LogError("[VideoChatHelper]初始化失败！系统资源不足");
                        return;
                    }
                case -101:
                    {
                        Debug.LogError("[VideoChatHelper]初始化失败！AppID无效");
                        return;
                    }
                default:
                    {
                        Debug.LogError("[VideoChatHelper]初始化失败！原因未知");
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

        Debug.Log("[VideoChatHelper]引擎初始化完成！");
    }

    

    /// <summary>
    /// 销毁引擎
    /// </summary>
    public void RTC_Destroy()
    {
        if (RtcEngine == null) return;
        RtcEngine.LeaveChannel();
        RtcEngine.InitEventHandler(null);
        RtcEngine.Dispose();
        Debug.Log("[VideoChatHelper]已销毁RTC音视频通话引擎！");
    }

    private void OnDestroy()
    {
        RTC_Destroy();
    }
    #endregion

    #region 设备设置
    /// <summary>
    /// 指定默认相机，请确保系统能正常识别到摄像头
    /// </summary>
    public void SetCameraDevice()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        IVideoDeviceManager mgr = RtcEngine.GetVideoDeviceManager();
        DeviceInfo[] result = mgr.EnumerateVideoDevices();
        if (result.Length == 0)
        {
            Debug.LogError("[VideoChatHelper]未能检测到设备上的摄像头！");
        }
        else
        {
            mgr.SetDevice(result[0].deviceId);
            Debug.Log("[VideoChatHelper]" + "已成功自动设置默认摄像头！" + mgr.EnumerateVideoDevices()[0].deviceName);
        }
#endif
    }

    /// <summary>
    /// 指定默认音频设备，请确保系统能正常识别麦克风
    /// </summary>
    public void SetAudioDevice()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        IAudioDeviceManager mgr = RtcEngine.GetAudioDeviceManager();
        DeviceInfo[] result = mgr.EnumeratePlaybackDevices();
        if (result.Length == 0)
        {
            Debug.LogError("[VideoChatHelper]未能检测到设备上的音频设备！");
        }
        else
        {
            mgr.SetPlaybackDevice(result[0].deviceId);
            Debug.Log("[VideoChatHelper]" + "已成功自动设置默认音频设备！" + mgr.EnumeratePlaybackDevices()[0].deviceName);
        }
#endif
    }

    /// <summary>
    /// 手动指定某个为音频设备
    /// </summary>
    /// <param name="device">音频设备（不知道哪个？见mgr.EnumeratePlaybackDevices()返回的数组）</param>
    public void SetAudioDevice(DeviceInfo device)
    {
        IAudioDeviceManager mgr = RtcEngine.GetAudioDeviceManager();
        mgr.SetPlaybackDevice(device.deviceId);
        Debug.Log("[VideoChatHelper]已手动设置音频设备！"+device.deviceName);
    }

    /// <summary>
    /// 手动指定某个为摄像头设备
    /// </summary>
    /// <param name="device"></param>
    public void SetCameraDevice(DeviceInfo device)
    {
        IVideoDeviceManager mgr = RtcEngine.GetVideoDeviceManager();
        mgr.SetDevice(device.deviceName);
        Debug.Log("[VideoChatHelper]已手动设置摄像头！" + device.deviceName);
    }

    /// <summary>
    /// 摄像头、麦克风开关
    /// </summary>
    /// <param name="mic">麦克风开关</param>
    /// <param name="cam">摄像头开关</param>
    public void PublishSettings(bool mic, bool cam)
    {
        Options.publishMicrophoneTrack.SetValue(mic);
        Options.publishCameraTrack.SetValue(cam);
        RtcEngine.UpdateChannelMediaOptions(Options);
        Debug.Log("[VideoChatHelper]麦克风开关更新！" + mic);
        Debug.Log("[VideoChatHelper]摄像头开关更新！" + cam);
    }

    public void StartAudioPublish()
    {
        Options.publishMicrophoneTrack.SetValue(true);
        RtcEngine.UpdateChannelMediaOptions(Options);
        Debug.Log("[VideoChatHelper]麦克风开关更新！" + true);
    }

    public void StopAudioPublish()
    {
        Options.publishMicrophoneTrack.SetValue(false);
        RtcEngine.UpdateChannelMediaOptions(Options);
        Debug.Log("[VideoChatHelper]麦克风开关更新！" + false);
    }

    public void StartCameraPublish()
    {
        Options.publishCameraTrack.SetValue(true);
        RtcEngine.UpdateChannelMediaOptions(Options);
        Debug.Log("[VideoChatHelper]摄像头开关更新！" + true);
    }

    public void StopCameraPublish()
    {
        Options.publishCameraTrack.SetValue(false);
        RtcEngine.UpdateChannelMediaOptions(Options);
        Debug.Log("[VideoChatHelper]摄像头开关更新！" + false);
    }
    #endregion

    #region 频道
    /// <summary>
    /// 加入频道
    /// </summary>
    public void RTC_JoinChannel()
    {
        //在这里销毁用户视频画面
        foreach (KeyValuePair<uint, VideoChatTexture> pair in VideoChatHelper.Instance.RealtimeVideos)
        {
            VideoChatHelper.Instance.OnVideoTextureDestroyed.Invoke(pair.Key);
            Destroy(pair.Value.gameObject);

        }
        VideoChatHelper.Instance.RealtimeVideos.Clear();
        RtcEngine.JoinChannel(token, channelName);
        //在这里加新的视频画面
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
        Debug.Log("[VideoChatHelper]加入频道！" + channelName);
    }

    /// <summary>
    /// 加入频道
    /// </summary>
    /// <param name="name">频道名称</param>
    public void RTC_JoinChannel(string name)
    {
        //在这里销毁用户视频画面
        foreach (KeyValuePair<uint, VideoChatTexture> pair in VideoChatHelper.Instance.RealtimeVideos)
        {
            VideoChatHelper.Instance.OnVideoTextureDestroyed.Invoke(pair.Key);
            Destroy(pair.Value.gameObject);

        }
        VideoChatHelper.Instance.RealtimeVideos.Clear();
        channelName = name;
        RtcEngine.JoinChannel(token, name);
        //在这里加新的视频画面
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
        Debug.Log("[VideoChatHelper]加入频道！" + channelName);
    }

    /// <summary>
    /// 退出频道
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

    #region 事件
    internal class UserEventHandler : IRtcEngineEventHandler
    {
        private readonly VideoChatHelper _videoSample;

        internal UserEventHandler(VideoChatHelper videoSample)
        {
            _videoSample = videoSample;
        }

        public override void OnError(int err, string msg)
        {
            Debug.LogError("[VideoChatHelper]API遇到错误！" + string.Format("OnError err: {0}, msg: {1}", err, msg));
        }

        public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            int build = 0;
            Debug.Log("[VideoChatHelper]成功加入频道！");
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
            Debug.Log("[VideoChatHelper]已成功重连频道！");
        }

        public override void OnLeaveChannel(RtcConnection connection, RtcStats stats)
        {
            Debug.Log("[VideoChatHelper]已退出频道！");
            //在这里销毁用户视频画面
            foreach(KeyValuePair<uint, VideoChatTexture> pair in VideoChatHelper.Instance.RealtimeVideos)
            {
                VideoChatHelper.Instance.OnVideoTextureDestroyed.Invoke(pair.Key);
                Destroy(pair.Value.gameObject);

            }
            VideoChatHelper.Instance.RealtimeVideos.Clear();
        }

        public override void OnClientRoleChanged(RtcConnection connection, CLIENT_ROLE_TYPE oldRole, CLIENT_ROLE_TYPE newRole, ClientRoleOptions newRoleOptions)
        {
            Debug.Log("[VideoChatHelper]用户角色发生变化！原来的角色："+oldRole.ToString()+"。新角色："+newRole.ToString());
        }

        public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
        {
            Debug.Log("[VideoChatHelper]有新的用户加入频道！" + string.Format("uid: ${0} elapsed: ${1}", uid, elapsed));
            //在这里加新的视频画面
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
            Debug.Log("[VideoChatHelper]有用户退出频道！" + string.Format("OnUserOffLine uid: ${0}, reason: ${1}", uid,
                (int)reason));
            //在这里销毁该用户视频画面
            Destroy(VideoChatHelper.Instance.RealtimeVideos[uid].gameObject);
            VideoChatHelper.Instance.RealtimeVideos.Remove(uid);
            VideoChatHelper.Instance.OnVideoTextureDestroyed.Invoke(uid);
        }

        public override void OnUplinkNetworkInfoUpdated(UplinkNetworkInfo info)
        {
            Debug.Log("[VideoChatHelper]上游网络报文更新！OnUplinkNetworkInfoUpdated");
        }

        public override void OnDownlinkNetworkInfoUpdated(DownlinkNetworkInfo info)
        {
            Debug.Log("[VideoChatHelper]下游网络报文更新！OnDownlinkNetworkInfoUpdated");
        }
    }
    #endregion
}