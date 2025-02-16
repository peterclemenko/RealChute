﻿using RealChute.Extensions;

/* RealChute was made by Christophe Savard (stupid_chris) and is licensed under CC-BY-NC-SA. You can remix, modify and
 * redistribute the work, but you must give attribution to the original author (me) and you cannot sell your derivatives.
 * For more information contact me on the forum. */

namespace RealChute.Libraries
{
    public class MaterialDefinition
    {
        #region Propreties
        private string _name = string.Empty;
        /// <summary>
        /// Name of the material
        /// </summary>
        public string name
        {
            get { return this._name; }
        }

        private string _description = "We don't know much about this, it might as well be made of fishnets";
        /// <summary>
        /// The description of this material
        /// </summary>
        public string description
        {
            get { return this._description; }
        }

        private float _areaDensity = 0.00005f;
        /// <summary>
        /// Area density of this material
        /// </summary>
        public float areaDensity
        {
            get { return this._areaDensity; }
        }

        private float _dragCoefficient = 1;
        /// <summary>
        /// Drag coefficient of this material
        /// </summary>
        public float dragCoefficient
        {
            get { return this._dragCoefficient; }
        }

        private float _areaCost = 0.075f;
        /// <summary>
        /// Cost of a square meter of this material
        /// </summary>
        public float areaCost
        {
            get { return this._areaCost; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates an empty material definition
        /// </summary>
        public MaterialDefinition() { }

        /// <summary>
        /// Creates a material definition from a config node
        /// </summary>
        /// <param name="node">Node to initiate the material from</param>
        public MaterialDefinition(ConfigNode node)
        {
            node.TryGetValue("name", ref _name);
            node.TryGetValue("description", ref _description);
            node.TryGetValue("areaDensity", ref _areaDensity);
            node.TryGetValue("dragCoefficient", ref _dragCoefficient);
            node.TryGetValue("areaCost", ref _areaCost);
        }
        #endregion
    }
}
