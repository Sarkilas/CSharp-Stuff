using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Ingenia.Data;
using Ingenia.Engine;
using System.Windows.Forms;

namespace Ingenia.Processing
{
    /// <summary>
    /// The data reader. This class reads and interprets data streams. 
    /// </summary>
    class DataReader
    {
        /// <summary>
        /// A dictionary containing all data types.
        /// </summary>
        public Dictionary<string, Type> DataTypes = new Dictionary<string, Type>();

        /// <summary>
        /// The line number. Debugging purposes only.
        /// </summary>
        private int Number = 0;

        /// <summary>
        /// Constructs a data reader object. 
        /// This constructor initializes all the data types.
        /// </summary>
        public DataReader()
        {
            DataTypes.Add("bool", typeof(Boolean));
            DataTypes.Add("string", typeof(String));
            DataTypes.Add("float", typeof(Single));
            DataTypes.Add("integer", typeof(Int32));
            DataTypes.Add("keyset", typeof(Base));
            DataTypes.Add("keylist", typeof(String));
            DataTypes.Add("list", typeof(Base));
            DataTypes.Add("chunk", typeof(Base));
            DataTypes.Add("skill", typeof(Base));
            DataTypes.Add("weapon", typeof(Base));
            DataTypes.Add("item", typeof(Base));
            DataTypes.Add("base", typeof(Base));
            DataTypes.Add("scene", typeof(Scene));
            DataTypes.Add("interface", typeof(GameInterface));
            DataTypes.Add("stat", typeof(Base));
            DataTypes.Add("formula", typeof(Base));
            DataTypes.Add("character", typeof(Character));
            DataTypes.Add("unit", typeof(Character));
            DataTypes.Add("player", typeof(Player));
            DataTypes.Add("event", typeof(Event));
            DataTypes.Add("map", typeof(Map));
            DataTypes.Add("layout", typeof(Layout));
        }

        /// <summary>
        /// Gets the type string of a particular data object.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public string Type(MemoryStream stream)
        {
            string type = "none", line;
            try
            {
                // Set up the reader
                StreamReader reader = new StreamReader(stream);

                // Read first line (must be opener tag)
                line = ReadLine(reader.ReadLine());

                // If not an opening tag: throw exception
                if (!IsOpenTag(line))
                    throw new Exception("There is no base object referenced. Must be the first line in a data object.");

                // Match the tag expression
                Match match = Regex.Match(line, @"<(.+?)>");

                // If not found: throw exception
                if (!match.Success)
                    throw new Exception("No tag found on the given line. Incorrect data structure.");

                // Obtain the tag value (always lowercase)
                string tag = match.Value.ToLower().Replace("<", String.Empty);
                tag = tag.Replace(">", String.Empty);
                tag = tag.Replace("/", String.Empty);

                // Set type
                type = tag;
            }
            catch { }

            return type.ToLower();
        }

