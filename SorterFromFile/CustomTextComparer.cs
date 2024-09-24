namespace SorterFromFile
{
    class CustomTextComparer : IComparer<string>
    {
        public int Compare(string? line1, string? line2)
        {
            if (line1 == line2)
            {
                return 0;
            }

            // Split rows
            var str1 = line1!.Split(". ", 2);
            var str2 = line2!.Split(". ", 2);

            // Compare by the string
            int result = string.Compare(str1[1], str2[1], StringComparison.Ordinal);
            
            if (result == 0)
            {
                // Compare by the number
                int num1 = int.Parse(str1[0]);
                int num2 = int.Parse(str2[0]);
                result = num1.CompareTo(num2);
            }

            return result;
        }
    }
}
