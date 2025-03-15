using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike?> GetUserLike(int SourceUserId, int TargetUserId);
        Task<PagedList<MemberDto>> GetUserLikes(LikesParams likesParams);
        Task <IEnumerable<int>>GetCurrentUserLikeIds(int currentUserIds);
        void DeleteLike(UserLike like);
        void AddLike(UserLike like);
        Task <bool> SaveChanges(); 

    }
}