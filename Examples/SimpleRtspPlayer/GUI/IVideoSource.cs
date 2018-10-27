using System;
using Client.RawFramesDecoding.DecodedFrames;

namespace Client.GUI
{
    public interface IVideoSource
    {
        event EventHandler<IDecodedVideoFrame> FrameReceived;

        void SetVideoSize(int width, int height);
    }
}