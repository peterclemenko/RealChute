﻿using System;

/* RealChute was made by Christophe Savard (stupid_chris) and is licensed under CC-BY-NC-SA. You can remix, modify and
 * redistribute the work, but you must give attribution to the original author (me) and you cannot sell your derivatives.
 * For more information contact me on the forum. */

namespace RealChute
{
    /// <summary>
    /// A generic StopWatch clone which runs on KSP's internal clock
    /// </summary>
    public class WarpWatch
    {
        #region Constants
        /// <summary>
        /// The amound of ticks in a second
        /// </summary>
        protected const long ticksPerSecond = 10000000L;

        /// <summary>
        /// The amount of milliseconds in a second
        /// </summary>
        protected const long millisecondPerSecond = 1000L;
        #endregion

        #region Fields
        /// <summary>
        /// UT Time of the last frame
        /// </summary>
        protected double lastFrame = 0d;

        /// <summary>
        /// Total elapsed time calculated by the watch in seconds
        /// </summary>
        protected double totalSeconds = 0d;
        #endregion

        #region Propreties
        private bool _isRunning = false;
        /// <summary>
        /// If the watch is currently counting down time
        /// </summary>
        public bool isRunning
        {
            get { return this._isRunning; }
            protected set { this._isRunning = value; }
        }

        /// <summary>
        /// The current elapsed time of the watch
        /// </summary>
        public TimeSpan elapsed
        {
            get { return new TimeSpan(this.elapsedTicks); }
        }

        /// <summary>
        /// The amount of milliseconds elapsed to the current watch
        /// </summary>
        public long elapsedMilliseconds
        {
            get
            {
                if (this._isRunning) { UpdateWatch(); }
                return (long)Math.Round(this.totalSeconds * millisecondPerSecond);
            }
        }

        /// <summary>
        /// The amount of ticks elapsed to the current watch
        /// </summary>
        public long elapsedTicks
        {
            get
            {
                if (this._isRunning) { UpdateWatch(); }
                return (long)Math.Round(this.totalSeconds * ticksPerSecond);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new WarpWatch
        /// </summary>
        public WarpWatch() { }
        #endregion

        #region Methods
        /// <summary>
        /// Starts the watch
        /// </summary>
        public void Start()
        {
            this.lastFrame = Planetarium.GetUniversalTime();
            this._isRunning = true;
        }

        /// <summary>
        /// Stops the watch
        /// </summary>
        public void Stop()
        {
            if (this._isRunning) { UpdateWatch(); }
            this._isRunning = false;
        }

        /// <summary>
        /// Resets the watch to zero and starts it
        /// </summary>
        public void Restart()
        {
            this.totalSeconds = 0d;
            this.lastFrame = Planetarium.GetUniversalTime();
            this._isRunning = true;
        }

        /// <summary>
        /// Stops the watch and resets it to zero
        /// </summary>
        public void Reset()
        {
            this.totalSeconds = 0d;
            this.lastFrame = 0d;
            this._isRunning = false;
        }

        /// <summary>
        /// Updates the time on the watch
        /// </summary>
        protected virtual void UpdateWatch()
        {
            double current = Planetarium.GetUniversalTime();
            this.totalSeconds += current - this.lastFrame;
            this.lastFrame = current;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Returns a string representation fo this instance
        /// </summary>
        public override string ToString()
        {
            return elapsed.ToString();
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Creates a new WarpWatch, starts it, and returns the current instance
        /// </summary>
        public static WarpWatch StartNew()
        {
            WarpWatch watch = new WarpWatch();
            watch.Start();
            return watch;
        }
        #endregion
    }
}