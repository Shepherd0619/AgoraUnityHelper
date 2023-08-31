## JoinChannelVideo 类

用途：在 Unity 中实现基本的加入频道和视频功能。

### 属性

- `_appIdInput`：AppIdInput 组件，用于获取 App ID。
- `_appID`：string 类型，App ID。
- `_token`：string 类型，Token。
- `_channelName`：string 类型，频道名称。
- `LogText`：Text 组件，用于显示日志信息。
- `Log`：Logger 类型，用于管理日志信息。
- `RtcEngine`：IRtcEngine 类型，RTC 引擎。
- `_videoDeviceSelect`：Dropdown 组件，用于选择视频设备。
- `_videoDeviceManager`：IVideoDeviceManager 类型，视频设备管理器。
- `_videoDeviceInfos`：DeviceInfo 数组，视频设备信息。

### 方法

#### `Start()` 方法

说明：初始化引擎并设置基本配置。
参数：无。
返回值：无。

#### `Update()` 方法

说明：在每一帧中调用，请求麦克风和摄像头权限。
参数：无。
返回值：无。

#### `LoadAssetData()` 方法

说明：从 AppIdInput 组件中加载数据。
参数：无。
返回值：无。

#### `CheckAppId()` 方法

说明：检查 App ID 是否有效。
参数：无。
返回值：bool 类型，App ID 是否有效。

#### `InitEngine()` 方法

说明：初始化引擎。
参数：无。
返回值：无。

#### `SetBasicConfiguration()` 方法

说明：设置基本配置。
参数：无。
返回值：无。

#### `JoinChannel()` 方法

说明：加入频道。
参数：无。
返回值：无。

#### `LeaveChannel()` 方法

说明：离开频道。
参数：无。
返回值：无。

#### `StartPreview()` 方法

说明：开始预览。
参数：无。
返回值：无。

#### `StopPreview()` 方法

说明：停止预览。
参数：无。
返回值：无。

#### `StartPublish()` 方法

说明：开始发布音视频流。
参数：无。
返回值：无。

#### `StopPublish()` 方法

说明：停止发布音视频流。
参数：无。
返回值：无。

#### `AdjustVideoEncodedConfiguration640()` 方法

说明：调整视频编码配置为 640x360。
参数：无。
返回值：无。

#### `AdjustVideoEncodedConfiguration480()` 方法

说明：调整视频编码配置为 480x480。
参数：无。
返回值：无。

#### `GetVideoDeviceManager()` 方法

说明：获取视频设备管理器。
参数：无。
返回值：无。

#### `SelectVideoCaptureDevice()` 方法

说明：选择视频采集设备。
参数：无。
返回值：无。

#### `OnDestroy()` 方法

说明：销毁时调用，释放资源。
参数：无。
返回值：无。

### 内部类：UserEventHandler

用途：处理 RTC 引擎事件。

#### 方法

#### `OnError()` 方法

说明：发生错误时调用。
参数：int 类型的错误码，string 类型的错误信息。
返回值：无。

#### `OnJoinChannelSuccess()` 方法

说明：加入频道成功时调用。
参数：RtcConnection 类型的连接信息，int 类型的时间。
返回值：无。

#### `OnRejoinChannelSuccess()` 方法

说明：重新加入频道成功时调用。
参数：RtcConnection 类型的连接信息，int 类型的时间。
返回值：无。

#### `OnLeaveChannel()` 方法

说明：离开频道时调用。
参数：RtcConnection 类型的连接信息，RtcStats 类型的统计信息。
返回值：无。

#### `OnClientRoleChanged()` 方法

说明：客户端角色发生变化时调用。
参数：RtcConnection 类型的连接信息，
      CLIENT_ROLE_TYPE 类型的旧的角色，
      CLIENT_ROLE_TYPE 类型的新的角色，
      ClientRoleOptions 类型的新的角色选项。
返回值：无。

#### `OnUserJoined()` 方法

说明：有新用户加入频道时调用。
参数：RtcConnection 类型的连接信息，
      uint 类型的用户 ID，
      int 类型的时间。
返回值：无。

#### `OnUserOffline()` 方法

说明：有用户离开频道时调用。
参数：RtcConnection 类型的连接信息，
      uint 类型的用户 ID，
      USER_OFFLINE_REASON_TYPE 类型的离线原因。
返回值：无。

#### `OnUplinkNetworkInfoUpdated()` 方法

说明：上行网络信息更新时调用。
参数：UplinkNetworkInfo 类型的网络信息。
返回值：无。

#### `OnDownlinkNetworkInfoUpdated()` 方法

说明：下行网络信息更新时调用。
参数：DownlinkNetworkInfo 类型的网络信息。
返回值：无。