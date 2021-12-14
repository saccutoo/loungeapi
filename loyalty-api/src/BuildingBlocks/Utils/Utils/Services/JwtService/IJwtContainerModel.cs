namespace Utils
{
    using System.Security.Claims;
    public interface IJwtContainerModel
    {
        #region Members
        string Issuer { get; set; }
        string SecretKey { get; set; }
        string SecurityAlgorithm { get; set; }
        int ExpireMinutes { get; set; }
        Claim[] Claims { get; set; }
        #endregion
    }
}
