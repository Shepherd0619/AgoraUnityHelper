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

public class VideoChatHelper : MonoBehaviour
{
    [Header("���ű���Ҫ���������ƵƵ��ͨ��")]
    [Header("��������")]
    [FormerlySerializedAs("APP_ID")]
    [SerializeField]
    private string appID = "";

    [FormerlySerializedAs("TOKEN")]
    [SerializeField]
    private string token = "";

    [FormerlySerializedAs("Ƶ������")]
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
            Debug.LogError("[VideoChatHelper]AppID��Tokenδ���á�");
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
        Debug.Log("[VideoChatHelper]������RTC����Ƶͨ�����棡");
    }

    private void OnDestroy()
    {
        RTC_Destroy();
    }

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
        }

        public override void OnClientRoleChanged(RtcConnection connection, CLIENT_ROLE_TYPE oldRole, CLIENT_ROLE_TYPE newRole, ClientRoleOptions newRoleOptions)
        {
            Debug.Log("[VideoChatHelper]�û���ɫ�����仯��ԭ���Ľ�ɫ��"+oldRole.ToString()+"���½�ɫ��"+newRole.ToString());
        }

        public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
        {
            Debug.Log("[VideoChatHelper]���µ��û�����Ƶ����" + string.Format("uid: ${0} elapsed: ${1}", uid, elapsed));
            //��������µ���Ƶ����

        }

        public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            Debug.Log("[VideoChatHelper]���û��˳�Ƶ����" + string.Format("OnUserOffLine uid: ${0}, reason: ${1}", uid,
                (int)reason));
            //���������ٸ��û���Ƶ����
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