        /// <summary>
        /// Imports a data object from a stream.
        /// </summary>
        /// <param name="stream">The stream to import from.</param>
        /// <returns>Returns a data object valid for the data object.</returns>
        public Base Import(MemoryStream stream, string key = null)
        {
            // Set up the data object
            Base data = new Base(); string line; Number = 0;
            try
            {
                // Set up the reader
                StreamReader reader = new StreamReader(stream);

                // Read first line (must be opener tag)
                line = ReadLine(reader.ReadLine());

                // If not an opening tag: throw exception
                if (!IsOpenTag(line))
                    throw new Exception("There is no base object referenced. Must be the first line in a data object.");

                // If list: process differently
                if (Tag(line) == "LIST" || Tag(line) == "KEYLIST")
                {
                    // Create new base object (now list)
                    data = new Base();
                    data.Object = "list";

                    // Read a line
                    line = ReadLine(reader.ReadLine());

                    // This line has to be a string and named key
                    if (!(Tag(line).ToLower() == "string") || !(PropertyName(line).ToLower() == "key"))
                        throw new Exception("First property in a list object must be a string key.");

                    // Add the key property
                    data.Properties.Add(PropertyName(line),
                        new DataProperty(TagType(line), PropertyValue(line)));

                    // Read a line
                    line = ReadLine(reader.ReadLine());

                    // If not items opening: throw exception
                    if (!(IsOpenTag(line)) || !(Tag(line).ToLower() == "items"))
                        throw new Exception("Items element list not found by the reader.");

                    // Set up an integer
                    int index = 0;
                    while (true)
                    {
                        // Read a line
                        line = ReadLine(reader.ReadLine());

                        // If closing: close
                        if (IsCloseTag(line))
                            break;

                        // Add the line data
                        data.Properties.Add(index.ToString(), new DataProperty(typeof(String), line));

                        // Increment the index
                        index++;
                    }
                }
                else
                {
                    // Create the base object
                    data = (Base)Activator.CreateInstance(TagType(line));

                    // Set base object
                    data.Object = Tag(line).ToLower();

                    // Until the end of the file: read lines
                    while (!reader.EndOfStream)
                    {
                        // Read a line
                        line = ReadLine(reader.ReadLine());

                        // If the close tag has the same type as the data: break
                        if (IsCloseTag(line))
                        {
                            if (TagType(line) == data.GetType())
                                break;
                        }

                        // If open tag: process the open tag
                        else if (IsOpenTag(line))
                            ReadElement(line, reader, data);

                        // Add property
                        else
                            data.Properties.Add(PropertyName(line),
                                new DataProperty(TagType(line), PropertyValue(line)));
                    }
                }

                // Close the reader
                reader.Close();

                // Return the data 
                return data;
            }
            catch (Exception exception)
            {
                throw new Exception("An error occured while reading a data object.\n\n" +
                    "Object Key: " + key + "\n\nError on line " + Number + ": " + exception.Message);
            }
        }

        /// <summary>
        /// Reads an element and any sub-elements.
        /// </summary>
        /// <param name="line">The opening line.</param>
        /// <param name="reader">The reader, for further processing.</param>
        /// <param name="data">The data object.</param>
        protected void ReadElement(string line, StreamReader reader, Base data, DataElement element = null)
        {
            // Get tag name
            string tag = char.ToUpper(Tag(line)[0]) + Tag(line).Substring(1);

            // Initialize the data element if null
            if (element == null)
            {
                element = new DataElement(tag);
                data.Elements.Add(element);
            }
            // Add a new element to the existing element
            else
            {
                element.Elements.Add(new DataElement(tag));
                element = element.Elements[element.Elements.Count - 1];
            }

            // Make sure it reads until the tag closes
            while (true)
            {
                // If end of file: throw exception
                if (reader.EndOfStream)
                    throw new Exception("Tag not properly closed. Incorrect data structure.");

                // Read a line
                line = ReadLine(reader.ReadLine());

                // Break if close tag
                if (IsCloseTag(line))
                {
                    if (tag == char.ToUpper(Tag(line)[0]) + Tag(line).Substring(1))
                        break;
                }

                // Read next element if open tag
                else if (IsOpenTag(line))
                    ReadElement(line, reader, data, element);

                // Add the property
                else {
                    element.Properties.Add(PropertyName(line),
                        new DataProperty(TagType(line), PropertyValue(line)));
                }
            }
        }

        /// <summary>
        /// Reads a line but fixes the format for easier processing.
        /// </summary>
        /// <param name="line">The line to fix.</param>
        /// <returns>Returns the fixed line.</returns>
        protected string ReadLine(string line)
        {
            // Increase the line number
            Number++;

            // Remove all leading spaces on the line
            while (line.Substring(0, 1) == " ")
                line = line.Substring(1);

            // Remove all leading tabs
            while (line.Substring(0, 1) == "\t")
                line = line.Substring(1);

            // Remove all spaces at the end of the line
            while (line.Substring(line.Length - 1) == " ")
                line = line.Substring(0, line.Length - 1);

            // Remove all tabs at the end of the line
            while (line.Substring(line.Length - 1) == "\t")
                line = line.Substring(0, line.Length - 1);

            // Return the fixed line
            return line;
        }

