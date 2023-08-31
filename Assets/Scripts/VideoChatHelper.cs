// Video Chat Helper
// 作者：朱梓瑞 (Shepherd0619)
// 主要方便调用视频频道通话
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Agora.Rtc;
using UnityEngine.Serialization;
using UnityEngine.Networking.Types;
using Agora_RTC_Plugin.API_Example.Examples.Basic.JoinChannelVideo;

public class VideoChatHelper : MonoBehaviour
{
    [Header("本脚本主要方便调用视频频道通话")]
    [Header("基本配置")]
    [FormerlySerializedAs("APP_ID")]
    [SerializeField]
    private string appID = "";

    [FormerlySerializedAs("TOKEN")]
    [SerializeField]
    private string token = "";

    [FormerlySerializedAs("频道名称")]
    [SerializeField]
    private string channelName = "";

    private IRtcEngine RtcEngine;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void RTC_Initialize()
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
        RtcEngine.Initialize(context);
        RtcEngine.InitEventHandler(handler);
    }

    private void RTC_Destroy()
    {
        if (RtcEngine == null) return;
        RtcEngine.InitEventHandler(null);
        RtcEngine.LeaveChannel();
        RtcEngine.Dispose();
        Debug.Log("[VideoChatHelper]已销毁RTC音视频通话引擎！");
    }

    private void OnDestroy()
    {
        RTC_Destroy();
    }

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
        }

        public override void OnClientRoleChanged(RtcConnection connection, CLIENT_ROLE_TYPE oldRole, CLIENT_ROLE_TYPE newRole, ClientRoleOptions newRoleOptions)
        {
            Debug.Log("[VideoChatHelper]用户角色发生变化！原来的角色："+oldRole.ToString()+"。新角色："+newRole.ToString());
        }

        public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
        {
            Debug.Log("[VideoChatHelper]有新的用户加入频道！" + string.Format("uid: ${0} elapsed: ${1}", uid, elapsed));
            //在这里加新的视频画面

        }

        public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            Debug.Log("[VideoChatHelper]有用户退出频道！" + string.Format("OnUserOffLine uid: ${0}, reason: ${1}", uid,
                (int)reason));
            //在这里销毁该用户视频画面
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
