namespace ClassLibrary1
{
    public interface ILoggerDependency
    {
        string GetCurrentDirectory();
        string GetDirectoryByLoggerName(string loggerName);
        string DefaultLogger { get; }
    }
}