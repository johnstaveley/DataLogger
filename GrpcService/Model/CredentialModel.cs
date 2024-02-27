using System.ComponentModel.DataAnnotations;

namespace GrpcService.Model;

public class CredentialModel
{
    [Required]
    public string? UserName { get; set; }
    [Required]
    public string? Password { get; set; }
}
