using Aspose.Zip;
using Aspose.Zip.Saving;
using System.Globalization;

namespace LogAnalyzer.Lib
{
    public class Analyze
    {
        public HashSet<string> UniqueRecords { get; set; } = new HashSet<string>();
        public Dictionary<string, int> errors = new Dictionary<string, int>();
        public int Count { get; set; }
        public void Search(string path)
        {
            StreamReader sr = new StreamReader(path);

            var line = sr.ReadLine();

            while (line != null)
            {
                line = line.Substring(25);
                UniqueRecords.Add(line);
            }
        }

        private async Task GroupErrors(string path)
        {
            StreamReader sr = new StreamReader(path);

            var line = await sr.ReadLineAsync();

            while(line != null)
            {
                if(!string.IsNullOrWhiteSpace(line) && char.IsDigit(line[0]))
                {
                    line = line.Substring(25);
                    if (!errors.ContainsKey(line)) errors.Add(line, 1);
                    else errors[line]++;
                }

                //Read the next line
                line = sr.ReadLine();
            }
        }

        public async Task<int> UniqueErrorCount(string path)
        {
            int count = 0;

            await GroupErrors(path);
            foreach (var error in errors)
            {
                if(error.Value == 1) count++;
            }

            return count;
        }

        public async Task<int> DuplicateErrorCount(string path)
        {
            int count = 0;

            await GroupErrors(path);
            foreach (var error in errors)
            {
                if (error.Value >= 2) count++;
            }

            return count;
        }

        public async Task DeleteArchive(string path, string period)
        {
            string tempFile = Path.GetTempFileName();

            using (var sr = new StreamReader(path))
            using (var sw = new StreamWriter(tempFile))
            {
                string line;

                while ((line = await sr.ReadLineAsync()) != null)
                {
                    line = line.Substring(0,10);
                    if (line != period)
                        await sw.WriteLineAsync(line);
                }
            }

            File.Delete(path);
            File.Move(tempFile, path);
        }

        public async Task ArchiveLogs(IList<string> paths)
        {
            string startDate, endDate, archiveName = string.Empty;

            if (paths.Count > 0)
            {
                IFormatProvider culture = new CultureInfo("en-US", true);
                using (var sr = new StreamReader(paths[0]))
                {
                    string? line = await sr.ReadLineAsync();
                    line = line.Substring(0,10);

                    sr.Close();

                    startDate = ConvertToSpecifiedFormat(line);
                    //startDate = DateTime.ParseExact(line, "dd_M_yyyy", culture).ToString();
                }

                using (var sr = new StreamReader(paths[paths.Count-1]))
                {
                    string? line = await sr.ReadLineAsync();
                    line = line.Substring(0, 10);
                    sr.Close();

                    endDate = ConvertToSpecifiedFormat(line);
                    //endDate = DateTime.ParseExact(line, "dd_M_yyyy", culture).ToString();
                }

                archiveName = String.Join('-', startDate, endDate);
            }

            using (FileStream zipFile = File.Open($"{archiveName}.zip", FileMode.Create))
            {
                // File to be added to archive
                for (int i = 0; i < paths.Count; i++)
                {
                    using (FileStream source1 = File.Open(paths[i], FileMode.Open, FileAccess.Read))
                    {
                        using (var archive = new Archive(new ArchiveEntrySettings()))
                        {
                            // Add file to the archive
                            archive.CreateEntry(paths[i], source1);
                            
                            // ZIP file
                            archive.Save(zipFile);
                        }
                    }
                }
            }
        }

        private string ConvertToSpecifiedFormat(string data)
        {
            var splittedDAta = data.Split('.');
            var newFormat = string.Join('_', splittedDAta);

            return newFormat;
        }
    }
}