using System.Collections.Generic;

namespace MyFitnessPal.Data.Utility.Output
{
    public interface IOutputWriter
    {
        void Write<T>(IEnumerable<T> records);
    }
}