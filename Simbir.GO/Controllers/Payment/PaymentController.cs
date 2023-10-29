using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.GO.Data;

namespace Simbir.GO.Controllers.Payment;

[ApiController]
[Route("api/[controller]/[action]")]
public class PaymentController : ControllerBase
{
    private ApplicationContext _context;

    /// <summary>
    /// Payment Controller
    /// </summary>
    /// <param name="context"></param>
    public PaymentController(ApplicationContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Добавляет на баланс аккаунта с id={accountId} 250 000 денежных единиц.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = "True"), HttpPost("{id}")]
    public ActionResult Hesoyam(int id)
    {
        var account = _context.Accounts.FirstOrDefault(f => f.Id == id);
        if (account is null) return Unauthorized("User does not exists");

        account.Balance += 250000;
        _context.SaveChanges();

        return Ok("Success");
    }

    /// <summary>
    /// Добавляет на баланс своего аккаунта 250 000 денежных единиц.
    /// </summary>
    /// <returns></returns>
    [Authorize, HttpPost]
    public ActionResult Hesoyam()
    {
        var username = User.Identity?.Name;

        var account = _context.Accounts.FirstOrDefault(f => f.Username == username);
        if (account is null) return BadRequest($"User not found or not authenticated.");

        account.Balance += 250000;
        _context.SaveChanges();

        return Ok("Success");
    }
    
    
}