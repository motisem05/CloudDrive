using CloudDrive.Services.Files;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;

namespace CloudDrive.Controllers
{
	[Route("/api/Files")]
	public class FilesController : ControllerBase
	{
		private readonly IFilesService _filesService;

		public FilesController(IFilesService filesService)
		{
			_filesService = filesService;
		}

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			return Ok(await _filesService.Get());
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			var result = await _filesService.Get(id);

			if (result.IsSuccssfull)
			{
				return Ok(result.Data);
			}

			return NotFound(result);
		}

		[HttpGet("Download/{id}")]
		public async Task<IActionResult> Download(int id)
		{
			var result = await _filesService.Download(id);

			if (result.IsSuccssfull)
			{
				return File(result.Data.Stream, result.Data.ContentType, result.Data.FileName);
			}

			return NotFound(result);
		}

		[HttpGet("DownloadAll")]
		public async Task<IActionResult> DownloadAll()
		{
			var result = await _filesService.DownloadAll();

			if (result.IsSuccssfull)
			{
				return File(result.Data.Stream, result.Data.ContentType, result.Data.FileName);
			}

			return NotFound(result);
		}

		[HttpPost]
		public async Task<IActionResult> Post([FromForm] IFormFile file)
		{
			var result = await _filesService.Insert(file);

			if (result.IsSuccssfull)
			{
				return Ok(result);
			}

			return BadRequest(result);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete([FromRoute] int id)
		{
			var result = await _filesService.Delete(id);

			if (result.IsSuccssfull)
			{
				return NoContent();
			}
			else
			{
				return BadRequest(result);
			}
		}
	}
}