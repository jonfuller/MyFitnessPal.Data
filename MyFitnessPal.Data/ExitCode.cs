namespace MyFitnessPal.Data
{
    public class ExitCode
    {
        public static ExitCode Success = new ExitCode(0, "Success");
        public static ExitCode GeneralError = new ExitCode(-1, "unspecified error");

        private ExitCode(int value, string message)
        {
            Value = value;
            Message = message;
        }

        public int Value { get; }
        public string Message { get; }
    }
}