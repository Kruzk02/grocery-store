using API.Data;
using API.Entity;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Impl;

public class UserService(UserDbContext context) : IUserService
{

    public async Task<List<User>> GetAllAsync() => await context.Users.ToListAsync();
    
    public async Task<User?> GetByIdAsync(int id) => await context.Users.FindAsync(id);

    public async Task<User> AddAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> UpdateAsync(int id, User user)
    {
        var existing = await context.Users.FindAsync(id);
        if (existing is null) return false;

        existing.Username = user.Username;
        existing.Email = user.Email;
        existing.Password = user.Password;
        
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await context.Users.FindAsync(id);
        if (user is null) return false;

        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return true;
    }
}