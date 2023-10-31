using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simbir.GO.Data;
using Simbir.GO.Models.Transport;

namespace Simbir.GO.Controllers.Transport;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransportController : ControllerBase
{
    private ApplicationContext _context;

    /// <summary>
    /// Transport controller
    /// </summary>
    /// <param name="context"></param>
    public TransportController(ApplicationContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получение информации о транспорте по id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public ActionResult<TransportInfo> Get(long id)
    {
        var transport = _context.Transports
            .Include(i => i.Owner)
            .FirstOrDefault(f => f.Id == id);
        if (transport is null) return NotFound($"Transport with Id = {id} not found");

        return Ok(transport);
    }

    /// <summary>
    ///  Добавление нового транспорта
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult<TransportInfo> Add([FromQuery] TransportModel request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var username = User.Identity?.Name;

        var account = _context.Accounts.FirstOrDefault(f => f.Username == username);
        if (account is null) return NotFound("User not found or not authenticated.");

        var transport = new TransportInfo
        {
            OwnerId = account.Id,
            Owner = account,
            CanBeRented = request.CanBeRented,
            TransportType = request.TransportType,
            Model = request.Model,
            Color = request.Color,
            Identifier = request.Identifier,
            Description = request.Description,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            MinutePrice = request.MinutePrice,
            DayPrice = request.DayPrice
        };
        
        _context.Transports.Add(transport);
        _context.SaveChanges();

        return Ok(transport);
    }

    /// <summary>
    /// Изменение транспорта оп id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public ActionResult Update(long id, [FromQuery] TransportModel request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var username = User.Identity?.Name;

        var account = _context.Accounts.FirstOrDefault(f => f.Username == username);
        if (account is null) 
            return NotFound($"User {username} not found or not authenticated"); 
        
        var transport = _context.Transports.FirstOrDefault(f => f.Id == id);
        if (transport is null) return NotFound($"Transport with Id = {id} not found");

        if (transport.OwnerId != account.Id)
            return BadRequest($"Transport with id = {id} this id does not belong to an authorized account");
        
        transport.CanBeRented = request.CanBeRented;
        transport.TransportType = request.TransportType;
        transport.Model = request.Model;
        transport.Color = request.Color;
        transport.Identifier = request.Identifier;
        transport.Description = request.Description;
        transport.Latitude = request.Latitude;
        transport.Longitude = request.Longitude;
        transport.MinutePrice = request.MinutePrice;
        transport.DayPrice = request.DayPrice;

        _context.SaveChanges();

        return Ok(transport);
    }

    /// <summary>
    /// Удаление транспорта по id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public ActionResult Delete(long id)
    {
        var username = User.Identity?.Name;

        var account = _context.Accounts.FirstOrDefault(f => f.Username == username);
        if (account is null) 
            return NotFound($"User {username} not found or not authenticated"); 
        
        var transport = _context.Transports.FirstOrDefault(f => f.Id == id);
        if (transport is null) return NotFound("transport with Id = {id} not found");

        if (transport.OwnerId != account.Id)
            return BadRequest($"Transport with id = {id} this id does not belong to an authorized account");

        _context.Transports.Remove(transport);
        _context.SaveChanges();

        return NoContent(); // Success
    }
}