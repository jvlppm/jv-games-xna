using System;

namespace Jv.Games.Xna.Sprites
{
    public class Animation
    {
        public Frame[] Frames { get; private set; }
        public TimeSpan FrameDuration { get; private set; }
        public int ResetToFrame { get; private set; }

        public Animation(Frame[] frames, TimeSpan frameDuration, int resetToFrame = 0)
        {
            Frames = frames;
            FrameDuration = frameDuration;
            ResetToFrame = resetToFrame;
        }
    }
}
