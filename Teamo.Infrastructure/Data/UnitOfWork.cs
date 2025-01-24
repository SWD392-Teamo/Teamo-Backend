using Infrastructure.Data;
using System.Collections;
using System.Collections.Concurrent;
using Teamo.Core.Entities;
using Teamo.Core.Interfaces;

namespace Teamo.Infrastructure.Data
{
    /// <summary>
    /// Base implementation of the unit of work interface.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _context;
        private ConcurrentDictionary<string, object> _repositories;

        /// <summary>
        /// Injects a DbContext instance to be used by all repositories.
        /// </summary>
        /// <param name="context"></param>
        public UnitOfWork(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// The asynchronous Complete method persists changes made to the database.
        /// </summary>
        /// <returns>
        /// The task result contains the value true or false 
        /// based on the number of rows affected
        /// </returns>
        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// The Dispose method implements the IDisposable interface 
        /// and ensures proper resource cleanup associated with the UnitOfWork.
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
        }

        /// <summary>
        /// The Repository method provides a generic way to retrieve a 
        /// repository instance for a specific entity type T. 
        /// It implements a caching mechanism to optimize repository creation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        /// Casts the retrieved repository instance (from the cache or newly created) 
        /// to IGenericRepository of type T and returns it.
        /// </returns>
        public IGenericRepository<T> Repository<T>() where T : BaseEntity
        {
            var type = typeof(T).Name;

            return (IGenericRepository<T>)_repositories.GetOrAdd(type, t =>
            {
                // This will return a constructed concrete type GenericRepository<T>
                var repoType = typeof(GenericRepository<>).MakeGenericType(typeof(T));
                
                // This will create an instance of that GenericRepository with DbContext injected
                return Activator.CreateInstance(repoType, _context)
                    ?? throw new InvalidOperationException(
                        String.Format("Could not create repository instance for {0}", t));
            });
        }
    }
}
