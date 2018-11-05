using System;

using MusicLibrary.Lib;

namespace DanceDj.Core
{

    public class PlayerTimes
    {
        public PlayerTimes(ITrack track) : this(track, 0) {
        }

        public PlayerTimes (ITrack track, int fade) {
            Track = track;
            ElapsedTimeInSeconds = 0;
            RemainingTimeInSeconds = track.DurationInSeconds;
            FadeOutTimeInSeconds = fade;
        }

        public ITrack Track { get; protected set; }
        public int TotalTimeInSeconds => Track.DurationInSeconds;
        public int ElapsedTimeInSeconds { get; protected set; }
        public int RemainingTimeInSeconds { get; protected set; }
        public int FadeOutTimeInSeconds { get; protected set; }
        public double FaderVolume {
            get {
                if (RemainingTimeInSeconds < FadeOutTimeInSeconds) {
                    return ((double)RemainingTimeInSeconds) / ((double)FadeOutTimeInSeconds);
                }
                return 1.0;
            }
        }

        public PlayerTimes Tick() {
            if (RemainingTimeInSeconds > 0) {
                return new PlayerTimes(Track) {
                    ElapsedTimeInSeconds = ElapsedTimeInSeconds + 1,
                    RemainingTimeInSeconds = RemainingTimeInSeconds - 1,
                    FadeOutTimeInSeconds = FadeOutTimeInSeconds,
                };
            }
            return this;
        }

        public PlayerTimes Reset() {
            return new PlayerTimes(Track, FadeOutTimeInSeconds);
        }

        public PlayerTimes StartFade() {
            if ((RemainingTimeInSeconds > 0) && (FadeOutTimeInSeconds > 0)) {
                int fadeDuration = Math.Min(TotalTimeInSeconds - ElapsedTimeInSeconds, FadeOutTimeInSeconds);
                return new PlayerTimes(Track) {
                    ElapsedTimeInSeconds = ElapsedTimeInSeconds,
                    RemainingTimeInSeconds = fadeDuration,
                    FadeOutTimeInSeconds = FadeOutTimeInSeconds,
                };
            }
            return this;
        }
    }
}
