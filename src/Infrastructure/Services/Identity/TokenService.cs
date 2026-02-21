using Application.AppConfigs;
using Application.DTOs.Email;
using Application.Services.Identity;
using Application.Services.MailService;
using Common.Requests.Identity;
using Common.Responses;
using Common.Responses.Wrappers;
using Google.Apis.Auth;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Identity
{
    public class TokenService : ITokenService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly AppConfiguration _appConfiguration;
        private readonly IEmailService _emailService;


        public TokenService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IOptions<AppConfiguration> appConfiguration, IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _appConfiguration = appConfiguration.Value;
            _emailService = emailService;

        }

        public async Task<ResponseWrapper<TokenResponse>> GetTokenAsync(TokenRequest tokenRequest)
        {
            var user  =  await _userManager.FindByEmailAsync(tokenRequest.Email);

            if (user is null)
            {
                return await ResponseWrapper<TokenResponse>.FailAsync("Invalid Credentials.");
            }

            if (!user.IsActive)
            {
                return await ResponseWrapper<TokenResponse>.FailAsync("User not active. Please contact the administrator");
            }

            if (!user.EmailConfirmed)
            {
                return await ResponseWrapper<TokenResponse>.FailAsync("Email not confirmed.");
            }

            var isPaswordValid = await _userManager.CheckPasswordAsync(user, tokenRequest.Password);
            if (!isPaswordValid)
            {
                return await ResponseWrapper<TokenResponse>.FailAsync("Invalid Credentials.");
            }

            if (await _userManager.GetTwoFactorEnabledAsync(user))
            {
                var twoFactorToken = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                await _emailService.SendAsync(new EmailRequest
                {
                    To = user.Email,
                    Subject = "PermissionHub - Login Verification Code",
                    Body = $"<h3>Your Login Verification Code: <strong>{twoFactorToken}</strong></h3><p>This code is valid for a few minutes.</p>"
                });

                return await ResponseWrapper<TokenResponse>.SuccessAsync(new TokenResponse
                {
                    RequiresTwoFactor = true
                }, "Verification code has been sent to your email address.");
            }

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryDate = DateTime.Now.AddDays(7);

            await _userManager.UpdateAsync(user);

            var token = await GenerateJWTAsync(user);

            var response = new TokenResponse
            {
                Token = token,
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryDate
            };

            return await ResponseWrapper<TokenResponse>.SuccessAsync(response);


        }

        public async  Task<ResponseWrapper<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            if (refreshTokenRequest is null)
            {
                return await ResponseWrapper<TokenResponse>.FailAsync("Invalid Client Token.");
            }
            var userPrincipal = GetPrincipalFromExpiredToken(refreshTokenRequest.Token);
            var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user is null)
                return await ResponseWrapper<TokenResponse>.FailAsync("User Not Found.");
            if (user.RefreshToken != refreshTokenRequest.RefreshToken || user.RefreshTokenExpiryDate <= DateTime.Now)
                return await ResponseWrapper<TokenResponse>.FailAsync("Invalid Client Token.");

            var token = GenerateEncrytedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            user.RefreshToken = GenerateRefreshToken();
            await _userManager.UpdateAsync(user);

            var response = new TokenResponse
            {
                Token = token,
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryDate
            };
            return await ResponseWrapper<TokenResponse>.SuccessAsync(response);


        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GenerateJWTAsync(ApplicationUser user)
        {
            var token = GenerateEncrytedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            return token;
        }


        private string GenerateEncrytedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_appConfiguration.TokenExpiryInMinutes),
                signingCredentials: signingCredentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            var encryptedToken = tokenHandler.WriteToken(token);
            return encryptedToken;
        }


        private SigningCredentials GetSigningCredentials()
        {
            var secret = Encoding.UTF8.GetBytes(_appConfiguration.Secret);
            return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        }

        private async Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            var permissionClaims = new List<Claim>();

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
                var currentRole = await _roleManager.FindByNameAsync(role);
                var allPermissionsForCurrentRole = await _roleManager.GetClaimsAsync(currentRole);
                permissionClaims.AddRange(allPermissionsForCurrentRole);
            }

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()), 
                new(JwtRegisteredClaimNames.Email, user.Email),       
                new(JwtRegisteredClaimNames.GivenName, user.FirstName), 
                new(JwtRegisteredClaimNames.FamilyName, user.LastName), 
            }
            .Union(userClaims)
            .Union(roleClaims)
            .Union(permissionClaims);

            return claims;
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfiguration.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken
                || !jwtSecurityToken.Header.Alg
                .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        public async Task<ResponseWrapper<TokenResponse>> GoogleLoginAsync(GoogleLoginRequest request)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {

                Audience = new List<string>() { _appConfiguration.GoogleClientId  }
   
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);

            var info = new UserLoginInfo(request.Provider, payload.Subject, request.Provider);

            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);

                if (user == null)
                {
                    return await ResponseWrapper<TokenResponse>.FailAsync("Bu mail adresi ile sistemde kayıtlı kullanıcı bulunamadı. Lütfen önce kayıt olun.");
                }

                var addLoginResult = await _userManager.AddLoginAsync(user, info);
                if (!addLoginResult.Succeeded)
                {
                    return await ResponseWrapper<TokenResponse>.FailAsync("Google hesabı sistemdeki kullanıcıyla eşleştirilemedi.");
                }
            }
            return await CreateUserExternalAsync(user);
        }

        private async Task<ResponseWrapper<TokenResponse>> CreateUserExternalAsync(ApplicationUser user)
        {
            if (!user.IsActive)
            {
                return await ResponseWrapper<TokenResponse>.FailAsync("Kullanıcı hesabı aktif değil.");
            }

            var tokenString = await GenerateJWTAsync(user);

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryDate = DateTime.Now.AddDays(7); 

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return await ResponseWrapper<TokenResponse>.FailAsync("Token üretilirken bir hata oluştu.");
            }

            var response = new TokenResponse
            {
                Token = tokenString,
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryDate
            };

            return await ResponseWrapper<TokenResponse>.SuccessAsync(response, "Google ile giriş başarılı.");
        }

        public async Task<IResponseWrapper> RevokeRefreshTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return await ResponseWrapper.FailAsync("Kullanıcı bulunamadı.");
            }
            user.RefreshToken = null;
            user.RefreshTokenExpiryDate = null; 

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return await ResponseWrapper.SuccessAsync("Token başarıyla iptal edildi (Çıkış yapıldı).");
            }

            return await ResponseWrapper.FailAsync("Token iptal edilirken bir hata oluştu.");
        }

        public async Task<ResponseWrapper<TokenResponse>> VerifyTwoFactorAsync(string email, string code)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return await ResponseWrapper<TokenResponse>.FailAsync("User not found.");

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", code);
            if (!isValid)
                return await ResponseWrapper<TokenResponse>.FailAsync("Code is invalid or expired.");

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryDate = DateTime.Now.AddDays(7);
            await _userManager.UpdateAsync(user);

            var token = await GenerateJWTAsync(user);

            return await ResponseWrapper<TokenResponse>.SuccessAsync(new TokenResponse
            {
                Token = token,
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryDate
            });
        }
    }
}
