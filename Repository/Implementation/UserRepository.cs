using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Interfaces;

namespace Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _context;

        public UserRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating user: {ex.Message}", ex);
            }
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user by email: {ex.Message}", ex);
            }
        }


        public Task<bool> ExistsAsync(string email)
        {
            try
            {
                return _context.Users.AnyAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking if user exists: {ex.Message}", ex);
            }
        }
    }

}
