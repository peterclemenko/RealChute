﻿using System.Collections.Generic;
using System.Linq;

/* RealChute was made by Christophe Savard (stupid_chris) and is licensed under CC-BY-NC-SA. You can remix, modify and
 * redistribute the work, but you must give attribution to the original author (me) and you cannot sell your derivatives.
 * For more information contact me on the forum. */

namespace RealChute.Libraries
{
    public class AtmoPlanets
    {
        #region Fetch
        private static AtmoPlanets _fetch = null;
        /// <summary>
        /// Accesses the atmospheric planets library
        /// </summary>
        public static AtmoPlanets fetch
        {
            get
            {
                if (_fetch == null) { _fetch = new AtmoPlanets(); }
                return _fetch;
            }
        }
        #endregion

        #region Propreties
        private Dictionary<CelestialBody, string> _bodies = new Dictionary<CelestialBody, string>();
        /// <summary>
        /// List of all the celestial bodies who have an atmosphere
        /// </summary>
        public Dictionary<CelestialBody, string> bodies
        {
            get { return this._bodies; }
        }

        /// <summary>
        /// Returns the string names of the bodies
        /// </summary>
        public string[] bodyNames
        {
            get { return bodies.Values.ToArray(); }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new instance of PlanetInfo
        /// </summary>
        public AtmoPlanets()
        {
            if (FlightGlobals.Bodies.Count > 0) { _bodies = FlightGlobals.Bodies.Where(b => b.atmosphere && b.pqsController != null).ToDictionary(b => b, b => b.bodyName); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the CelestialBody of the given name
        /// </summary>
        /// <param name="name">Name of the body</param>
        public CelestialBody GetBody(string name)
        {
            return bodies.Single(pair => pair.Value == name).Key;
        }

        /// <summary>
        /// Returns the CelestialBody according to it's index in the dictionary
        /// </summary>
        /// <param name="index">Index of the body</param>
        public CelestialBody GetBody(int index)
        {
            return bodies.Keys.ToArray()[index];
        }

        /// <summary>
        /// Returns the index of the CelestialBody within the dictionary
        /// </summary>
        /// <param name="name">CelestialBody searched for</param>
        /// <returns></returns>
        public int GetPlanetIndex(string name)
        {
            return bodyNames.ToList().IndexOf(name);
        }
        #endregion
    }
}
