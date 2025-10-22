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
    [HttpGet]
    public async Task<IActionResult> GetGamesPaged([FromQuery] int page, [FromQuery] int number)
    {
        var games = await _gamesServices.GetAllAsync(page, number);
        return Ok(games);
    }
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
