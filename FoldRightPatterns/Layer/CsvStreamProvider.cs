using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RightFoldPatterns.Layer
{
    public class CsvStreamProvider
    {
        public IEnumerable<string> LoadLines(string path)
        {
            using var reader = new StreamReader(path);
            string? line;
            while ((line = reader.ReadLine()) != null)
                yield return line;
        }
    }

}
