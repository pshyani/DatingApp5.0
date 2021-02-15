using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.DTO;
using DatingApp.API.Entities;
using DatingApp.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using DatingApp.API.Extensions;
using DatingApp.API.Helpers;

namespace DatingApp.API.Data
{
    public class LikeRepository : ILikeRepository
    {
        private readonly DataContext _context;
        public LikeRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
           return await _context.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if(likesParams.Predicate == "liked")
            {
                likes = likes.Where(p => p.SourceUserId == likesParams.UserId);
                users = likes.Select(p => p.LikedUser);
            }
            else if(likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(p => p.LikedUserId == likesParams.UserId);
                users = likes.Select(p => p.SourceUser);
            }
        
            var likedUsers = users.Select(user => new LikeDTO
            {
                UserName= user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            });

            return await PagedList<LikeDTO>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);

        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
                    .Include(x => x.LikedUsers)
                    .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}