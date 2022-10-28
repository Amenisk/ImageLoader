using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using static System.Net.Mime.MediaTypeNames;

namespace ImageLoader.Data
{
    public class FileSystemService
    {
        public void UploadImageToDb(string path)
        {
            var client = new MongoClient("mongodb://localhost");
            var database = client.GetDatabase("Images");
            var gridFS = new GridFSBucket(database);
            string[] paths = Directory.GetFiles(path);

            foreach (var p in paths)
            {
                using (FileStream fs = new FileStream(p, FileMode.Open))
                {
                    gridFS.UploadFromStream(GetNameFile(p), fs);
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

        private string GetNameFile(string path)
        {
            string[] partPath = path.Split('\\');

            return partPath[partPath.Length - 1];
        }
    }
}
