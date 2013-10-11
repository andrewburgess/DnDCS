﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DnDCS_Client.Shared
{
    public class FrameAnimation<T>
    {
        public const int REPEAT_FOREVER = -1;

        public bool IsRunning { get; private set; }

        public TimeSpan StartGameTime { get; private set; }
        private float ElapsedSinceStart
        {
            get
            {
                // If we're currently repeating, we'll always offset the time elapsed so we properly skip over all the frames before the repeat point.
                if (isRepeating)
                    return (float)(CurrentGameTime.TotalSeconds - StartGameTime.TotalSeconds) + FrameIntervals[RepeatToIndex].Item1;
                else
                    return (float)(CurrentGameTime.TotalSeconds - StartGameTime.TotalSeconds);
            }
        }

        public T[] Frames { get; private set; }
        public Tuple<float, int>[] FrameIntervals { get; private set; }

        public bool IsComplete { get; private set; }
        public Action OnComplete { get; set; }

        public bool Repeat { get; private set; }
        private bool isRepeating;
        public float RepeatDelay { get; private set; }
        public int RepeatToIndex { get; private set; }
        public int RepeatCount { get; private set; }
        public int CurrentRepeatCount { get; private set; }

        public TimeSpan LastGameTime { get; private set; }
        public TimeSpan CurrentGameTime { get; private set; }
        public int CurrentFrameIntervalIndex { get; private set; }
        public T CurrentFrame { get { return Frames[FrameIntervals[CurrentFrameIntervalIndex].Item2]; } }

        /// <summary> Creates a new Frame Translation. </summary>
        /// <param name="frames"> The frames to show. </param>
        /// <param name="frameIntervals"> The Frame Intervals to show for the frames. </param>
        public FrameAnimation(T[] frames, Tuple<float, int>[] frameIntervals)
        {
            if (frames.Length == 0 || frameIntervals.Length == 0)
                throw new InvalidOperationException("Frames and Interval arrays must both have at least 1 element.");

            Frames = frames;
            FrameIntervals = frameIntervals;
        }

        public void SetRepeat(float repeatDelay, int repeatToIndex = 0, int repeatCount = REPEAT_FOREVER)
        {
            if (IsRunning)
                throw new InvalidOperationException("Cannot change repeat values for a running animation.");
            else if (repeatToIndex < 0 || repeatToIndex >= FrameIntervals.Length)
                throw new ArgumentException("RepeatToIndex must be a valid frame index.", "repeatToIndex");
            else if (repeatDelay < 0.0f)
                throw new ArgumentException("RepeatDelay must be greater than 0.", "repeatDelay");

            Repeat = true;
            RepeatDelay = repeatDelay;
            RepeatToIndex = repeatToIndex;
            RepeatCount = repeatCount;
        }

        /// <summary> Resets any values that would allow the animation to start again, and starts it. Can only be called when IsRunning is false. </summary>
        public void Start(GameTime startTime)
        {
            if (IsRunning)
                throw new InvalidOperationException("Can only start a non-running animation.");

            Reset();

            IsRunning = true;
            StartGameTime = startTime.TotalGameTime;
            CurrentGameTime = startTime.TotalGameTime;
            LastGameTime = startTime.TotalGameTime;
        }

        /// <summary> Resets any values that would allow the animation to start again. Can only be called when IsRunning is false. </summary>
        public void Reset()
        {
            if (IsRunning)
                throw new InvalidOperationException("Can only reset a non-running animation.");

            IsComplete = false;
            isRepeating = false;
            CurrentFrameIntervalIndex = 0;
            CurrentRepeatCount = 0;
        }

        public void Stop(bool reset = false)
        {
            IsRunning = false;
            if (reset)
                Reset();
        }

        public void Update(GameTime gameTime, bool startIfNeeded = false)
        {
            if (IsComplete)
                return;
            else if (!IsRunning)
            {
                if (startIfNeeded)
                    Start(gameTime);
                else
                    return;
            }

            LastGameTime = CurrentGameTime;
            CurrentGameTime = gameTime.TotalGameTime;

            Update_Frame();

            if (IsComplete && OnComplete != null)
            {
                OnComplete();
            }
        }

        private void Update_Frame()
        {
            var elapsedSinceStart = ElapsedSinceStart;

            if (elapsedSinceStart < FrameIntervals[0].Item1)
            {
                // Still before the first interval, which should normally be 0.0f, so we'll show the first frame.
                CurrentFrameIntervalIndex = 0;
                return;
            }

            for (var i = CurrentFrameIntervalIndex; i < FrameIntervals.Length; i++)
            {
                if (elapsedSinceStart < FrameIntervals[i].Item1)
                {
                    // The frame we want to show will always be the one immediately after the one the time has just passed over.
                    // However, in situations where the interval is shorter than the Update cycle, we'll enforce that
                    // the frame will show at least briefly by simply incrementing the frame index by 1 ever time.
                    var newIndex = Math.Max(0, i - 1);
                    if (CurrentFrameIntervalIndex != newIndex)
                        CurrentFrameIntervalIndex++;
                    return;
                }
            }

            if (Repeat)
            {
                // If we're on the last frame, then we'll cycle over to the first one once the Repeat Delay elapses.
                // Otherwise, we'll just shift to the next frame because the Update cycle may simply be delayed.
                if (CurrentFrameIntervalIndex == (FrameIntervals.Length - 1))
                {
                    // Repeating, so wait until the last interval + the repeat delay before going back to the Repeat Frame (normally 0) and reseting the Start Time.
                    if (elapsedSinceStart > FrameIntervals.Last().Item1 + RepeatDelay)
                    {
                        if (RepeatCount != REPEAT_FOREVER)
                        {
                            // If we're only repeating a certain number of times and have reached that threshold, then we'll stop altogether.
                            if (CurrentRepeatCount > RepeatCount)
                            {
                                IsComplete = true;
                                IsRunning = false;
                                return;
                            }
                            CurrentRepeatCount++;
                        }

                        CurrentFrameIntervalIndex = RepeatToIndex;
                        StartGameTime = CurrentGameTime;
                        isRepeating = true;
                        return;
                    }
                }
                else
                {
                    CurrentFrameIntervalIndex++;
                }
            }
            else
            {
                // No repeat, so always show the last frame forever.
                CurrentFrameIntervalIndex = (FrameIntervals.Length - 1);
                IsComplete = true;
                IsRunning = false;
                return;
            }
        }
    }
}
