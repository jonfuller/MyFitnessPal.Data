using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace MyFitnessPal.Data.Utility.Output
{
    public class CsvOutputWriter : IOutputWriter
    {
        private readonly TextWriter _output;

        public CsvOutputWriter(TextWriter output)
        {
            _output = output;
        }
        public void Write<T>(IEnumerable<T> records)
        {
            using (var csv = new CsvWriter(_output, new CsvConfiguration(CultureInfo.InvariantCulture), true))
            {
                csv.WriteRecords(records);
            }
        }
    }
}