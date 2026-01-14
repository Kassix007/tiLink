using backend.Models;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;

namespace backend.Service
{
    public class FileManipulation
    {
        private readonly string _csvFile;

        public FileManipulation(IOptions<FilePaths> options)
        {
            _csvFile = options.Value.CsvFile;
        }

        //readAllLinksFromCSV - return HashSet/List of Link
        //write create update by ID
    }
}
