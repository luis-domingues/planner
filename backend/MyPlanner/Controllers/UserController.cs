using Microsoft.AspNetCore.Mvc;
using MyPlanner.Infra;
using MyPlanner.Models;

namespace MyPlanner.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly PlannerDbContext _context;

    public UserController(PlannerDbContext context)
    {
        _context = context;
    }

    [HttpPost("registerUser")]
    public IActionResult Register([FromBody] User user)
    {
        if (user == null || string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.MobilePhone) ||
            string.IsNullOrWhiteSpace(user.Password))
            return BadRequest("Preencher todos os campos");

        if (_context.Users.Any(u => u.Username == user.Username))
            return Conflict("Usuário já cadastrado");
                
        if (_context.Users.Any(u => u.MobilePhone == user.MobilePhone))
            return Conflict("Telefone já cadastrado");

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        _context.Users.Add(user);
        _context.SaveChanges();
        return Ok("Registro feito com sucesso");
    }

    [HttpPut("updateUser/{id}")]
    public IActionResult UpdateUser([FromBody] User user, int id)
    {
        if (user == null || id <= 0)
            return BadRequest("Id inválido");

        var existingUser = _context.Users.FirstOrDefault(u => u.Id == id);
        if (existingUser == null)
            BadRequest("Usuário não encontrado");
        
        if (!string.IsNullOrWhiteSpace(user.Username) && user.Username != existingUser.Username && _context.Users.Any(u => u.Username == user.Username))
            return Conflict("Usuário já cadastrado");

        if (!string.IsNullOrWhiteSpace(user.MobilePhone) && user.MobilePhone != existingUser.MobilePhone &&
            _context.Users.Any(u => u.MobilePhone == user.MobilePhone))
            return Conflict("Telefone já cadastrado");

        if (!string.IsNullOrWhiteSpace(user.Username))
            existingUser.Username = user.Username;
        if (!string.IsNullOrWhiteSpace(user.MobilePhone))
            existingUser.MobilePhone = user.MobilePhone;
        if (!string.IsNullOrWhiteSpace(user.Password))
        {
            if (user.Password.Length < 6)
                return BadRequest("A senha deve possuir pelo menos 6  caracteres");
            
            existingUser.Password = user.Password;
        }

        _context.Users.Update(existingUser);
        _context.SaveChanges();

        return Ok(new
        {
            Message = "Informações atualizadas com sucesso",
            userId = existingUser.Id
        });
    }

    [HttpGet("getUserInfo")]
    public IActionResult GetUserInfo(int id)
    {
        if (id <= 0) return BadRequest("ID inválido");
        
        var user = _context.Users
            .Where(u => u.Id == id)
            .Select(u => new
            {
                u.Id,
                u.Username,
                u.MobilePhone
            })
            .FirstOrDefault();

        if (user == null) return NotFound("Usuário não encontrado");
        
        return Ok(user);
    }

    [HttpDelete("deleteUser/{id}")]
    public IActionResult DeleteUser(int id)
    {
        var user = _context.Users
            .FirstOrDefault(u => u.Id == id);

        if (user == null)
            return NotFound("Usuário não encontrado");

        _context.Users.Remove(user);
        _context.SaveChanges();

        return Ok(new
        {
            Message = "Usuário excluído com sucesso",
            UserId = id
        });
    }
}