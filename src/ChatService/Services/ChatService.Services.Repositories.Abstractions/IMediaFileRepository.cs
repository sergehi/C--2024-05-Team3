using ChatService.Entities;
using Common.Repositories;

namespace ChatService.Services.Repositories.Abstractions;

public interface IMediaFileRepository: IRepository<MediaFile, int>
{
}