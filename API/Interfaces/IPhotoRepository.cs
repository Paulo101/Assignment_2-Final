using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IPhotoRepository
    {
        Task<IEnumerable<Photo>> GetUnapprovedPhotos();
         Task<Photo> GetPhotoById(int id);
         void RemovePhoto(int photoId);

        
    }
}