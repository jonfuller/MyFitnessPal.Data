namespace MyFitnessPal.Data
{
    public class ExitCode
    {
        public static ExitCode Success = new ExitCode(0, "Success");
        public static ExitCode GeneralError = new ExitCode(-1, "unspecified error");
        public static ExitCode BadLogin = new ExitCode(1, "unable to login");

        private ExitCode(int value, string message)
        {
            Value = value;
            Message = message;
        }

        public int Value { get; }
        public string Message { get; }
    }
}