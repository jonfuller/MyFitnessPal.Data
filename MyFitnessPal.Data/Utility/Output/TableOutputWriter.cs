using System.Collections.Generic;
using System.IO;
using ConsoleTables;

namespace MyFitnessPal.Data.Utility.Output
{
    public class TableOutputWriter : IOutputWriter
    {
        private readonly TextWriter _output;

        public TableOutputWriter(TextWriter output)
        {
            _output = output;
        }

        public void Write<T>(IEnumerable<T> records)
        {
            ConsoleTable
               .From(records)
               .Configure(o =>
                {
                    o.OutputTo = _output;
                    o.NumberAlignment = Alignment.Right;
                })
               .Write(Format.Alternative);
        }
    }
}