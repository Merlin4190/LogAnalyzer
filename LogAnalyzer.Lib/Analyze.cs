namespace LogAnalyzer.Lib
{
    public class Analyze
    {
        public HashSet<string> UniqueRecords { get; set; } = new HashSet<string>();
        public int Count { get; set; }
        public void Search(string path)
        {
            StreamReader sr = new StreamReader(path);

            var line = sr.ReadLine();

            while (line != null)
            {
                line = line.Substring(25);
                UniqueRecords.Add(line);
                Count++;
            }
        }
    }
}