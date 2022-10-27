using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using static System.Net.Mime.MediaTypeNames;

namespace ImageLoader.Data
{
    public class FileSystemService
    {
        public void UploadImageToDb()
        {
            var client = new MongoClient("mongodb://localhost");
            var database = client.GetDatabase("Images");
            var gridFS = new GridFSBucket(database);

            for (int i = 1; i < 8; i++)
            {
                using (FileStream fs = new FileStream($"C:/Users/LENOVO/Pictures/Images/{i}.jpg", FileMode.Open))
                {
                    gridFS.UploadFromStream($"{i}.jpg", fs);
                }
            }
        }

        public string DownloadToLocal(string name)
        {
            var client = new MongoClient("mongodb://localhost");
            var database = client.GetDatabase("Images");
            var gridFS = new GridFSBucket(database);
            using (FileStream fs = new FileStream($"{Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/wwwroot/Images/")}{name}", FileMode.OpenOrCreate))
            {
                gridFS.DownloadToStreamByName($"{name}", fs);
            }

            return $"Images/{name}";
        }

        public List<string> GetNamesOfImages()
        {
            var client = new MongoClient("mongodb://localhost");
            var database = client.GetDatabase("Images");
            var collection = database.GetCollection<GridFSFileInfo>("fs.files");
            var names = new List<string>();
            var images = collection.Find(x => x.Filename != null).ToList<GridFSFileInfo>();


            foreach (var image in images)
            {
                names.Add(image.Filename);
            }

            return names;
        }
    }
}
