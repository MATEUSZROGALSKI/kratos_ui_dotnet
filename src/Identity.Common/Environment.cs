namespace System;

public static partial class EnvironmentVariables
{
    private const string _LISTENING_PORT = "LISTENING_PORT";
    private const string _KRATOS_PUBLIC_URL = "KRATOS_PUBLIC_URL";
    private const string _KRATOS_INTERNAL_URL = "KRATOS_INTERNAL_URL";
    private const string _KRATOS_COOKIE_NAME = "KRATOS_COOKIE_NAME";
    private const string _COOKIE_SCHEMA_NAME = "COOKIE_SCHEMA_NAME";

    public static string PublicUrl =>
        Environment.GetEnvironmentVariable(_KRATOS_PUBLIC_URL) ?? throw new ArgumentException($"{_KRATOS_PUBLIC_URL} not set");
    public static string InternalUrl =>
        Environment.GetEnvironmentVariable(_KRATOS_INTERNAL_URL) ?? throw new ArgumentException($"{_KRATOS_INTERNAL_URL} not set");
    public static string CookieName =>
        Environment.GetEnvironmentVariable(_KRATOS_COOKIE_NAME) ?? throw new ArgumentException($"{_KRATOS_COOKIE_NAME} not set");
    public static string SchemaName =>
        Environment.GetEnvironmentVariable(_COOKIE_SCHEMA_NAME) ?? throw new ArgumentException($"{_COOKIE_SCHEMA_NAME} not set");
    public static int ListeningPort =>
        int.Parse(Environment.GetEnvironmentVariable(_LISTENING_PORT) ?? throw new ArgumentException($"{_LISTENING_PORT} not set"));
}
