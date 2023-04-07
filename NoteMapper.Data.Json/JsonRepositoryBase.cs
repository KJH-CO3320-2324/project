using Newtonsoft.Json;
using NoteMapper.Core;
using NoteMapper.Data.Core.Errors;

namespace NoteMapper.Data.Json
{
    /// <summary>
    /// Basic document repository that stores json in local files. 
    /// Should not be used in production as it is not ACID.
    /// </summary>
    public abstract class JsonRepositoryBase<T>
    {
        private readonly IApplicationErrorRepository _applicationErrorRepository;
        private readonly JsonRepositorySettings _settings;

        protected JsonRepositoryBase(JsonRepositorySettings settings,
            IApplicationErrorRepository applicationErrorRepository)
        {
            _applicationErrorRepository = applicationErrorRepository;
            _settings = settings;
            DefaultUserId = _settings.DefaultUserId;
        }

        protected string DefaultUserId { get; }

        protected abstract string DirectoryName { get; }

        protected Task<ServiceResult> CreateAsync(string id, T entity)
        {
            return UpdateAsync(id, entity);
        }

        protected async Task<ServiceResult> DeleteAsync(string id)
        {
            string filePath = GetFilePath(id);

            try
            {                
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                
                return ServiceResult.Successful();
            }
            catch (Exception ex)
            {
                await LogException(ex, GetFilePath(id));
                return ServiceResult.Failure("Error deleting entity");
            }
        }

        protected async Task<T?> FindAsync(string id)
        {
            try
            {
                string filePath = GetFilePath(id);                
                if (!File.Exists(filePath))
                {
                    return default;
                }

                string json = await File.ReadAllTextAsync(filePath);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return default;
            }
        }

        protected async Task<ServiceResult> UpdateAsync(string id, T entity)
        {
            await EnsureDirectoryExistsAsync();

            string filePath = GetFilePath(id);

            try
            {                             
                string json = JsonConvert.SerializeObject(entity);
                await File.WriteAllTextAsync(filePath, json);
                return ServiceResult.Successful();
            }
            catch (Exception ex)
            {
                await LogException(ex, filePath);
                return ServiceResult.Failure("Error updating entity");
            }
        }

        private async Task EnsureDirectoryExistsAsync()
        {
            string path = GetPath();
            if (Directory.Exists(path))
            {
                return;
            }

            try
            {
                Directory.CreateDirectory(path);
            }            
            catch (Exception ex)
            {
                await LogException(ex, path);
            }
        }

        private string GetFilePath(string id)
        {
            string path = GetPath();
            string filePath = Path.Combine(path, $"{id}.json");
            return filePath;
        }

        private string GetPath()
        {
            string path = Path.Combine(_settings.FilePath, DirectoryName);            
            return path;
        }

        private async Task LogException(Exception ex, string filePath)
        {
            ApplicationError error = new(_settings.CurrentEnvironment, ex);
            error.AddProperty("FilePath", filePath);
            await _applicationErrorRepository.CreateAsync(error);
        }
    }
}