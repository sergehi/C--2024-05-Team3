using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtSettings _jwtSettings;

    public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _jwtSettings = jwtSettings.Value;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = _userManager.Users.ToList();
        var userList = new List<object>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userList.Add(new
            {
                user.Id,
                user.UserName,
                user.Email,
                Roles = roles
            });
        }

        return Ok(userList);
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = _roleManager.Roles.ToList();
        var roleList = roles.Select(role => new
        {
            role.Id,
            role.Name
        }).ToList();

        return Ok(roleList);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (model == null || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password) || string.IsNullOrWhiteSpace(model.Email))
        {
            return BadRequest("Invalid registration request");
        }

        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            return BadRequest("Email is already taken");
        }

        if (!string.IsNullOrWhiteSpace(model.Role))
        {
            var roleExists = await _roleManager.RoleExistsAsync(model.Role);
            if (!roleExists)
            {
                return BadRequest("Role does not exist");
            }
        }

        var user = new ApplicationUser
        {
            UserName = model.Username,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            MiddleName = model.MiddleName,
            RefreshToken = GenerateRefreshToken(),
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30)
        };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            if (!string.IsNullOrWhiteSpace(model.Role))
            {
                var roleExists = await _roleManager.RoleExistsAsync(model.Role);
                if (roleExists)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                }
            }

            return Ok(new { Result = "User created successfully" });
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (model == null || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
        {
            return BadRequest("Invalid login request");
        }

        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null)
        {
            return Unauthorized();
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false); // Password hash is checked here
        if (!result.Succeeded)
        {
            return Unauthorized();
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey ?? string.Empty);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, userRoles.FirstOrDefault() ?? string.Empty)
            }),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
        await _userManager.UpdateAsync(user);

        return Ok(new { Token = tokenHandler.WriteToken(token) });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenModel tokenModel)
    {
        if (tokenModel is null)
        {
            return BadRequest("Invalid client request");
        }

        var principal = GetPrincipalFromExpiredToken(tokenModel.AccessToken);
        var username = principal?.Identity?.Name;

        if (string.IsNullOrEmpty(username) || principal?.Claims == null)
        {
            return BadRequest("Invalid token");
        }

        var user = await _userManager.FindByNameAsync(username);

        if (user == null || user.RefreshToken != tokenModel.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return BadRequest("Invalid client request");
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        var newAccessToken = GenerateAccessToken(new Claim[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
        new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
        new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
        new Claim(ClaimTypes.Role, userRoles.FirstOrDefault() ?? string.Empty)
        });

        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
        await _userManager.UpdateAsync(user);

        return Ok(new AuthResponse { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
    }



    [HttpPost("create-role")]
    public async Task<IActionResult> CreateRole([FromBody] RoleModel model)
    {
        if (model == null || string.IsNullOrWhiteSpace(model.RoleName))
        {
            return BadRequest("Invalid role request");
        }

        var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);
        if (roleExists)
        {
            return BadRequest("Role already exists");
        }

        var roleResult = await _roleManager.CreateAsync(new IdentityRole(model.RoleName));
        if (!roleResult.Succeeded)
        {
            return BadRequest(roleResult.Errors);
        }

        return Ok(new { Result = "Role created successfully" });
    }

    [HttpPost("remove-role")]
    public async Task<IActionResult> RemoveRole([FromBody] UserRoleModel model)
    {
        if (model == null || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.RoleName))
        {
            return BadRequest("Invalid role removal request");
        }

        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null)
        {
            return BadRequest("User not found");
        }

        var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);
        if (!roleExists)
        {
            return BadRequest("Role does not exist");
        }

        var result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(new { Result = "Role removed successfully" });
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] UserRoleModel model)
    {
        if (model == null || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.RoleName))
        {
            return BadRequest("Invalid role assignment request");
        }

        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null)
        {
            return BadRequest("User not found");
        }

        var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);
        if (!roleExists)
        {
            return BadRequest("Role does not exist");
        }

        var result = await _userManager.AddToRoleAsync(user, model.RoleName);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(new { Result = "Role assigned successfully" });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin-endpoint")]
    public IActionResult AdminEndpoint()
    {
        return Ok("This is an admin-only endpoint.");
    }

    #region Helper method

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    private string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey ?? string.Empty);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey ?? string.Empty);
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;

        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    #endregion
}