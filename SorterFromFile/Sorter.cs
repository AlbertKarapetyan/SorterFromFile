namespace SorterFromFile
{
    internal class Sorter(AppSettings appSettings)
    {
        private readonly AppSettings _appSettings = appSettings;
        private long _totalLines = 0;

        public void Sort()
        {
            Console.Write("Initializing...");

            List<string> tempFiles = FileSplit(_appSettings.InputFilePath, _appSettings.TempRowsNumber);

            // Sorting all temp files content
            foreach (var tempFile in tempFiles)
            {
                SortingFileContent(tempFile);
            }

            // Merging
            MergingFiles(tempFiles, _appSettings.OutputFilePath);

            // Remove all temp files
            foreach (var tempFile in tempFiles)
            {
                File.Delete(tempFile);
            }

            Console.WriteLine($"\nFile created: {_appSettings.OutputFilePath}");
        }

        private List<string> FileSplit(string inputFile, int rowsNumber)
        {
            List<string> tempFiles = new List<string>();
            using (StreamReader reader = new StreamReader(inputFile))
            {
                while (!reader.EndOfStream)
                {
                    List<string> lines = new List<string>();
                    for (int i = 0; i < rowsNumber && !reader.EndOfStream; i++)
                    {
                        lines.Add(reader.ReadLine()!);
                        _totalLines++;
                    }

                    string tempFile = Path.GetTempFileName();
                    File.WriteAllLines(tempFile, lines);

                    tempFiles.Add(tempFile);
                }
            }
            return tempFiles;
        }

        private void SortingFileContent(string filePath)
        {
            CustomTextComparer customTextComparer = new CustomTextComparer();

            var lines = File.ReadAllLines(filePath);
            Array.Sort(lines, customTextComparer.Compare);
            File.WriteAllLines(filePath, lines);
        }


        private void MergingFiles(List<string> tempFiles, string outputFile)
        {   
            int lastPercent = 0;
            int rowsCount = 0;
            var filesCount = tempFiles.Count;

            var readers = tempFiles.Select(file => new StreamReader(file)).ToList();
            try
            {
                using (StreamWriter writer = new StreamWriter(outputFile))
                {
                    var queue = new SortedList<string, Queue<StreamReader>>(new CustomTextComparer());

                    foreach (var reader in readers)
                    {
                        if (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine()!;

                            if (!queue.ContainsKey(line))
                            {
                                queue.Add(line, new Queue<StreamReader>());
                            }
                            queue[line].Enqueue(reader);
                        }
                    }

                    // Merging
                    while (queue.Any())
                    {
                        var firstLine = queue.First();
                        writer.WriteLine(firstLine.Key);
                        rowsCount++;

                        var reader = firstLine.Value.Dequeue();

                        // No Lines
                        if (!firstLine.Value.Any())
                        {
                            queue.Remove(firstLine.Key);
                        }

                        if (!reader.EndOfStream)
                        {
                            string nextLine = reader.ReadLine()!;

                            if (!queue.ContainsKey(nextLine))
                            {
                                queue.Add(nextLine, new Queue<StreamReader>());
                            }

                            queue[nextLine].Enqueue(reader);
                        }

                        int percent = (int)(rowsCount * 100 / _totalLines);

                        if (percent > lastPercent)
                        {
                            lastPercent = percent;
                            Console.Write($"\rProgress: {percent}%");
                        }
                    }
                    Console.Write($"\rProgress: 100%");
                }
            }
            finally
            {
                // Close all readers
                foreach (var reader in readers)
                {
                    reader.Dispose();
                }
            }
        }
    }
}
