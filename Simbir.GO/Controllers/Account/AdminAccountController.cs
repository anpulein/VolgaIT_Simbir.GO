using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.GO.Data;
using Simbir.GO.Models;
using Simbir.GO.Services;

namespace Simbir.GO.Controllers.Account;

[ApiController]
[Route("api/Admin/Account")]
[Authorize(Roles = "True")]
public class AdminAccountController : ControllerBase
{
    private readonly IBCryptNet _bCryptNet;
    private readonly ApplicationContext _context;
    private readonly ILogger<Models.Account> _logger;
    
    public AdminAccountController(IBCryptNet bCryptNet, ApplicationContext context, ILogger<Models.Account> logger)
    {
        _bCryptNet = bCryptNet;
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<Models.Account> GetAccounts()
    {
        
        return null;
    }
    
    [HttpGet("{id}")]
    public ActionResult<Models.Account> GetAccount(int id)
    {
        var account = _context.Accounts.FirstOrDefault(f => f.Id == id);

        if (account is null) return Unauthorized("User does not exist");
        
        return account;
    }

    [HttpPost]
    public ActionResult<Models.Account> Add(Models.Account request)
    {
        if (!ModelState.IsValid) return BadRequest(request);

        var findUser = _context.Accounts.Any(a => a.Username == request.Username);

        if (findUser) return BadRequest($"User with {request.Username} already exists");

        string passwordHash = _bCryptNet.GetPasswordHash(request.Password);

        var account = new Models.Account
        {
            Username = request.Username,
            Password = passwordHash,
            IsAdmin = request.IsAdmin,
            Balance = request.Balance,
        };

        _context.Accounts.Add(account);
        _context.SaveChanges();
        
        return account;
    }

    [HttpPut("{id}")]
    public ActionResult<Models.Account> Update(int id, Models.Account request)
    {
        var account = _context.Accounts.FirstOrDefault(f => f.Id == id);
        if (account is null) BadRequest($"Such with Id = {id} already exists");

        var findUser = _context.Accounts.Any(a => a.Username == request.Username);
        if (findUser) return BadRequest($"Such {request.Username} already exists, use another one");

        account.Username = request.Username;
        account.Password = _bCryptNet.GetPasswordHash(request.Password);
        account.IsAdmin = request.IsAdmin;
        account.Balance = request.Balance;

        _context.SaveChanges();
        
        return account;
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var account = _context.Accounts.FirstOrDefault(f => f.Id == id);
        if (account is null) return BadRequest($"Such with Id = {id} already exists");

        _context.Accounts.Remove(account);
        _context.SaveChanges();
        
        return Ok("Success");
    }
}