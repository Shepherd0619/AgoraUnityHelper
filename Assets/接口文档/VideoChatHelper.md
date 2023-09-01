# VideoChatHelper 类

该类是一个视频聊天辅助类，用于方便调用视频频道通话，并输出RawImage。



## 属性

- `appID`：字符串。Agora的App ID。
- `token`：字符串。频道的访问令牌。
- `channelName`：字符串。频道的名称。
- `RealtimeVideos`：字典。保存实时视频的字典。
- `OnVideoTextureCreated`：Action。当视频画面创建完毕时触发的事件。（本脚本自定义）
- `OnVideoTextureDestroyed`：Action。当视频画面被销毁时触发的事件。（本脚本自定义）
- `Options`: ChannelMediaOption。里头包含音视频开关、分辨率、质量等。



## 方法

- `RTC_Initialize()`：初始化引擎。
- `RTC_JoinChannel()`：加入频道。
- `RTC_JoinChannel(string name)`：根据指定的频道名称加入频道。
- `RTC_LeaveChannel()`：退出频道。
- `RTC_Destroy()`：销毁引擎。
- `SetCameraDevice()`：指定默认相机。
- `SetAudioDevice()`：指定默认音频设备。
- `SetCameraDevice(DeviceInfo device)`：指定具体相机。
- `SetAudioDevice(DeviceInfo device)`：指定具体音频设备。



## 事件

以下事件如无特殊说明，均为SDK自带。

- `OnJoinChannelSuccess`：成功加入频道时触发的事件。
- `OnRejoinChannelSuccess`：成功重连频道时触发的事件。
- `OnLeaveChannel`：离开频道时触发的事件。
- `OnClientRoleChanged`：用户角色发生变化时触发的事件。
- `OnUserJoined`：有新的用户加入频道时触发的事件。
- `OnUserOffline`：有用户退出频道时触发的事件。
- `OnUplinkNetworkInfoUpdated`：上行网络信息更新时触发的事件。
- `OnDownlinkNetworkInfoUpdated`：下行网络信息更新时触发的事件。