using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
      
    public class PhotoRepository : IPhotoRepository
    {
        private readonly DataContext _context;
        public PhotoRepository(DataContext context)
        {
            _context = context;

        }

        public async Task<Photo> GetPhotoById(int id)
        {
            return await _context.Photos.FindAsync(id);
      
        }

        public async Task<IEnumerable<Photo>> GetUnapprovedPhotos()
        {
            return await _context.Photos.
            Where(s => !s.isApproved)
            .ToListAsync();
        }

        public void RemovePhoto(int photoId)
        {
            
            var photo = _context.Photos.SingleOrDefault(s => s.Id == photoId);
            _context.Photos.Remove(photo);
            _context.SaveChanges();
        }
    }
}