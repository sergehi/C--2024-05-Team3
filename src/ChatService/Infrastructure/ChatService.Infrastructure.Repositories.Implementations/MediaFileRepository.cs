using ChatService.Entities;
using ChatService.Infrastructure.EntityFramework;
using ChatService.Services.Repositories.Abstractions;
using Common.Repositories;

namespace ChatService.Infrastructure.Repositories.Implementations;

public class MediaFileRepository: Repository<MediaFile, int>, IMediaFileRepository
{
    public MediaFileRepository(DatabaseContext context) : base(context)
    {
    }
}