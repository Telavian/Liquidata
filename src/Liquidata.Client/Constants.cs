namespace Liquidata.Client;

public class Constants
{
    public class Browser
    {
        public const string AllProjectsKey = "AllProjects";
        public static string ProjectKey(Guid id) => $"Project_{id}";
    }    
}
