using LogAnalyzer.Lib;

namespace Analyzer
{
    public static class App
    {
        public static async Task Run()
        {
            Analyze analyzer = new Analyze();
            Console.WriteLine("********* FILE LOG ANALYZER ***********");
            Console.WriteLine("******** SELECT OPERATIONS **********");
            Console.WriteLine(
                "1.  Counts number of unique errors per log files \n" +
                "2.  Counts number of duplicated errors per log files \n" +
                "3.  Delete archive from a period \n" +
                "4.  Archive logs from a period \n" +
                "5.  Exit \n" 
                );
            
            char choice = Console.ReadLine().ToCharArray()[0];
            

            if(choice == '1')
            {
                Console.Write("Enter file path: ");
                string? filePath = Console.ReadLine();
                Console.WriteLine($"Total number of unique errors: {await analyzer.UniqueErrorCount(filePath)}");
            }
            else if(choice == '2')
            {
                Console.Write("Enter file path: ");
                string? filePath = Console.ReadLine();
                Console.WriteLine($"Total number of duplicate errors: {await analyzer.DuplicateErrorCount(filePath)}");
            }
            else if(choice == '3')
            {
                Console.Write("Enter file path: ");
                string? filePath = Console.ReadLine();

                Console.Write("Enter Date/Period: ");
                string? period = Console.ReadLine();

                await analyzer.DeleteArchive(filePath, period);
            }
            else if(choice == '4')
            {
                Console.Write("Enter number of files: ");
                int? numberOfFiles = int.Parse(Console.ReadLine());
                List<string> paths = new List<string>();

                for(int i = 1; i < numberOfFiles+1; i++)
                {
                    Console.Write($"Enter file path {i}: ");
                    paths.Add(Console.ReadLine());

                }
                var filePath = Console.ReadLine();
                await analyzer.ArchiveLogs(paths);
            }

        }
    }
}
