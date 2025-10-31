using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.DBContext;
using Server.Games.Entities;
using Server.Games.Services;

namespace Server.Games.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGamesServices _gamesServices;

    public GamesController(IGamesServices gamesServices)
    {
        _gamesServices = gamesServices;
    }

    // GET: api/games
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var games = await _gamesServices.GetAllAsync();
        return Ok(games);
    }
    [HttpGet("Filtered")]
    public async Task<IActionResult> GetGamesPaged([FromQuery] string? orderBy, [FromQuery] string? filterByCategory, [FromQuery] string? search ,[FromQuery] int page, [FromQuery] int batch)
    {
        var games = await _gamesServices.GetAllAsync(orderBy, filterByCategory, search, page, batch);
        return Ok(games);
    }
    [HttpGet("user/{id}")]
    public async Task<IActionResult> GetAllByUser()
    {
        // TODO: Filter by user
        var games = await _gamesServices.GetAllAsync();
        return Ok(games);
    }

    [Authorize]
    [HttpGet("user/Filtered")]
    public async Task<IActionResult> GetGamesPagedByUser([FromQuery] string? orderBy, [FromQuery] string? filterByCategory, [FromQuery] string? filterByStatus ,[FromQuery] string? search, [FromQuery] int page, [FromQuery] int number, [FromQuery] long userId)
    {
        //TODO: Filter by user
        var games = await _gamesServices.GetAllByUserAsync(orderBy,filterByCategory,filterByStatus,search, page, number,userId);
        return Ok(games);
    }
    [Authorize]
    [HttpPost("AddRelation")]
    public async Task<IActionResult> AddGamesByUser([FromBody] GamesUserRelationEntity game)
    {
        //TODO: Filter by user
        await _gamesServices.AddGameByUserAsync(game);
        return Ok();
    }
    [Authorize]
    [HttpPut("UpdateRelation")]
    public async Task<IActionResult> UpdateGamesByUser([FromBody] GamesUserRelationEntity game)
    {
        //TODO: Filter by user
        await _gamesServices.UpdateGameByUserAsync(game);
        return Ok();
    }
    [Authorize]
    [HttpDelete("DeleteRelation/{id}")]
    public async Task<IActionResult> DeleteGamesByUser([FromRoute] long id)
    {
        //TODO: Filter by user
        await _gamesServices.DeleteGameByUserAsync(id);
        return Ok();
    }
    [Authorize(Roles = "Admin")]
    // GET: api/games/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromHeader] long id)
    {
        var game = await _gamesServices.GetByIdAsync(id);
        if (game == null)
            return NotFound();
        return Ok(game);
    }
    [Authorize(Roles ="Admin")]
    // POST: api/games
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GamesEntity game)
    {
        if (game == null)
            return BadRequest();
        try { 
            var existingGame = await _gamesServices.GetByIdAsync(game.Id);
            if (existingGame != null)
            {
                return Conflict($"A game with ID {game.Id} already exists.");
            }
            await _gamesServices.AddAsync(game);
        }
        catch(InvalidOperationException e) {
            return Conflict(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        return Created();
    }
    [Authorize(Roles = "Admin")]
    // PUT: api/games/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromHeader] long id,[FromBody] GamesEntity updated)
    {
        if (updated == null || id != updated.Id)
            return BadRequest();

        try
        {
            await _gamesServices.UpdateAsync(updated);  
        }catch (KeyNotFoundException)
        {
            return NotFound();
        }catch (Exception e)
        {
            return BadRequest(e.Message);
        }

        return Ok();
    }
    [Authorize(Roles = "Admin")]
    // DELETE: api/games/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromHeader] long id)
    {
        try {
            await _gamesServices.DeleteAsync(id);
        }catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

        return NoContent();
    }
}
