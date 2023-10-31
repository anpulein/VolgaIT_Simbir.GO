using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simbir.GO.Data;
using Simbir.GO.Models.Rent;
using Simbir.GO.Models.Transport;

namespace Simbir.GO.Controllers.Rent;

[ApiController]
[Route("api/[controller]/[action]")]
public class RentController : ControllerBase
{
    private readonly ApplicationContext _context;

    /// <summary>
    /// Rent controller
    /// </summary>
    /// <param name="context"></param>
    public RentController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<TransportModel>> Transport([FromQuery] RentRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var transports = _context.Transports
            .ToList()
            .Where(w =>
            {
                return CalculateDistance(request.Lat, request.Long, w.Latitude, w.Longitude) <= request.Radius && w.TransportType == request.TransportType;
            });

        if (transports.Any())
            return NotFound("Transports not found");

        return Ok(transports);
    }

    [Route("api/Rent")]
    [HttpGet("{rentId}")]
    public ActionResult<Rental> Get(int rentId)
    {
        var username = User.Identity?.Name;

        var account = _context.Accounts.FirstOrDefault(f => f.Username == username);
        if (account is null)
            return NotFound($"User {username} not found or not authenticated");

        var rent = _context.Rents
            .Include(a => a.Account)    
            .Include(t => t.Transport)
            .FirstOrDefault(f => f.Id == rentId);
        if (rent is null)
            return NotFound($"Rent not found");

        if (rent.AccountId != account.Id && rent.Transport.OwnerId == account.Id)
            return BadRequest("This lease is not issued to your account");
        
        return Ok(rent);
    }
    
    [Authorize, HttpGet]
    public ActionResult<IEnumerable<Rental>> MyHistory()
    {
        var username = User.Identity?.Name;

        var account = _context.Accounts.FirstOrDefault(f => f.Username == username);
        if (account is null) 
            return NotFound($"User {username} not found or not authenticated");

        var rents = _context.Rents
            .Include(a => a.Account)
            .Include(t => t.Transport)
            .Where(w => w.AccountId == account.Id)
            .ToList();

        if (rents.Count == 0)
            return NotFound($"The user {username} has an empty history of rents");

        return Ok(rents);
    }

    [Authorize, HttpGet("{transportId}")]
    public ActionResult<IEnumerable<Rental>> TransportHistory(int transportId)
    {
        var username = User.Identity?.Name;
        
        var account = _context.Accounts.FirstOrDefault(f => f.Username == username);
        if (account is null) 
            return NotFound($"User {username} not found or not authenticated");
        
        var rents = _context.Rents
            .Include(a => a.Account)
            .Include(t => t.Transport)
            .Where(w => w.TransportId == transportId && w.Transport.OwnerId == account.Id)
            .ToList();

        if (rents.Count == 0)
            return NotFound($"The user {username} has an empty history of rents");
        
        return Ok(rents);
    }

    [Authorize, HttpGet("{transportId}")]
    public ActionResult<IEnumerable<Rental>> New(int transportId, [FromQuery] PriceType priceType)
    {
        var username = User.Identity?.Name;

        var account = _context.Accounts.FirstOrDefault(f => f.Username == username);
        if (account is null)
            return NotFound($"User {username} not found or not authenticated");

        var transport = _context.Transports.FirstOrDefault(f => f.Id == transportId);
        if (transport is null)
            return NotFound($"Transport with Id = {transportId} not found");

        if (transport.OwnerId == account.Id)
            return BadRequest("The user cannot rent his car");

        var price = priceType == PriceType.Days ? transport.DayPrice : transport.MinutePrice;
        if (price is null)
            return BadRequest("Error: the cost per unit of time (day/minute) is not set for the machine");
        
        var rent = new Rental
        {
            TransportId = transport.Id,
            Transport = transport,
            AccountId = account.Id,
            Account = account,
            TimeStart = DateTime.Now,
            PriceOfUnit = (double)price,
            PriceType = priceType,
        };

        _context.Rents.Add(rent);
        _context.SaveChanges();

        return Ok(rent);
    }
        
    [Authorize, HttpGet("{rentId}/{lat}/{lon}")]
    public ActionResult<IEnumerable<Rental>> End(int rentId, double lat, double lon)
    {
        var username = User.Identity?.Name;

        var account = _context.Accounts.FirstOrDefault(f => f.Username == username);
        if (account is null)
            return NotFound($"User {username} not found or not authenticated");

        var rent = _context.Rents
            .Include(a => a.Account)    
            .Include(t => t.Transport)
            .FirstOrDefault(f => f.Id == rentId);
        if (rent is null)
            return NotFound($"Rent not found");

        if (rent.AccountId != account.Id)
            return BadRequest("This lease is not issued to your account");

        rent.TimeEnd = DateTime.Now;
        
        var resultTime = rent.TimeEnd - rent.TimeStart;
        if (rent.PriceType == PriceType.Days)
            rent.FinalPrice = resultTime.Value.Days * rent.PriceOfUnit;
        else
            rent.FinalPrice = resultTime.Value.Minutes * rent.PriceOfUnit;

        rent.Transport.Latitude = lat;
        rent.Transport.Longitude = lon;

        _context.SaveChanges();
        
        return Ok(rent);
    }
    
    private double CalculateDistance(double latA, double lonA, double latT, double lonT)
    {
        // Радиус земли
        double earthRadius = 6371; // Радиус Земли в километрах
        double dLat = Math.PI / 180 * (latT - latA);
        double dLon = Math.PI / 180 * (lonT - lonA);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(Math.PI / 180 * latA) * Math.Cos(Math.PI / 180 * latT) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadius * c;
    }
    
}