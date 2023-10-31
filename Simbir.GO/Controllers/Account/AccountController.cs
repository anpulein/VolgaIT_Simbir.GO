using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.GO.Data;
using Simbir.GO.Extensions;
using Simbir.GO.Models;
using Simbir.GO.Services;

namespace Simbir.GO.Controllers.Account;

[ApiController]
[Route("api/[controller]/[action]")]
public class AccountController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IBCryptNet _bCryptNet;
    private readonly ApplicationContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<Models.AccountInfo> _logger;

    /// <summary>
    /// Account Controller
    /// </summary>
    /// <param name="tokenService"></param>
    /// <param name="bCryptNet"></param>
    /// <param name="context"></param>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    public AccountController(ITokenService tokenService, IBCryptNet bCryptNet, ApplicationContext context, IConfiguration configuration, ILogger<Models.AccountInfo> logger)
    {
        _tokenService = tokenService;
        _bCryptNet = bCryptNet;
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Получение данных о текущем аккаунте
    /// </summary>
    /// <returns></returns>
    [Authorize, HttpGet]
    public ActionResult<AccountInfo> Me()
    {
        var username = User.Identity?.Name; 
        
        var account = _context.Accounts.FirstOrDefault(f => f.Username == username);
        if (account is null) return BadRequest($"User not found or not authenticated.");
        
        return account;
    }

    /// <summary>
    /// Получение нового jwt токена пользователя
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [ActionName("SignIn")]
    public ActionResult<AccountResponse> UserSignIn([FromQuery]AccountModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var account = _context.Accounts.FirstOrDefault(f => f.Username == model.Username);
        if (account is null) return Unauthorized("User does not exist");

        if (!_bCryptNet.IsPasswordHash(model.Password, account.Password))
        {
            return Unauthorized("Invalid password");
        }

        var accessToken = _tokenService.CreateToken(account);
        var refreshTokenValidityInDays = _configuration.GetSection("Jwt:RefreshTokenValidityInDays").Get<int>();
        account.RefreshToken = _configuration.GenerateRefreshToken();
        account.RefreshTokenExpiryTime =
            DateTime.UtcNow.AddDays(refreshTokenValidityInDays);

        _context.SaveChanges();

        var identity = new ClaimsIdentity(account.CreateClaims(), "jwt");
        HttpContext.User = new ClaimsPrincipal(identity);
        
        return Ok(new AccountResponse
        {
            Username = account.Username,
            Password = account.Password,
            Token = accessToken,
            RefreshToken = account.RefreshToken
        });
    }
    
    /// <summary>
    /// Регистрация нового аккаунта
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [ActionName("SignUp")]
    public ActionResult<AccountModel> UserSignUp([FromQuery]AccountModel model)
    {
        if (!ModelState.IsValid) return BadRequest(model);

        var findUser = _context.Accounts.Any(a => a.Username == model.Username);
        if (findUser) return BadRequest($"User with {model.Username} already exists");
        
        string passwordHash = _bCryptNet.GetPasswordHash(model.Password);

        var account = new Models.AccountInfo
        {
            Username = model.Username,
            Password = passwordHash
        };

        _context.Accounts.Add(account);
        _context.SaveChanges();

        return (new AccountModel
        {
            Username = account.Username,
            Password = account.Password
        });
    }
    
    /// <summary>
    /// Выход из аккаунта
    /// </summary>
    /// <returns></returns>
    [Authorize, HttpPost]
    [ActionName("SignOut")]
    public IActionResult UserSignOut()
    {
        var username = User.Identity?.Name;
        
        var account = _context.Accounts.FirstOrDefault(f => f.Username == username);
        if (account is null) 
            return BadRequest($"User not found or not authenticated");
        
        account.RefreshToken = null;
        _context.SaveChanges();
        
        return Ok($"Username {account.Username} sign out");
    }
    
    /// <summary>
    /// Обновление своего аккаунта
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [Authorize, HttpPut]
    public ActionResult<AccountModel> Update([FromQuery]AccountModel model)
    {
        if (!ModelState.IsValid) return BadRequest(model);
        
        var username = User.Identity?.Name;
        
        var account = _context.Accounts.FirstOrDefault(f => f.Username == username);
        if (account is null)
            return BadRequest($"User {model.Username} not found or not authenticated");
        
        account.Username = model.Username;
        account.Password = _bCryptNet.GetPasswordHash(model.Password);
        
        _context.SaveChanges();

        return (new AccountModel
        {
            Username = account.Username,
            Password = account.Password
        });
    }
}