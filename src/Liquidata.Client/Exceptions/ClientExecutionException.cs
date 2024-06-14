namespace Liquidata.Client.Exceptions;

public class ClientExecutionException : Exception
{
    public ClientExecutionException()
        : base("Not supported in client application")
    {
        // Nothing
    }
}
