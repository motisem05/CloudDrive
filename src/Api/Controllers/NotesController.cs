using CloudDrive.Domain.Entities;
using CloudDrive.Services.Files;
using CloudDrive.Services.Note;
using Microsoft.AspNetCore.Mvc;

namespace CloudDrive.Controllers
{
	[Route("/api/Notes")]
	public class NotesController : ControllerBase
	{
		private readonly INotesService  _service;

		public NotesController(INotesService ControllerService)
		{
			 _service = ControllerService;
		}

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			return Ok(await  _service.Get());
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			var result = await  _service.Get(id);

			if (result.IsSuccssfull)
			{
				return Ok(result.Data);
			}

			return NotFound(result);
		}

		[HttpPost]
		public async Task<IActionResult> Post([FromBody] NoteDto note)
		{
			var result = await  _service.Insert(note);

			if (result.IsSuccssfull)
			{
				return Ok(result.Data);
			}

			return BadRequest(result);
		}


		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete([FromRoute] int id)
		{
			var result = await  _service.Delete(id);

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
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] NoteDto note)
        {
            var result = await  _service.Update(note);

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