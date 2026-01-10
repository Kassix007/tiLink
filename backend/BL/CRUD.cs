using System.IO;

namespace backend.BL
{
    public class CRUD
    {
        private readonly string _filePath;
        private static readonly object _lock = new();

        public CRUD (string filePath)
        {
            _filePath = filePath;
        }

        // CREATE (Append)
        public void Create(string csvLine)
        {
            lock (_lock)
            {
                using StreamWriter writer = new StreamWriter(_filePath, append: true);
                writer.WriteLine(csvLine);
            }
        }

        // READ ALL
        public List<string[]> ReadAll(bool skipHeader = true)
        {
            lock (_lock)
            {
                var lines = File.ReadAllLines(_filePath);
                return lines
                    .Skip(skipHeader ? 1 : 0)
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .Select(l => l.Split(','))
                    .ToList();
            }
        }

        // UPDATE (by column index)
        public bool Update(int keyColumnIndex, string keyValue, string newCsvLine)
        {
            lock (_lock)
            {
                var lines = File.ReadAllLines(_filePath).ToList();

                for (int i = 1; i < lines.Count; i++)
                {
                    var cols = lines[i].Split(',');
                    if (cols[keyColumnIndex] == keyValue)
                    {
                        lines[i] = newCsvLine;
                        File.WriteAllLines(_filePath, lines);
                        return true;
                    }
                }
                return false;
            }
        }

        // DELETE (by column index)
        public bool Delete(int keyColumnIndex, string keyValue)
        {
            lock (_lock)
            {
                var lines = File.ReadAllLines(_filePath).ToList();
                int originalCount = lines.Count;

                lines = lines
                    .Where((line, index) =>
                        index == 0 || line.Split(',')[keyColumnIndex] != keyValue)
                    .ToList();

                if (lines.Count != originalCount)
                {
                    File.WriteAllLines(_filePath, lines);
                    return true;
                }
                return false;
            }
        }
    }
}
