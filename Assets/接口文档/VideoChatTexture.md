# VideoChatTexture 类

此类主要方便用户UID和画面一一对应以及实时更新视频画面

## VideoChatTexture.Width

- 类型: int
- 说明: 获取视频像素的宽度

## VideoChatTexture.Height

- 类型: int
- 说明: 获取视频像素的高度

## VideoChatTexture.InitTexture()

- 类型: void
- 说明: 初始化Texture

## VideoChatTexture.InitIrisVideoFrame()

- 类型: void
- 说明: 初始化Iris的视频帧

## VideoChatTexture.GetRefCount()

- 类型: int
- 说明: 获取引用计数

## VideoChatTexture.CanTextureAttach()

- 类型: bool
- 说明: 检查是否可以附加Texture

## VideoChatTexture.EnableVideoFrameWithIdentity()

- 类型: void
- 说明: 使用标识符激活视频帧

## VideoChatTexture.ReFreshTexture()

- 类型: void
- 说明: 刷新Texture

## VideoChatTexture.SetVideoStreamIdentity(uint uid, string channelId, 
VIDEO_SOURCE_TYPE source_type, VIDEO_OBSERVER_FRAME_TYPE frameType)

- 类型: void
- 参数:
  - uid: uint，用户ID
  - channelId: string，频道ID
  - source_type: VIDEO_SOURCE_TYPE，视频源类型
  - frameType: VIDEO_OBSERVER_FRAME_TYPE，视频帧类型
- 说明: 设置视频流标识符

## VideoChatTexture.Attach()

- 类型: void
- 说明: 附加视频表面

## VideoChatTexture.Detach()

- 类型: void
- 说明: 分离视频表面

## VideoChatTexture.DestroyTexture()

- 类型: void
- 说明: 销毁Texture

## 枚举类型

### VIDEO_SOURCE_TYPE

```csharp
public enum VIDEO_SOURCE_TYPE
{
    VIDEO_SOURCE_CAMERA_PRIMARY = 0,
    VIDEO_SOURCE_CAMERA_SECONDARY = 1,
    VIDEO_SOURCE_SCREEN_PRIMARY = 2,
    VIDEO_SOURCE_SCREEN_SECONDARY = 3,
    VIDEO_SOURCE_CUSTOM = 4,
}
```

- VIDEO_SOURCE_CAMERA_PRIMARY: 摄像头源 - 主摄像头
- VIDEO_SOURCE_CAMERA_SECONDARY: 摄像头源 - 副摄像头
- VIDEO_SOURCE_SCREEN_PRIMARY: 屏幕共享源 - 主屏幕
- VIDEO_SOURCE_SCREEN_SECONDARY: 屏幕共享源 - 辅助屏幕
- VIDEO_SOURCE_CUSTOM: 自定义视频源

### VIDEO_OBSERVER_FRAME_TYPE

```csharp
public enum VIDEO_OBSERVER_FRAME_TYPE
{
    FRAME_TYPE_YUV420 = 0,
    FRAME_TYPE_YUV420_TEXTURE = 1,
    FRAME_TYPE_RGBA = 2,
    FRAME_TYPE_MJPEG = 3,
    FRAME_TYPE_RGBA_TEXTURE = 4,
}
```

- FRAME_TYPE_YUV420: YUV420格式的视频帧
- FRAME_TYPE_YUV420_TEXTURE: 包含YUV420格式的视频帧的纹理
- FRAME_TYPE_RGBA: RGBA格式的视频帧
- FRAME_TYPE_MJPEG: MJPEG格式的视频帧
- FRAME_TYPE_RGBA_TEXTURE: 包含RGBA格式的视频帧的纹理