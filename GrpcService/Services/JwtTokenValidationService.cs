﻿using GrpcService.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GrpcService.Services;

public class JwtTokenValidationService
{
    private readonly ILogger<JwtTokenValidationService> _logger;
    /*    private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
    */
    private readonly IConfiguration _config;

    public JwtTokenValidationService(ILogger<JwtTokenValidationService> logger,
        //UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
        IConfiguration config)
    {
        _logger = logger;
        //_userManager = userManager;
        //_signInManager = signInManager;
        _config = config;
    }

    public async Task<TokenModel> GenerateTokenModelAsync(CredentialModel model)
    {
        var acceptableUserName = _config.GetValue<string>("Settings:UserName");
        var acceptablePassword = _config.GetValue<string>("Settings:Password");        
        if (model.UserName != acceptableUserName || model.Password != acceptablePassword)
        {
            return new TokenModel()
            {
                Success = false
            };
        }
        var result = new TokenModel
        {
            Success = true
        };
        // Create the token
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, acceptableUserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, acceptableUserName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _config["Tokens:Issuer"],
            _config["Tokens:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        result.Token = new JwtSecurityTokenHandler().WriteToken(token);
        result.Expiration = token.ValidTo;
        return result;
    }

    /*public async Task<TokenModel> GenerateTokenModelAsync(CredentialModel model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName);
        var result = new TokenModel()
        {
            Success = false
        };

        if (user != null)
        {
            var check = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (check.Succeeded)
            {
                // Create the token
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                  _config["Tokens:Issuer"],
                  _config["Tokens:Audience"],
                  claims,
                  expires: DateTime.Now.AddMinutes(30),
                  signingCredentials: creds);

                result.Token = new JwtSecurityTokenHandler().WriteToken(token);
                result.Expiration = token.ValidTo;
                result.Success = true;
            }
        }
        return result;
    }*/
}
