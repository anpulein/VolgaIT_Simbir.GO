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
    private static Models.Account _account;
    private readonly ITokenService _tokenService;
    private readonly IBCryptNet _bCryptNet;
    private readonly ApplicationContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<Models.Account> _logger;

    public AccountController(ITokenService tokenService, IBCryptNet bCryptNet, ApplicationContext context, IConfiguration configuration, ILogger<Models.Account> logger)
    {
        _tokenService = tokenService;
        _bCryptNet = bCryptNet;
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    [Authorize, HttpGet]
    public ActionResult<Models.Account> Me() => _context.Accounts.FirstOrDefault(f => f.Username == _account.Username);

    [HttpPost]
    public ActionResult<AccountResponse> SignIn(AccountRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var account = _context.Accounts.FirstOrDefault(f => f.Username == request.Username);

        if (account is null) return Unauthorized("User does not exist");

        if (!_bCryptNet.IsPasswordHash(request.Password, account.Password))
        {
            return BadRequest("Invalid password");
        }

        var accessToken = _tokenService.CreateToken(account);
        account.RefreshToken = _configuration.GenerateRefreshToken();
        account.RefreshTokenExpiryTime =
            DateTime.UtcNow.AddDays(_configuration.GetSection("Jwt:RefreshTokenValidityInDays").Get<int>());

        _context.SaveChanges();

        _account = account;
        
        return Ok(new AccountResponse
        {
            Username = account.Username,
            Password = account.Password,
            Token = accessToken,
            RefreshToken = account.RefreshToken
        });
    }
    
    [HttpPost]
    public ActionResult<AccountRequest> SignUp(AccountRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(request);

        var findUser = _context.Accounts.Any(a => a.Username == request.Username);

        if (findUser) return BadRequest($"User with {request.Username} already exists");
        
        string passwordHash = _bCryptNet.GetPasswordHash(request.Password);

        var account = new Models.Account
        {
            Username = request.Username,
            Password = passwordHash
        };

        _context.Accounts.Add(account);
        _context.SaveChanges();

        return (new AccountRequest
        {
            Username = account.Username,
            Password = account.Password
        });
    }
    
    
    [Authorize, HttpPost]
    public IActionResult SignOut()
    {
        var account = _context.Accounts.FirstOrDefault(f => f.Username == _account.Username);
        account.RefreshToken = null;
        _context.SaveChanges();
        
        return Ok($"Username {account.Username} sign out.");
    }
    
    [Authorize, HttpPut]
    public ActionResult<AccountRequest> Update(AccountRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(request);
        
        if (_context.Accounts.Any(a => a.Username == request.Username))
            return BadRequest($"Such {request.Username} already exists, use another one");

        var account = _context.Accounts.FirstOrDefault(f => f.Username == _account.Username);
        account.Username = request.Username;
        account.Password = _bCryptNet.GetPasswordHash(request.Password);
        
        _context.SaveChanges();

        return (new AccountRequest
        {
            Username = account.Username,
            Password = account.Password
        });
    }
}