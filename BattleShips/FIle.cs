using System;
using System.IO;
using System.Runtime.Serialization;

namespace BattleShips
{
    /// <summary>Serialiser for writing an object to a file</summary>
    public class Serialiser<T>
    {
        public readonly string Filename;
        public readonly DataContractSerializer Ser;

        /// <summary>Create a new serialiser which writes to <c>filename</c></summary>
        public Serialiser(string filename)
        {
            Filename = filename;
            Ser = new DataContractSerializer(typeof(T));
        }

        /// <summary>Write <c>obj</c> to the file</summary>
        public void SerialiseObject(T obj)
        {
            using (FileStream writer = new FileStream(Filename, FileMode.Create))
            {
                Ser.WriteObject(writer, obj);
            }
        }

        /// <summary>Check if the file exists</summary>
        public bool FileExists() => File.Exists(Filename);

        /// <summary>Load an object from the file</summary>
        public T LoadObject()
        {
            using (FileStream writer = new FileStream(Filename, FileMode.Open))
            {
                T? obj = (T?)Ser.ReadObject(writer);
                if (ReferenceEquals(obj, null))
                    throw new NullReferenceException();
                return obj;
            }
        }

        /// <summary>Delete the file</summary>
        public void Clear()
        {
            File.Delete(Filename);
        }
    }
}