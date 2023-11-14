using CloudDrive.Domain.Entities;
using CloudDrive.Services.CreditCards;
using Microsoft.AspNetCore.Mvc;
using Services.CreditCards;

namespace clouddrive.Controllers
{
    [Route("/api/UserCreditCards")]
    public class UserCreditCardsController : Controller
    {
        private readonly ICreditCardsServices _creditCardsService;
        public UserCreditCardsController(ICreditCardsServices creditCardsService)
        {
            _creditCardsService = creditCardsService;
        }

        [HttpGet("{id}")]
        /*
         * Get CreditCard for the user 
         */
        public async Task<IActionResult> GetCreditCard(int id)
        {
            var userCreditCard = await _creditCardsService.GetAsync(id);


            if (userCreditCard == null)
            {
                return NotFound(userCreditCard);
            }
            else
            {
                return Ok(userCreditCard);
            }

        }
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserCreditCards(int userId)
        {
            var userCreditCards = await _creditCardsService.GetAllAsync(userId);

            if(userCreditCards.IsSuccssfull)
            {
                return Ok(userCreditCards);
            }
            else
            {
                return NotFound(userCreditCards);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserCreditCard([FromBody] UserCreditCard userCreditCard)
        {
            var result = await _creditCardsService.InsertAsync(userCreditCard);

            if(result.IsSuccssfull)
            {
                return Ok(result);

            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserCreditCard(int id, [FromBody] CreditCardsDto userCreditCard)
        {
            var result = await _creditCardsService.UpdateAsync(id, userCreditCard);
            if(result.IsSuccssfull)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result); ;
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserCreditCard(int id)
        {
            var result = await _creditCardsService.DeleteAsync(id);
            if( result.IsSuccssfull)
            {

                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }


    }
}
