using CloudDrive.Services.Notebooks;
using Microsoft.AspNetCore.Mvc;

namespace CloudDrive.Controllers
{
	[Route("/api/Notebooks")]

	public class NotebookController : ControllerBase
	{
		private readonly INotebooksService _service;

		public NotebookController(INotebooksService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			return Ok(await _service.Get());
		}

		[HttpPost]
		public async Task<IActionResult> Post([FromBody] NotebookDto notebook)
		{
			var result = await _service.Insert(notebook);

			if (result.IsSuccssfull)
			{
				return Ok(result.Data);
			}

			return BadRequest(result);
		}


		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			var result = await _service.Get(id);

			if (result.IsSuccssfull)
			{
				return Ok(result.Data);
			}

			return NotFound(result);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete([FromRoute] int id)
		{
			var result = await _service.Delete(id);

			if (result.IsSuccssfull)
			{
				return NoContent();
			}
			else
			{
				return BadRequest(result);
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update([FromRoute] int id, [FromBody] NotebookDto user)
		{
			var result = await _service.Update(id, user);

			if (result.IsSuccssfull)
			{
				return Ok(result.Data);
			}
			else
			{
				return BadRequest(result);
			}
		}
	}

}