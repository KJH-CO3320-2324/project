namespace NoteMapper.Services.Users
{
    public interface IUserService
    {
        Task<UserPreferences> GetPreferences(Guid? userId);

        Task UpdateUserPreferences(Guid userId, UserPreferences preferences);
    }
}
