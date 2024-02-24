using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Ingenia.Engine;

namespace Ingenia.Data
{
    /// <summary>
    /// The base class for all data objects. All data objects must inherit this class.
    /// </summary>
    [Serializable]
    public class Base
    {
        /// <summary>
        /// The object name of this base object.
        /// </summary>
        public string Object { get; set; }

        /// <summary>
        /// The properties of the data object.
        /// </summary>
        public Dictionary<string, DataProperty> Properties = new Dictionary<string, DataProperty>();

        /// <summary>
        /// List of data elements. Always sorted by index and not by keys.
        /// Elements are always checked by the engine.
        /// </summary>
        public List<DataElement> Elements = new List<DataElement>();

        /// <summary>
        /// The required properties for this data object.
        /// </summary>
        public List<string> Required = new List<string>();

        /// <summary>
        /// The key for this data object.
        /// </summary>
        public string Key { get { return Properties["key"].Value; } }

        /// <summary>
        /// Constructs a base data object.
        /// </summary>
        public Base() { 
            // Add the required property set
            Required.Add("key");
        }

        /// <summary>
        /// True if the object meets the requirements. False if not.
        /// </summary>
        public virtual bool Valid
        {
            get
            {
                // Get the list of active keys
                List<string> keys = Properties.Keys.ToList();

                // Check the required keys and make sure each
                // key is present in the properties of the object
                foreach (string key in Required)
                    if (!keys.Contains(key))
                        return false;

                // Return true if all required keys are present
                return true;
            }
        }

        /// <summary>
        /// Returns the error message for this base object.
        /// Will be an empty string if no errors are found.
        /// </summary>
        public virtual string ErrorMessage(string name)
        {
            // Return empty string if valid
            if (Valid) return String.Empty;

            // Set the error string
            string message = "Required properties missing in data object '" + name + "'.\n\n\t";

            // Get the list of active keys
            List<string> keys = Properties.Keys.ToList();

            // Check the required keys and make sure each
            // key is present in the properties of the object
            foreach (string key in Required)
                if (!keys.Contains(key))
                    message += key + "\n\t";

            // Return true if all required keys are present
            return message;
        }

        public override string ToString()
        {
            string temp = "Base Data Object\n\n";

            temp += "PROPERTIES\n\n";

            foreach (KeyValuePair<string, DataProperty> pair in Properties)
                temp += "(" + pair.Value.Type.ToString() + ") " + pair.Key + " => " + pair.Value.Value + "\n";

            temp += "\nELEMENTS\n\n";

            foreach (DataElement element in Elements)
                temp += ElementString(element);

            temp += "\nEND OF OBJECT";

            return temp;
        }

        public string ElementString(DataElement element, string prefix = "")
        {
            string temp = prefix + "Element Name: " + element.Name + "\n";

            foreach (KeyValuePair<string, DataProperty> pair in element.Properties)
                temp += prefix + "\t(" + pair.Value.Type.ToString() + ") " + pair.Key + ":" + pair.Value.Value + "\n";

            foreach(DataElement elem in element.Elements)
                temp += ElementString(elem, prefix + "\t");

            return temp;
        }
    }

    /// <summary>
    /// The class that contains data property information.
    /// </summary>
    [Serializable]
    public class DataProperty
    {
        public Type Type;
        public string Value;

        public DataProperty(Type type, string value)
        {
            Type = type; Value = value;
            if (Type == typeof(Single))
            {
                float single;
                if (!float.TryParse(Value, out single))
                    Value = value.Replace('.', ',');
            }
        }

        /// <summary>
        /// Yields this property as a valid integer including script values.
        /// </summary>
        public int Integer
        {
            get
            {
                if (Value.EndsWith("}"))
                    return Convert.ToInt32(Script.Run(Value));
                else
                    return AsInteger;
            }
        }

        /// <summary>
        /// Yields this property as a valid float including script values.
        /// </summary>
        public float Float
        {
            get
            {
                if (Value.EndsWith("}"))
                {
                    if (Script.Run(Value).GetType() == typeof(int))
                        return Convert.ToSingle(Script.Run(Value));
                    return (float)Script.Run(Value);
                }
                else
                    return AsFloat;
            }
        }

        /// <summary>
        /// Yields this property as a valid boolean including script values.
        /// </summary>
        public bool Boolean
        {
            get
            {
                if (Value.EndsWith("}"))
                    return Convert.ToBoolean(Script.Run(Value));
                else
                    return AsBool;
            }
        }
        
        /// <summary>
        /// Yields this property as a valid string including script values.
        /// </summary>
        public string String
        {
            get
            {
                // First attempt regex for scripts
                MatchCollection matches = Regex.Matches(Value, @"{(.+?)}");

                // If sucessful: replace every script occurence in string and return the final value
                string str = Value;
                if (matches.Count > 0)
                {
                    // Iterate the script matches
                    foreach (Match match in matches)
                        foreach (Capture capture in match.Captures)
                            str = str.Replace(capture.Value, Script.Run(capture.Value).ToString());                    

                    // Return the fixed string
                    return str;
                }
                else
                    return Value;
            }
        }

        /// <summary>
        /// The data property value returned as a float.
        /// Will return 0 if the value is not a float.
        /// </summary>
        public float AsFloat
        {
            get
            {
                float single;
                if (float.TryParse(Value, out single))
                    return single;
                return 0f;
            }
        }

        /// <summary>
        /// The data property value returned as an integer.
        /// Will return 0 if the value is not an integer.
        /// </summary>
        public int AsInteger
        {
            get
            {
                int integer;
                if (int.TryParse(Value, out integer))
                    return integer;
                return 0;
            }
        }

        /// <summary>
        /// The data property value returned as a boolean.
        /// Will return false if the value is not a boolean.
        /// </summary>
        public bool AsBool
        {
            get {
                bool check;
                if (bool.TryParse(Value, out check))
                    return check;
                return false;
            }
        }
    }

    /// <summary>
    /// The class that contains data element information.
    /// </summary>
    [Serializable]
    public class DataElement
    {
        public string Name;
        public Dictionary<string, DataProperty> Properties = new Dictionary<string, DataProperty>();
        public List<DataElement> Elements = new List<DataElement>();

        public DataElement(string name)
        {
            Name = name;
        }
    }
}
