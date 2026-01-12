using Microsoft.Extensions.Options;

namespace backend.BL
{
    public class CreateFileStorage
    {
        private readonly string _path;
        //private string filePath = Path.Combine(folderPath, "Test.csv");
        private static readonly object _lock = new();


        //public static readonly string folderPath = @"C:\tiLink\TestFolder\"; //define folder path

        //public static readonly string filePath = Path.Combine(folderPath, "Test.csv"); //define csv file

        public CreateFileStorage(string path) 
        { 
            _path = path;
        }

        public void CreateCsv()
        {
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path.folderPath);

            //File.WriteAllText(_paths.CsvFile, "Test,Test2,Test3" + Environment.NewLine);

            string id = Guid.NewGuid().ToString();

            lock (_lock)
            {

                if (!System.IO.File.Exists(_path.CsvFile))
                {
                    using StreamWriter headerWriter = new StreamWriter(_path.CsvFile, append: false);
                    headerWriter.WriteLine("GUID,Data");
                }

                try
                {
                    using StreamWriter writer = new StreamWriter(filePath, append: true); //write data to file
                    writer.WriteLine($"{id},{csvText}");
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Error");
                }
            }
        }
    }
}
