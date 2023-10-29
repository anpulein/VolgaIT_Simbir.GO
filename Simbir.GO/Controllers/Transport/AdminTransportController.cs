using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simbir.GO.Data;
using Simbir.GO.Models.Transport;

namespace Simbir.GO.Controllers.Transport;

[ApiController]
[Route("api/Admin/Transport")]
[Authorize(Roles = "True")]
public class AdminTransportController : ControllerBase
{
    private readonly ApplicationContext _context;

    /// <summary>
    /// AdminTransport controller
    /// </summary>
    /// <param name="context"></param>
    public AdminTransportController(ApplicationContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получение списка всех транспортных средств
    /// </summary>
    /// <param name="start"></param>
    /// <param name="count"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [HttpGet("{start}/{count}/{type}")]
    public IEnumerable<TransportInfo> GetTransports(int start, int count, TransportType type)
    {
        if (start < 0 || count <= 0)
        {
            throw new ArgumentOutOfRangeException("Incorrect start and count parameters");
        }

        if (start > _context.Transports.Count()) 
            throw new ArgumentOutOfRangeException("Incorrect start parameter");

        return _context.Transports
            .Skip(start)
            .Take(count)
            .Where(w => w.TransportType == type)
            .ToList();
    }

    /// <summary>
    /// Получение информации о транспортном средстве по id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public ActionResult<TransportInfo> GetTransport(int id)
    {
        var transport = _context.Transports
            .Include(i => i.Owner)
            .FirstOrDefault(f => f.Id == id);
        if (transport is null) return NotFound($"Transport with Id = {id} not found");

        return Ok(transport);
    }

    /// <summary>
    /// Создание нового транспортного средства
    /// </summary>
    /// <param name="ownerId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("{ownerId}")]
    public ActionResult<TransportInfo> Add(long ownerId, [FromQuery] TransportModel request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
    
        var account = _context.Accounts.FirstOrDefault(f => f.Id == ownerId);
        if (account is null) return NotFound("User not found");

        var transport = new TransportInfo
        {
            OwnerId = ownerId,
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
    /// Изменение транспортного средства по id
    /// </summary>
    /// <param name="ownerId"></param>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{ownerId}/{id}")]
    public ActionResult<TransportInfo> Update(long ownerId, long id, [FromQuery] TransportModel request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var transport = _context.Transports
            .Include(i => i.Owner)
            .FirstOrDefault(f => f.Id == id);
        if (transport is null) return NotFound($"Transport with Id = {id} not found");

        if (transport.OwnerId != ownerId)
            return NotFound($"Transport with id = {id} this id does not belong to an authorized account");
        
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
    public ActionResult<TransportInfo> Delete(long id)
    {
        var transport = _context.Transports
            .FirstOrDefault(f => f.Id == id);
        if (transport is null) return NotFound($"Transport with Id = {id} not found");

        _context.Transports.Remove(transport);
        _context.SaveChanges();

        return NoContent();
    }
}