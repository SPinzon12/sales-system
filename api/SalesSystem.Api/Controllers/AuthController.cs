using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesSystem.Application.DTOs;
using SalesSystem.Application.UseCases;

namespace SalesSystem.Api.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest(new { error = "El usuario y contraseña son requeridos" });
        }

        _logger.LogInformation("Intento de autenticacion para usuario: {Username}", dto.Username);

        var result = await _authService.LoginAsync(dto);
        
        if (result == null)
        {
            _logger.LogWarning("Autenticacion fallida para usuario: {Username}", dto.Username);
            return Unauthorized(new { error = "Usuario o contraseña inválidos" });
        }

        _logger.LogInformation("Autenticacion exitosa para usuario: {Username}", dto.Username);
        return Ok(result);
    }
}
