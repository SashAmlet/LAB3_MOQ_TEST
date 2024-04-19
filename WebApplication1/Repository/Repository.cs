
using Microsoft.EntityFrameworkCore;
using WebApplication1.Context;
using WebApplication1.Models;

namespace WebApplication1.Repository
{
    public class Repository: IRepository
    {
        private readonly APIContext context;
        public Repository(APIContext context)
        {
            this.context = context;
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var entities = await context.Set<User>().ToListAsync();

            if (entities == null)
            {
                throw new ArgumentNullException("The GetAll failed, there are no such an entities");
            }
            return entities;
        }        
        public async Task<User> GetByIdAsync(int id)
        {
            var entity = await context.Set<User>().FindAsync(id);

            if (entity == null)
            {
                throw new ArgumentNullException("The GetById failed, there is no such entity");
            }
            return entity;
        }
        public async Task AddAsync(User entity)
        {
            await context.Set<User>().AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, User entity)
        {
            if (id != entity.Id)
                throw new ArgumentException("The id value does not match the entity.Id value");

            context.Set<User>().Update(entity);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TExists(id))
                {
                    throw new ArgumentNullException("The update failed, there is no such entity");
                }
                else
                {
                    throw;
                }
            }
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await context.Set<User>().FindAsync(id);
            if (entity == null)
            {
                return false;
            }
            context.Remove(entity);
            await context.SaveChangesAsync();
            return true;
        }

        private bool TExists(int id)
        {
            return (context.Set<User>()?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
