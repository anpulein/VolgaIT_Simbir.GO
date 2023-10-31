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
    private readonly ILogger<Models.AccountInfo> _logger;
    
    /// <summary>
    /// AdminAccount controller
    /// </summary>
    /// <param name="bCryptNet"></param>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public AdminAccountController(IBCryptNet bCryptNet, ApplicationContext context, ILogger<Models.AccountInfo> logger)
    {
        _bCryptNet = bCryptNet;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Получение списка всех аккаунтов
    /// </summary>
    /// <param name="start"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [HttpGet("{start}/{count}")]
    public ActionResult<IEnumerable<AccountInfo>> GetAccounts(int start, int count)
    {
        if (start < 0 || count <= 0)
        {
            throw new ArgumentOutOfRangeException("Incorrect start and count parameters");
        }

        if (start > _context.Accounts.Count()) throw new ArgumentOutOfRangeException("Incorrect start parameter");

        return Ok(_context.Accounts
            .Skip(start)
            .Take(count)
            .ToList());
    }
    
    /// <summary>
    /// Получение информации об аккаунте по id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public ActionResult<AdminAccountModel> GetAccount(int id)
    {
        var account = _context.Accounts.FirstOrDefault(f => f.Id == id);
        if (account is null) return Unauthorized("User does not exist");
        
        return Ok(new AdminAccountModel
        {
            Username = account.Username,
            Password = account.Password,
            IsAdmin = account.IsAdmin,
            Balance = account.Balance
        });
    }
    
    /// <summary>
    /// Создание администратором нового аккаунта
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult<AdminAccountModel> Add([FromQuery]AdminAccountModel request)
    {
        if (!ModelState.IsValid) return BadRequest(request);

        var findUser = _context.Accounts.Any(a => a.Username == request.Username);
        if (findUser) return BadRequest($"User with {request.Username} already exists");

        string passwordHash = _bCryptNet.GetPasswordHash(request.Password);

        var account = new Models.AccountInfo
        {
            Username = request.Username,
            Password = passwordHash,
            IsAdmin = request.IsAdmin,
            Balance = request.Balance,
        };

        _context.Accounts.Add(account);
        _context.SaveChanges();

        return Ok(new AdminAccountModel
        {
            Username = account.Username,
            Password = account.Password,
            IsAdmin = account.IsAdmin,
            Balance = account.Balance
        });
    }

    /// <summary>
    /// Изменение администратором аккаунта по id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public ActionResult<AdminAccountModel> Update(int id, [FromQuery]AdminAccountModel request)
    {
        var account = _context.Accounts.FirstOrDefault(f => f.Id == id);
        if (account is null) BadRequest($"User with Id = {id} not found");

        var findUser = _context.Accounts.Any(a => a.Username == request.Username);
        if (findUser) return BadRequest($"User with {request.Username} already exists, use another one");

        account.Username = request.Username;
        account.Password = _bCryptNet.GetPasswordHash(request.Password);
        account.IsAdmin = request.IsAdmin;
        account.Balance = request.Balance;

        _context.SaveChanges();

        return Ok(new AdminAccountModel
        {
            Username = account.Username,
            Password = account.Password,
            IsAdmin = account.IsAdmin,
            Balance = account.Balance
        });
    }

    /// <summary>
    /// Удаление аккаунта по id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var account = _context.Accounts.FirstOrDefault(f => f.Id == id);
        if (account is null) return BadRequest($"Such with Id = {id} not found");

        _context.Accounts.Remove(account);
        _context.SaveChanges();
        
        return Ok("Success");
    }
}