using System;

namespace SalesSystem.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private User() { }

    public User(string username, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("El nombre de usuario no puede estar vacio", nameof(username));
        
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("El hash de contraseña no puede estar vacio", nameof(passwordHash));

        Id = Guid.NewGuid();
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateEmail(string email)
    {
        Email = email;
    }
}
