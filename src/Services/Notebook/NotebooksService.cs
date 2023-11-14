using CloudDrive.Domain;
using CloudDrive.Domain.Entities;
using CloudDrive.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CloudDrive.Services.Notebooks
{
	public interface INotebooksService
	{
		Task<List<NotebookDto>> Get();
		Task<Result<NotebookDto>> Insert(NotebookDto notebook);
		Task<Result<NotebookDto>> Get(int id);
		Task<Result> Delete(int id);
		Task<Result<NotebookDto>> Update(int id, NotebookDto notebook);
	}

	public class NotebooksService : INotebooksService
	{
		private readonly AppDbContext _db;
		private readonly ILogger<NotebooksService> _logger;

		public NotebooksService(
			AppDbContext db,
			ILogger<NotebooksService> logger
		)
		{
			_db = db;
			_logger = logger;
		}
		public async Task<List<NotebookDto>> Get()
		{
			var notebook = await _db.Notebooks.ToListAsync();

			List<NotebookDto> results = new List<NotebookDto>();

			foreach (var item in notebook)
			{
				results.Add(new NotebookDto
				{
					Id = item.Id,
					Name = item.Name,
					CreateDate = item.CreateDate,
					Category = item.Category,
					Color = item.Color
				});
			}
			return results;
		}

		public async Task<Result<NotebookDto>> Insert(NotebookDto notebook)
		{
			var transaction = _db.Database.BeginTransaction();
			try
			{

				// LINQ
				var data = new Notebook
				{
					Name = notebook.Name,
					CreateDate = DateTime.Now,
					Category = notebook.Category,
					Color = notebook.Color

				};

				_db.Notebooks.Add(data);

				await _db.SaveChangesAsync();

				_logger.LogInformation(
					"Saved Notebook to databse with  name: '{Name}' with id: {id}",
					data.Id,
					data.Name
				);

				transaction.Commit();

				return new Result<NotebookDto>
				{
					Message = "Inserted",
					IsSuccssfull = true,
					Data = new NotebookDto
					{
						Id = data.Id,
						Name = data.Name,
						CreateDate = data.CreateDate,
						Category = data.Category,
						Color = data.Color
					}
				};

			}
			catch (Exception ex)
			{
				_logger.LogError("Failed to save Notebook because of exception: '{Message}'", ex.Message);

				transaction.Rollback();

				return new Result<NotebookDto>
				{
					Message = "Error while trying to save Notebook due to technical reason with code: " + ex.HResult,
					IsSuccssfull = false,
				};
			}
		}
		public async Task<Result<NotebookDto>> Get(int id)
		{
			var data = await _db.Notebooks.FindAsync(id);

			if (data == null)
			{
				return new Result<NotebookDto>
				{
					Message = "Notebook not found",
					IsSuccssfull = false,
				};
			}

			return new Result<NotebookDto>
			{
				IsSuccssfull = true,
				Data = new NotebookDto
				{
					Id = data.Id,
					Name = data.Name,
					CreateDate = data.CreateDate,
					Category = data.Category,
					Color = data.Color
				}
			};
		}

		public async Task<Result> Delete(int id)
		{
			var transaction = _db.Database.BeginTransaction();

			try
			{
				var notebook = await _db.Notebooks.FindAsync(id);
				if (notebook == null)
				{
					return new Result
					{
						IsSuccssfull = false,
						Message = "notebook not found",
					};
				}
				_db.Notebooks.Remove(notebook);

				await _db.SaveChangesAsync();

				_logger.LogInformation(
					"Deleted notebook from database with name '{Name}' and id: {Id}",
					notebook.Name,
					notebook.Id
				);

				transaction.Commit();

				return new Result
				{
					IsSuccssfull = true,
					Message = "Deleted from database",
				};
			}
			catch (Exception ex)
			{
				_logger.LogError("Failed to delete notebook because of exception: '{Message}'", ex.Message);

				transaction.Rollback();

				return new Result
				{
					IsSuccssfull = false,
					Message = "Error while trying to delete the notebook due to technical reason with code: " + ex.HResult,
				};
			}
		}
		public async Task<Result<NotebookDto>> Update(int id, NotebookDto notebook)
		{
			var transaction = _db.Database.BeginTransaction();

			try
			{
				var entity = await _db.Notebooks.FindAsync(id);

				if (entity == null)
				{
					return new Result<NotebookDto>
					{
						IsSuccssfull = false,
						Message = "notebook not found",
					};
				}
				entity.Name = notebook.Name;
				entity.CreateDate = notebook.CreateDate;
				entity.Color = notebook.Color;
				entity.Category = notebook.Category;

				_logger.LogInformation(
					"notebook found"
				);

				_db.Notebooks.Update(entity);

				await _db.SaveChangesAsync();

				transaction.Commit();

				return new Result<NotebookDto>
				{
					IsSuccssfull = true,
					Message = "Updated",
					Data = new NotebookDto
					{	
						Name = entity.Name,
						Category = entity.Category,
						CreateDate = entity.CreateDate,
						Color = entity.Color
					}
				};
			}
			catch (Exception ex)
			{
				_logger.LogError("Failed to update notebook because of exception: '{Message}'", ex.Message);

				transaction.Rollback();

				return new Result<NotebookDto>
				{
					IsSuccssfull = false,
					Message = "Error while trying to update the notebook due to technical reason with code: " + ex.HResult,
				};
			}
		}
	}
}