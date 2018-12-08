using System;
using System.IO;
using ScanPDF.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(SaveiOS))]
namespace ScanPDF.iOS
{
    public class SaveiOS : ISave
    {
        public string Save(MemoryStream fileStream)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filepath = Path.Combine(path, "SavedDocument.pdf");

            FileStream outputFileStream = File.Open(filepath, FileMode.Create);
            fileStream.Position = 0;
            fileStream.CopyTo(outputFileStream);
            outputFileStream.Close();
            return filepath;
        }
    }
}
