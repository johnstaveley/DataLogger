using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GrpcService.Model;

public class DataLogTokenValidationParameters : TokenValidationParameters
{
    public DataLogTokenValidationParameters(IConfiguration config)
    {
        ValidIssuer = config["Tokens:Issuer"];
        ValidAudience = config["Tokens:Audience"];
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Tokens:Key"]));
    }
}
