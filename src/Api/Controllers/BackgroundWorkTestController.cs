using CloudDrive.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloudDrive.Api.Controllers
{
	[ApiController, Route("/api/test")]
	public class BackgroundWorkTestController : ControllerBase
	{
		private readonly BackgroundWorkService _backgroundWorkSrevice;

		public BackgroundWorkTestController(
			BackgroundWorkService backgroundWorkSrevice
		)
		{
			_backgroundWorkSrevice = backgroundWorkSrevice;
		}

		[HttpGet("{work}")]
		public IActionResult Get([FromRoute] string work)
		{
			_backgroundWorkSrevice.AddWork(() =>
			{
				Console.WriteLine("Work sent from controller to background service with work: " + work);
			});

			return Ok();
		}
	}
}