using Microsoft.AspNetCore.Mvc;
using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;

namespace MyFinlys.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegisterController : ControllerBase
{
    private readonly IRegisterService _registerService;

    public RegisterController(IRegisterService registerService)
    {
        _registerService = registerService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _registerService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RegisterCreateDto dto)
    {
        var created = await _registerService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] RegisterUpdateDto dto)
    {
        var updated = await _registerService.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _registerService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
