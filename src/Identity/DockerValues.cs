namespace Identity;

public static class DockerValues
{
    private const string _LISTENING_PORT = "LISTENING_PORT";
    private const string _KRATOS_PUBLIC_URL = "KRATOS_PUBLIC_URL";
    private const string _KRATOS_ADMIN_URL = "KRATOS_ADMIN_URL";

    public static string PublicUrl =>
        Environment.GetEnvironmentVariable(_KRATOS_PUBLIC_URL) ?? throw new ArgumentException($"{_KRATOS_PUBLIC_URL} not set");
    public static string AdminUrl =>
        Environment.GetEnvironmentVariable(_KRATOS_ADMIN_URL) ?? throw new ArgumentException($"{_KRATOS_ADMIN_URL} not set");
    public static int ListeningPort =>
        int.Parse(Environment.GetEnvironmentVariable(_LISTENING_PORT) ?? throw new ArgumentException($"{_LISTENING_PORT} not set"));
}