﻿using System;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/* RealChute was made by Christophe Savard (stupid_chris) and is licensed under CC-BY-NC-SA. You can remix, modify and
 * redistribute the work, but you must give attribution to the original author (me) and you cannot sell your derivatives.
 * For more information contact me on the forum. */

namespace RealChute
{
    public static class RCUtils
    {
        #region Constants
        /// <summary>
        /// Transforms from gees to m/s²
        /// </summary>
        public const double geeToAcc = 9.80665d;

        /// <summary>
        /// URL of the RealChute settings config from the GameData folder
        /// </summary>
        public const string localSettingsURL = "GameData/RealChute/RealChute_Settings.cfg";

        /// <summary>
        /// URL of the RealChute PluginData folder from the GameData folder
        /// </summary>
        public const string localPluginDataURL = "GameData/RealChute/Plugins/PluginData";

        /// <summary>
        /// DeploymentStates with their string equivalent
        /// </summary>
        public static readonly Dictionary<DeploymentStates, string> states = new Dictionary<DeploymentStates, string>(5)
        {
            #region States
            { DeploymentStates.STOWED, "STOWED" },
            { DeploymentStates.PREDEPLOYED, "PREDEPLOYED" },
            { DeploymentStates.LOWDEPLOYED, "LOWDEPLOYED" },
            { DeploymentStates.DEPLOYED, "DEPLOYED" },
            { DeploymentStates.CUT, "CUT" }
           #endregion
        };
        #endregion

        #region Arrays
        /// <summary>
        /// Represents the time suffixes
        /// </summary>
        public static readonly char[] timeSuffixes = { 's', 'm' };

        /// <summary>
        /// Represent the types of parachutes
        /// </summary>
        public static readonly string[] types = { "Main", "Drogue", "Drag" };
        #endregion

        #region Propreties
        /// <summary>
        /// String URL to the RealChute settings config
        /// </summary>
        public static string settingsURL
        {
            get { return Path.Combine(KSPUtil.ApplicationRootPath, localSettingsURL); }
        }

        /// <summary>
        /// Returns the RealChute PluginData folder
        /// </summary>
        public static string pluginDataURL
        {
            get { return Path.Combine(KSPUtil.ApplicationRootPath, localPluginDataURL); }
        }

        private static GUIStyle _redLabel = null;
        /// <summary>
        /// A red KSP label for ProceduralChute
        /// </summary>
        public static GUIStyle redLabel
        {
            get
            {
                if (_redLabel == null)
                {
                    GUIStyle style = new GUIStyle(HighLogic.Skin.label);
                    style.normal.textColor = XKCDColors.Red;
                    style.hover.textColor = XKCDColors.Red;
                    _redLabel = style;
                }
                return _redLabel;
            }
        }

        private static GUIStyle _boldLabel = null;
        /// <summary>
        /// A bold KSP style label for RealChute GUI
        /// </summary>
        public static GUIStyle boldLabel
        {
            get
            {
                if (_boldLabel == null)
                {
                    GUIStyle style = new GUIStyle(HighLogic.Skin.label);
                    style.fontStyle = FontStyle.Bold;
                    _boldLabel = style;
                }
                return _boldLabel;
            }
        }

        /// <summary>
        /// Gets the current version of the assembly
        /// </summary>
        public static string assemblyVersion
        {
            get
            {
                System.Version version = new System.Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion);
                if (version.Revision == 0)
                {
                    if (version.Build == 0) { return "v" + version.ToString(2); }
                    return "v" + version.ToString(3);
                }
                return "v" + version.ToString();
            }
        }

        /// <summary>
        /// Returns if FAR is currently loaded in the game
        /// </summary>
        public static bool FARLoaded
        {
            get { return AssemblyLoader.loadedAssemblies.Any(a => a.dllName == "FerramAerospaceResearch"); }
        }

        /// <summary>
        /// If the FAR detection is disabled
        /// </summary>
        public static bool disabled = false;

