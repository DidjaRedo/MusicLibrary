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

        private PlayerTimes() {}

        public static readonly PlayerTimes Default = new PlayerTimes() {
            Track = null,
            ElapsedTimeInSeconds = 0,
            RemainingTimeInSeconds = 0,
            FadeOutTimeInSeconds = 0
        };

        public ITrack Track { get; protected set; }
        public int TotalTimeInSeconds => Track?.DurationInSeconds ?? 0;
        public int ElapsedTimeInSeconds { get; protected set; }
        public int RemainingTimeInSeconds { get; protected set; }
        public int FadeOutTimeInSeconds { get; protected set; }
        public double FaderVolume {
            get {
                if ((FadeOutTimeInSeconds > 0) && (RemainingTimeInSeconds < FadeOutTimeInSeconds)) {
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

        public PlayerTimes Seek(int position) {
            if ((position >= 0) && (position < TotalTimeInSeconds)) {
                return new PlayerTimes(Track) {
                    ElapsedTimeInSeconds = position,
                    RemainingTimeInSeconds = TotalTimeInSeconds - position,
                    FadeOutTimeInSeconds = FadeOutTimeInSeconds
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