        /// <summary>
        /// Returns the type of the tag associated with the given line.
        /// </summary>
        /// <param name="line">The line to locate the tag on.</param>
        /// <returns>Returns the type corresponding to this tag.</returns>
        /// <exception cref="System.Exception">Throws an exception when no valid type is found for the tag, or no tag is found at all.</exception>
        protected Type TagType(string line)
        {
            // Match the tag expression
            Match match = Regex.Match(line, @"<(.+?)>");

            // If not found: throw exception
            if (!match.Success)
                throw new Exception("No tag found on the given line. Incorrect data structure.");

            // Obtain the tag value (always lowercase)
            string tag = match.Value.ToLower().Replace("<", String.Empty);
            tag = tag.Replace(">", String.Empty);
            tag = tag.Replace("/", String.Empty);
            
            // If this data type doesn't bind: throw exception
            if (!DataTypes.ContainsKey(tag))
                return Number == 0 ? typeof(Base) : typeof(String);

            // Return the type
            return DataTypes[tag];
        }

        /// <summary>
        /// Gets the tag as a string without the brackets.
        /// </summary>
        /// <param name="line">The line to find the tag on.</param>
        /// <returns>Returns the tag as a string without the brackets.</returns>
        protected string Tag(string line)
        {
            // Match the tag expression
            Match match = Regex.Match(line, @"<(.+?)>");

            // If not found: throw exception
            if (!match.Success)
                throw new Exception("No tag found on the given line. Incorrect data structure.");

            // Get the value
            string value = match.Value.Replace("<", String.Empty);
            value = value.Replace(">", String.Empty);
            value = value.Replace("/", String.Empty);

            // Obtain the tag value (always lowercase)
            return value;
        }

        /// <summary>
        /// Gets the property name as a string without the brackets.
        /// </summary>
        /// <param name="line">The line to find the tag on.</param>
        /// <returns>Returns the property name as a string.</returns>
        protected string PropertyName(string line)
        {
            // Match the tag expression
            Match match = Regex.Match(line, @">(.+?):");

            // If not found: throw exception
            if (!match.Success)
                throw new Exception("No property name found on the given line. Incorrect data structure.\n\nLine: " + line);

            // Get value
            string value = match.Value.ToLower().Replace(">", String.Empty);
            value = value.Replace(":", String.Empty);

            // Obtain the property name (always upcase)
            return ReadLine(value);
        }

        /// <summary>
        /// Gets the property value as a string without the brackets.
        /// </summary>
        /// <param name="line">The line to find the tag on.</param>
        /// <returns>Returns the property value as a string.</returns>
        protected string PropertyValue(string line)
        {
            // Match the tag expression
            Match match = Regex.Match(line, @":(.+?)$");

            // If not found: throw exception
            if (!match.Success)
                throw new Exception("No property name found on the given line. Incorrect data structure.\n\nLine: " + line);

            // Obtain the property name (always upcase)
            return ReadLine(match.Value.Replace(":", String.Empty));
        }

        /// <summary>
        /// Checks if the tag on the line is an opening tag.
        /// </summary>
        /// <param name="line">The line to check.</param>
        /// <returns>Returns true if an opening tag, false if not.</returns>
        protected bool IsOpenTag(string line)
        {
            return line.Substring(0, 1) == "<" &&
                line.Substring(line.Length - 1) == ">" &&
                line.Substring(0, 2) != "</";
        }

        /// <summary>
        /// Checks if the tag on the line is a closing tag.
        /// </summary>
        /// <param name="line">The line to check.</param>
        /// <returns>Returns true if a closing tag, false if not.</returns>
        protected bool IsCloseTag(string line)
        {
            return line.Substring(0, 2) == "</" &&
                line.Substring(line.Length - 1) == ">";
        }
    }
}
