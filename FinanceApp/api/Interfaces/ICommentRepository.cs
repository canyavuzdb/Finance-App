using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetAllAsync(CommentQueryObject queryObject);
        Task<Comment?> GetByIdAsync(int Id);
        Task<Comment> CreateAsync(Comment comment);
        Task<Comment?> UpdateAsync(int Id, Comment commentModel);
        Task<Comment?> DeleteAsync(int Id);
    }
}