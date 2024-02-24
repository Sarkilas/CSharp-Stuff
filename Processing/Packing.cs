using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Ingenia.Processing
{
    /// <summary>
    /// Packs and unpacks data packages.
    /// </summary>
    static class Packing
    {
        /// <summary>
        /// Packs a list of files into a package file.
        /// </summary>
        /// <param name="target">The target package file. Full path required.</param>
        /// <param name="contents">The directory data objects to pack.</param>
        /// <returns>Returns the exception message of an error occured. Otherwise returns null.</returns>
        public static string Pack(string target, DirectoryData contents)
        {
            // Make sure the sequence can escape
            try
            {
                // Create the filestream
                FileStream stream = File.Open(target, FileMode.Create);

                // Set up writer
                BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF7);

                // Write the directory data
                PackDirectory(writer, contents);

                // Write end of file
                writer.Write((byte)PackValue.EndOfPackage);

                // Close the stream
                stream.Close();
            }
            catch (Exception e)
            {
                return e.Message;
            }

            // Return null (no error occured)
            return null;
        }

        /// <summary>
        /// Unpacks a package file and places the content files within a package.
        /// </summary>
        /// <param name="source">The source package to unpack.</param>
        /// <param name="target">The target folder to unpack to. This path should always end with a slash.</param>
        /// <returns>Returns the exception message if an error occured. Returns null if no errors.</returns>
        public static string Unpack(string source, string target)
        {
            // Make sure the sequence can escape
            byte value = (byte)PackValue.EndOfPackage; string message = null;
            try
            {
                // Set working directory
                string cd = target;

                // Open the stream
                FileStream stream = File.OpenRead(source);

                // Create the binary reader
                BinaryReader reader = new BinaryReader(stream, Encoding.UTF7);

                // Read all the data
                value = reader.ReadByte();

                // Read the directory (and sub-directories)
                ReadDirectory(reader, cd);

                // Close the stream
                stream.Close();
            }
            catch (Exception e)
            {
                return e.Message;
            }

            // Return message (null of no error occured)
            return message;
        }

        /// <summary>
        /// Loads a list of byte arrays from a package file with a key representing each file.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Dictionary<string, byte[]> Load(string source)
        {
            // Set up the dictionary
            Dictionary<string, byte[]> data = new Dictionary<string, byte[]>();

            // Make sure the sequence can escape
            byte value = (byte)PackValue.EndOfPackage; 
            try
            {
                // Open the stream
                FileStream stream = File.OpenRead(source);

                // Create the binary reader
                BinaryReader reader = new BinaryReader(stream, Encoding.UTF7);

                // Read all the data
                value = reader.ReadByte();

                // Read the directory (and sub-directories)
                LoadDirectory(reader, data);

                // Close the stream
                stream.Close();
            }
            catch (Exception e)
            {
                throw e;
            }

            return data;
        }

        /// <summary>
        /// Writes the data of a DirectoryData object into a package file.
        /// </summary>
        /// <param name="writer">The BinaryWriter used for this package.</param>
        /// <param name="data">The DirectoryData to pack into the file.</param>
        /// <param name="value">The directory value, be it root or sub. Default root.</param>
        private static void PackDirectory(BinaryWriter writer, DirectoryData data, PackValue value = PackValue.RootDirectory)
        {
            // Write directory flag
            writer.Write((byte)value);

            // Write directory name
            writer.Write(data.Name);

            // Write file count
            writer.Write(data.Files.Count);

            // Go through all files in this directory
            if (data.Files.Count > 0)
                foreach (string file in data.Files)
                {
                    // Get file information
                    FileInfo info = new FileInfo(file);

                    // Write the enum to make sure its properly read
                    writer.Write((byte)PackValue.File);

                    // Write the file size
                    writer.Write(info.Length);

                    // Write the file name
                    writer.Write(info.Name);

                    // Write the bytes
                    writer.Write(File.ReadAllBytes(file));
                }

            // Check all sub-directories
            foreach (DirectoryData sub in data.Directories)
                PackDirectory(writer, sub, PackValue.SubDirectory);

            // Write up level flag if within a sub-directory
            if (value == PackValue.SubDirectory)
                writer.Write((byte)PackValue.UpLevel);
        }

        /// <summary>
        /// Reads the data of a DirectoryData object from a package file.
        /// </summary>
        /// <param name="reader">The BinaryReader used for this package.</param>
        /// <param name="cd">The current directory string.</param>
        private static void ReadDirectory(BinaryReader reader, string cd)
        {
            // Set up the value
            byte value;

            // Read the directory name
            string name = reader.ReadString();
            if (name != "ROOT")
                cd += name + "\\";

            // Read file count
            int count = reader.ReadInt32();

            // Create the directory
            Directory.CreateDirectory(cd);

            // Go through all files in this directory
            for (int i = 0; i < count; i++)
            {
                // Make sure we are reading a file
                value = reader.ReadByte();
                // if not: throw exception
                if (value != (byte)PackValue.File)
                    throw new Exception("Incorrect package file structure.");

                // Read the file length
                long size = reader.ReadInt64();

                // Read the name
                string filename = reader.ReadString();

                // Read the file byte array
                byte[] bytes = reader.ReadBytes((int)size);

                // Create the file steam
                File.WriteAllBytes(cd + filename, bytes);
            }

            // Read the next byte
            value = reader.ReadByte();

            // Check all sub-directories
            if (value == (byte)PackValue.SubDirectory)
                ReadDirectory(reader, cd);

            // If up level: go up a level in the current directory
            if (value == (byte)PackValue.UpLevel)
            {
                cd = Path.GetDirectoryName(cd);
                cd = cd.Substring(0, cd.LastIndexOf('\\') + 1);
                // Read the next byte until its no longer uplevel
                while (value == (byte)PackValue.UpLevel)
                {
                    value = reader.ReadByte();
                    if (value == (byte)PackValue.UpLevel)
                    {
                        cd = Path.GetDirectoryName(cd);
                        cd = cd.Substring(0, cd.LastIndexOf('\\') + 1);
                    }
                }
                if (value != (byte)PackValue.EndOfPackage)
                    ReadDirectory(reader, cd);
            }
        }

        private static void LoadDirectory(BinaryReader reader, Dictionary<string, byte[]> data)
        {
            // Set up the value
            byte value;

            // Read the directory name
            string name = reader.ReadString();

            // Read file count
            int count = reader.ReadInt32();

            // Go through all files in this directory
            for (int i = 0; i < count; i++)
            {
                // Make sure we are reading a file
                value = reader.ReadByte();
                // if not: throw exception
                if (value != (byte)PackValue.File)
                    throw new Exception("Incorrect package file structure.");

                // Read the file length
                long size = reader.ReadInt64();

                // Read the name
                string filename = reader.ReadString();

                // Get the key
                string key = filename.ToLower();

                // Fix key
                if (key.Substring(key.Length - 3) == "dat")
                    key = key.Substring(0, key.LastIndexOf('.'));

                // Read the file byte array
                byte[] bytes = reader.ReadBytes((int)size);

                // Create the stream
                data.Add(key, bytes);
            }

            // Read the next byte
            value = reader.ReadByte();

            // Check all sub-directories
            if (value == (byte)PackValue.SubDirectory)
                LoadDirectory(reader, data);

            // If up level: go up a level in the current directory
            if (value == (byte)PackValue.UpLevel)
            {
                // Read the next byte until its no longer uplevel
                while (value == (byte)PackValue.UpLevel)
                    value = reader.ReadByte();
                if (value != (byte)PackValue.EndOfPackage)
                    LoadDirectory(reader, data);
            }
        }
    }

    /// <summary>
    /// Pack value enums. Important for reading the packages correctly.
    /// </summary>
    enum PackValue
    {
        File, RootDirectory, SubDirectory, UpLevel, NewRoot, EndOfPackage
    }

    /// <summary>
    /// Holds directory data including files and sub-directories.
    /// </summary>
    class DirectoryData
    {
        public string Name = "";
        public List<DirectoryData> Directories = new List<DirectoryData>();
        public List<string> Files = new List<string>();
    }
}
