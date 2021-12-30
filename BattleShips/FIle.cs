using System;
using System.IO;
using System.Runtime.Serialization;

namespace BattleShips
{
    public class Serialiser<T>
    {
        public readonly string Filename;
        public readonly DataContractSerializer Ser;

        public Serialiser(string filename)
        {
            Filename = filename;
            Ser = new DataContractSerializer(typeof(T));
        }

        public void SerialiseObject(T obj)
        {
            using (FileStream writer = new FileStream(Filename, FileMode.Create))
            {
                Ser.WriteObject(writer, obj);
            }
        }

        public bool FileExists() => File.Exists(Filename);

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

        public void Clear()
        {
            File.Delete(Filename);
        }
    }
}