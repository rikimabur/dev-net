namespace multiple_authentication_schemes
{
    public abstract class JwtPublic
    {
        public string Issuer { get; init; } = string.Empty;
        public string Audience { get; init; } = string.Empty;
        public string Key { get; init; } = string.Empty;
    }
    public sealed class JwtPublicOptions : JwtPublic
    {
        public const string SectionName = "JwtPublic";
    }

    public sealed class JwtInternalOptions : JwtPublic
    {
        public const string SectionName = "JwtInternal";
    }
}
