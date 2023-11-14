using System.IO;
using System.IO.Compression;
using System.Net;
using CloudDrive.Domain;
using CloudDrive.Domain.Entities;
using CloudDrive.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CloudDrive.Services.Files
{
	public interface IFilesService
	{
		Task<List<DataDto>> Get();
		Task<Result<DataDto>> Get(int id);
		Task<Result<FileDto>> Download(int id);
		Task<Result<FileDto>> DownloadAll();
		Task<Result<DataDto>> Insert(IFormFile file);
		Task<Result> Delete(int id);
	}

	public class FilesService : IFilesService
	{
		private readonly AppDbContext _db;
		private readonly ILogger<FilesService> _logger;
		private readonly FileConfigurations _fileConfigurations;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public FilesService(
			AppDbContext db,
			ILogger<FilesService> logger,
			IHttpContextAccessor httpContextAccessor,
			FileConfigurations fileConfigurations
		)
		{
			_db = db;
			_logger = logger;
			_httpContextAccessor = httpContextAccessor;
			_fileConfigurations = fileConfigurations;
		}

        public async Task<List<DataDto>> Get()
        {
            var data = await _db.Data.ToListAsync();

            List<DataDto> results = new List<DataDto>();

            foreach (var item in data)
            {
                results.Add(new DataDto
                {
                    Id = item.Id,
                    FileName = item.OriginalFileName,
                    ContentType = item.ContentType
                });
            }

            return results;
        }

        public async Task<Result<DataDto>> Get(int id)
        {
            var data = await _db.Data.FindAsync(id);

            if (data == null)
            {
                return new Result<DataDto>
                {
                    Message = "Item not found",
                    IsSuccssfull = false,
                };
            }

            return new Result<DataDto>
            {
                IsSuccssfull = true,
                Data = new DataDto
                {
                    Id = data.Id,
                    FileName = data.OriginalFileName,
                    ContentType = data.ContentType
                }
            };
        }

        public async Task<Result<FileDto>> Download(int id)
        {
            var data = await _db.Data.FindAsync(id);

            if (data == null || !File.Exists(data.Path))
            {
                return new Result<FileDto>
                {
                    Message = "Item not found",
                    IsSuccssfull = false,
                };
            }

            Stream readFileStream = File.OpenRead(data.Path);

			return new Result<FileDto>
			{
				IsSuccssfull = true,
				Data = new FileDto
				{
					Stream = readFileStream,
					FileName = data.OriginalFileName,
					ContentType = data.ContentType
				}
			};
		}

		public async Task<Result<FileDto>> DownloadAll()
		{
			var data = await _db.Data.ToListAsync();

			if (data == null || data.Count == 0)
			{
				return new Result<FileDto>
				{
					Message = "Item not found",
					IsSuccssfull = false,
				};
			}

			Dictionary<string, int> usedNames = new Dictionary<string, int>(data.Count);

			string fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".zip";

			string path = Path.Combine(_fileConfigurations.FileSavePath, fileName);

			// TODO: Make file strem into a memory stream!!
			using (Stream stream = File.Create(path))
			{
				using ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Create);

				foreach (var item in data)
				{
					if (!File.Exists(item.Path))
					{
						continue;
					}


					string name = item.OriginalFileName;

					if (usedNames.ContainsKey(name))
					{
						int count = usedNames[name];

						usedNames[name]++;

						name = count + name;
					}
					else
					{
						usedNames[name] = 1;
					}

					var entry = zipArchive.CreateEntry(name, CompressionLevel.Optimal);

					using var entryStream = entry.Open();

					using Stream readFileStream = File.OpenRead(item.Path);

					await readFileStream.CopyToAsync(entryStream);
				}
			}

			return new Result<FileDto>
			{
				IsSuccssfull = true,
				Data = new FileDto()
				{
					FileName = fileName,
					ContentType = "application/zip",
					Stream = File.OpenRead(path)
				}
			};
		}

		public async Task<Result<DataDto>> Insert(IFormFile file)
		{
			var transaction = _db.Database.BeginTransaction();

            try
            {
                // Path
                var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                string path = Path.Combine(_fileConfigurations.FileSavePath, newFileName);

                // Contet type
                var originalName = file.FileName;

                // LINQ
                var data = new Data
                {
                    NewFileName = newFileName,
                    OriginalFileName = originalName,
                    ContentType = file.ContentType,
                    Path = path
                };

                _db.Data.Add(data);

                await _db.SaveChangesAsync();

                _logger.LogInformation(
                    "Saved '{originalName}' to databse with new name: '{newFileName}' with id: {id}",
                    data.OriginalFileName,
                    data.NewFileName,
                    data.Id
                );

                // file name
                using (Stream stream = File.Create(path))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation(
                    "Saved '{originalName}' to storage with new name: '{newFileName}'",
                    data.OriginalFileName,
                    data.NewFileName
                );

                transaction.Commit();

                return new Result<DataDto>
                {
                    Message = "Inserted",
                    IsSuccssfull = true,
                    Data = new DataDto
                    {
                        Id = data.Id,
                        FileName = data.OriginalFileName,
                        ContentType = data.ContentType
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to save file because of exception: '{Message}'", ex.Message);

                transaction.Rollback();

                return new Result<DataDto>
                {
                    Message = "Error while trying to save file due to technical reason with code: " + ex.HResult,
                    IsSuccssfull = false,
                };
            }
        }

        public async Task<Result> Delete(int id)
        {
            var transaction = _db.Database.BeginTransaction();

            try
            {
                var entity = _db.Data.Find(id);

				if (entity == null)
				{
					return new Result
					{
						Message = "Item not found",
						IsSuccssfull = false,
					};
				}

				_db.Data.Remove(entity);

                await _db.SaveChangesAsync();

                _logger.LogInformation(
                    "Removed '{originalName}' from databse with new name: '{newFileName}' with id: {id}",
                    entity.OriginalFileName,
                    entity.NewFileName,
                    entity.Id
                );

                string path = Path.Combine(_fileConfigurations.FileSavePath, entity.NewFileName);

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                else
                {
                    transaction.Rollback();

                    return new Result
                    {
                        Message = "Item with name " + entity.OriginalFileName + " doenst exist",
                        IsSuccssfull = false,
                    };
                }

				transaction.Commit();

                return new Result
                {
                    Message = "Removed from db",
                    IsSuccssfull = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to remove file because of exception: '{Message}'", ex.Message);

                transaction.Rollback();

				return new Result
				{
					Message = "Error while trying to remove file due to technical reason with code: " + ex.HResult,
					IsSuccssfull = false,
				};
			}
		}

	}

}