using OeipWrapper;
using OeipWrapper.FixPipe;
using OeipWrapper.Live;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveView : MonoBehaviour
{
    [SerializeField]
    public Texture2D SourceTex = null;
    private OeipPipe3D pipe = null;
    public OeipLivePipe LivePipe = null;

    private int userId = -1;
    private int index = -1;

    void Awake()
    {
        pipe = OeipManager.Instance.CreatePipe<OeipPipe3D>(OeipGpgpuType.OEIP_CUDA);
        LivePipe = new OeipLivePipe(pipe);
        //显示        
        LivePipe.OnLiveImageChange += LivePipe_OnLiveImageChange;
        OeipLiveManager.Instance.OnVideoFrameEvent += Instance_OnVideoFrameEvent;
    }

    //当返回的长或是宽变化后
    private void LivePipe_OnLiveImageChange(VideoFormat videoFrame)
    {
        if (SourceTex == null || SourceTex.width != videoFrame.width || SourceTex.height != videoFrame.height)
        {
            SourceTex = new Texture2D((int)videoFrame.width, (int)videoFrame.height, TextureFormat.RGBA32, false);
            SourceTex.Apply();
            //Unity 纹理作为输出
            pipe.SetPipeOutputGpuTex(LivePipe.OutGpuIndex, SourceTex.GetNativeTexturePtr());
        }
    }

    public void SetPullUserIndex(int userId, int index)
    {
        this.userId = userId;
        this.index = index;
    }

    //拉流拉到数据
    private void Instance_OnVideoFrameEvent(int userId, int index, OeipVideoFrame videoFrame)
    {
        if (this.userId != userId || this.index != index)
            return;
        LivePipe.RunLivePipe(ref videoFrame);
    }

    // Update is called once per frame
    void Update()
    {
        if (pipe == null)
            return;
        pipe.UpdateOutTex();
    }
}
