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
        public HashSet<string> GetLongUrls()
        {
            var longUrls = new HashSet<string>();

            if (!File.Exists(_csvFile))
                return longUrls;

            foreach (var line in File.ReadLines(_csvFile).Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(',');

                if (parts.Length >= 3)
                    longUrls.Add(parts[2]);
            }

            return longUrls;
        }

        //readAllLinksByID - return Link object
        public string? GetLongUrlById(Guid id)
        {
            if (!File.Exists(_csvFile))
                return null;

            using var parser = new TextFieldParser(_csvFile); //to avoid crash on links with commas
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            // Skip header
            if (!parser.EndOfData)
                parser.ReadLine();

            while (!parser.EndOfData)
            {
                var fields = parser.ReadFields();
                if (fields == null || fields.Length < 3)
                    continue;

                // Trim spaces just in case
                var rowIdStr = fields[0].Trim();

                if (Guid.TryParse(rowIdStr, out var rowId) && rowId == id)
                {
                    return fields[2].Trim(); // LongURL
                }
            }

            return null; // not found
        }

        //write create update by ID
    }
}
