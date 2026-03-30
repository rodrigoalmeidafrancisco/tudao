namespace Shared.Commands._Base
{
    public class CommandResult<T>
    {
        public CommandResult()
        {

        }

        public int StatusCode { get; set; }
        public Guid? ErrorId { get; set; } = null;
        public string Path { get; set; } = null;
        public string StackTrace { get; set; } = null;
        public string Message { get; set; } = null;
        public long? Total { get; set; } = null;
        public T Data { get; set; }
    }
}