        private static MethodInfo _densityMethod = null;
        /// <summary>
        /// A delegate to the FAR GetCurrentDensity method
        /// </summary>
        public static MethodInfo densityMethod
        {
            get
            {
                if (_densityMethod == null)
                {
                    _densityMethod = AssemblyLoader.loadedAssemblies.FirstOrDefault(a => a.dllName == "FerramAerospaceResearch").assembly
                        .GetTypes().Single(t => t.Name == "FARAeroUtil").GetMethods().Where(m => m.IsPublic && m.IsStatic)
                        .Where(m => m.ReturnType == typeof(double) && m.Name == "GetCurrentDensity" && m.GetParameters().Length == 2)
                        .Single(m => m.GetParameters()[0].ParameterType == typeof(CelestialBody) && m.GetParameters()[1].ParameterType == typeof(double));
                }
                return _densityMethod;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns true if the time is parseable, returns false otherwise
        /// </summary>
        /// <param name="text">Time value to parse</param>
        public static bool CanParseTime(string text)
        {
            float f = 0;
            return TryParseTime(text, ref f);
        }

        /// <summary>
        /// Parse a time value
        /// </summary>
        /// <param name="text">String to parse</param>
        public static float ParseTime(string text)
        {
            float result = 0;
            TryParseTime(text, ref result);
            return result;
        }

        /// <summary>
        /// Tries to parse a float, taking into account the last character as a time indicator. If none, seconds are assumed.
        /// </summary>
        /// <param name="text">Time value to parse</param>
        /// <param name="result">Value to store the result in</param>
        public static bool TryParseTime(string text, ref float result)
        {
            if (string.IsNullOrEmpty(text)) { return false; }
            float multiplier = 1, test = 0;
            char indicator = text[text.Length - 1];
            if (timeSuffixes.Contains(indicator))
            {
                text = text.Remove(text.Length - 1);
                if (indicator == 'm') { multiplier = 60; }
            }
            if (float.TryParse(text, out test))
            {
                result = test * multiplier;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the value is parsable without actually parsing it
        /// </summary>
        /// <param name="text">String to parse</param>
        public static bool CanParse(string text)
        {
            float f = 0;
            return TryParse(text, ref f);
        }

        /// <summary>
        /// Tries parsing a float from text. IF it fails, the ref value is left unchanged.
        /// </summary>
        /// <param name="text">String to parse</param>
        /// <param name="result">Value to store the result in</param>
        public static bool TryParse(string text, ref float result)
        {
            if (string.IsNullOrEmpty(text)) { return false; }
            float f = 0;
            if (float.TryParse(text, out f))
            {
                result = f;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the spares can be parsed
        /// </summary>
        /// <param name="text">Value to parse</param>
        public static bool CanParseWithEmpty(string text)
        {
            if (string.IsNullOrEmpty(text)) { return true; }
            float test;
            return float.TryParse(text, out test);
        }

        /// <summary>
        /// Parse the string and returns -1 if it's empty
        /// </summary>
        /// <param name="text">String to parse</param>
        public static float ParseWithEmpty(string text)
        {
            if (string.IsNullOrEmpty(text)) { return -1; }
            return float.Parse(text);
        }

        /// <summary>
        /// Parses the spare chutes. If string is empty, returns true and value becomes -1.
        /// </summary>
        /// <param name="text">Value to parse</param>
        /// <param name="result">Value to store the result in</param>
        public static bool TryParseWithEmpty(string text, ref float result)
        {
            if (string.IsNullOrEmpty(text))
            {
                result = -1;
                return true;
            }
            float test;
            if (float.TryParse(text, out test))
            {
                result = test;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the array of values contained within a string
        /// </summary>
        /// <param name="text">Array to parse</param>
        public static string[] ParseArray(string text)
        {
            return text.Split(',').Select(s => s.Trim()).ToArray();
        }

        /// <summary>
        /// Returns true if the string can be parsed and stores it into the ref value.
        /// </summary>
        /// <param name="text">String to parse</param>
        /// <param name="result">Value to store the result in</param>
        public static bool TryParseVector3(string text, ref Vector3 result)
        {
            if (string.IsNullOrEmpty(text)) { return false; }
            string[] splits = ParseArray(text);
            if (splits.Length != 3) { return false; }
            float x, y, z;
            if (!float.TryParse(splits[0], out x)) { return false; }
            if (!float.TryParse(splits[1], out y)) { return false; }
            if (!float.TryParse(splits[2], out z)) { return false; }
            result = new Vector3(x, y, z);
            return true;
        }

        /// <summary>
        /// Returns the area of a circular parachute of the given diameter
        /// </summary>
        /// <param name="diameter">Diameter of the chute</param>
        public static float GetArea(float diameter)
        {
            return (diameter * diameter * Mathf.PI) / 4f;
        }

        /// <summary>
        /// Returns the diameter of a given area
        /// </summary>
        /// <param name="area">Area to determine dthe diameter of</param>
        public static float GetDiameter(float area)
        {
            return Mathf.Sqrt(area / Mathf.PI) * 2f;
        }

        /// <summary>
        /// Rounds the float to the closets half
        /// </summary>
        /// <param name="f">Number to round</param>
        public static float Round(float f)
        {
            string[] splits = f.ToString().Split('.').Select(s => s.Trim()).ToArray();
            if (splits.Length != 2) { return f; }
            float round = 0, decimals = float.Parse("0." + splits[1]);
            if (decimals >= 0.25f && decimals < 0.75) { round = 0.5f; }
            else if (decimals >= 0.75) { round = 1; }
            return Mathf.Max(float.Parse(splits[0]) + round, 0.5f);
        }

        /// <summary>
        /// Checks if the value is within the given range
        /// </summary>
        /// <param name="f">Value to check</param>
        /// <param name="min">Bottom of the range to check</param>
        /// <param name="max">Top of the range to check</param>
        public static bool CheckRange(float f, float min, float max)
        {
            return f <= max && f >= min;
        }

        /// <summary>
        /// Transform the given time value in seconds to minutes and seconds
        /// </summary>
        /// <param name="time">Time value to transform</param>
        public static string ToMinutesSeconds(float time)
        {
            float minutes = 0, seconds = time;
            while (seconds >= 60)
            {
                seconds -= 60;
                minutes++;
            }
            return String.Concat(minutes, "m ", seconds.ToString("0.0"), "s");
        }

        /// <summary>
        /// Returns true if the number is a whole number (no decimals)
        /// </summary>
        /// <param name="f">Float to check</param>
        public static bool IsWholeNumber(float f)
        {
            return !f.ToString().Contains('.');
        }

        /// <summary>
        /// Removes any excess amount of "(Clone)" bits from part names
        /// </summary>
        /// <param name="part">Part to fix</param>
        public static void RemoveClone(Part part)
        {
            part.name = part.partInfo.name + "(Clone)";
        }

        /// <summary>
        /// Returns a simplified string for the chute number
        /// </summary>
        /// <param name="id">ID of the parachute</param>
        public static string ParachuteNumber(int id)
        {
            switch (id)
            {
                case 0:
                    return "Main chute:";
                case 1:
                    return "Secondary chute:";
                default:
                    return String.Format("Chute #{0}:", id + 1);
            }
        }
        #endregion
    }
}
