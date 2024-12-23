namespace ServiPrueba.Shared
{
    // Result Pattern
    public class Result<T>
    {
        public bool _success { get; }
        public T? _value { get; }
        public string? _errorMessage { get; }

        private Result(bool success, T? value, string? errorMessage)
        {
            _success = success;
            _value = value;
            _errorMessage = errorMessage;
        }

        public static Result<T> Success(T value) => new(true, value, null);
        public static Result<T> Failure(string errorMessage) => new(false, default, errorMessage);
    }
}
