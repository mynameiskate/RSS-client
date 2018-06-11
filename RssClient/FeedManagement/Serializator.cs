using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace RssClient.FeedManagement
{
    public static class Serializator
    {
        private const string SavePath = @"..\favourites\";
        private const string Extension = ".dat";

        public static bool RemoveFromFavourites(this FeedItem item)
        {
            string path = SavePath + item.HashCode + Extension;
            try
            {
                File.Delete(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string BinarySerialize(this FeedItem item)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                var path = SavePath + item.HashCode + Extension;
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    formatter.Serialize(fs, item);
                    fs.Flush();
                }
                    return path;
            }
            catch
            {
                throw new Exception("Couldn't save item");
            }
        }

        public static FeedItem BinaryDeserialize(string path)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                var item = new FeedItem();
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    item = (FeedItem)formatter.Deserialize(fs);
                }
                    return item;
                }
            catch
            {
                return null;
            }
        }

        public static Hashtable DeserializeList()
        {
            var itemTable = new Hashtable();
            try
            {
                string[] files = Directory.GetFiles(SavePath, $"*{Extension}", SearchOption.TopDirectoryOnly);
                foreach(string path in files)
                {
                    FeedItem item = BinaryDeserialize(path);
                    if (item != null)
                    {
                        itemTable.Add(item.HashCode, item);
                    }
                }
                return itemTable;
            }
            catch
            {
                return itemTable;
            }
        }
    }
}
