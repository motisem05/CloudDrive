using CloudDrive.Domain;
using CloudDrive.Domain.Entities;
using CloudDrive.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.CreditCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDrive.Services.CreditCards
{
    public interface ICreditCardsServices
    {
        Task<Result<CreditCardsDto>> GetAsync(int id);
        Task<Result<List<CreditCardsDto>>> GetAllAsync(int id);
        Task<Result<CreditCardsDto>> InsertAsync(UserCreditCard userCreditCard);
        Task<Result> DeleteAsync(int id);
        Task<Result<CreditCardsDto>> UpdateAsync(int id, CreditCardsDto userCreditCard);
    }

    public class UserCreditCards : ICreditCardsServices
    {
        //Connect the Database 
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<UserCreditCards> _logger;
        public UserCreditCards(AppDbContext appDbContext, ILogger<UserCreditCards> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }


        public async Task<Result<CreditCardsDto>> InsertAsync(UserCreditCard userCreditCard)
        {
            var transaction = _appDbContext.Database.BeginTransaction();
            try
            {
                _appDbContext.UserCreditCards.Add(userCreditCard);
                _logger.LogInformation("Inserted new Credit Card '{cardNumber}' with holder name '{holderName}'", userCreditCard.CreditCardNumber, userCreditCard.HolderName);
                await _appDbContext.SaveChangesAsync();
                transaction.Commit();
                return new Result<CreditCardsDto>
                {
                    IsSuccssfull = true,
                    Data = new CreditCardsDto(userCreditCard)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to Save Credit Card with Number '{CardNumber}' because of exceptions: '{Message}'", userCreditCard.CreditCardNumber, ex.Message);
                transaction.Rollback();
                return new Result<CreditCardsDto>
                {
                    IsSuccssfull = false,
                    Message = "Error while trying to save Credit Card due to technical reason with code " + ex.HResult
                };
            }

        }

        public async Task<Result> DeleteAsync(int id)
        {
            var transaction = _appDbContext.Database.BeginTransaction();
            try
            {
                var CardToDelete = _appDbContext.UserCreditCards.Find(id);
                if (CardToDelete != null)
                {
                    _appDbContext.UserCreditCards.Remove(CardToDelete);
                    _logger.LogInformation("Deleted Credit Card with id '{id}'", id);
                    await _appDbContext.SaveChangesAsync();
                    transaction.Commit();
                    return new Result
                    {
                        IsSuccssfull = true,
                        Message = "successfuly removed credit card # " + id + " from database"
                    };
                }
                else
                {
                    return new Result
                    {
                        IsSuccssfull = false,
                        Message = "card not found in database"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to delete Credit Card with id '{id}' because of exception '{message}'", id, ex.Message);
                transaction.Rollback();
                return new Result
                {
                    Message = "Error while trying to delete Credit Card due to technical reason with code: " + ex.HResult,
                    IsSuccssfull = false,
                };
            }


        }
        /*
         Gets Credit Card for one user by the Cards Id 
         */
        public async Task<Result<CreditCardsDto>> GetAsync(int id)
        {
            var userCredit = await _appDbContext.UserCreditCards.FindAsync(id);
            if (userCredit == null)
            {
                return new Result<CreditCardsDto>
                {
                    IsSuccssfull = false,
                    Message = "No Credit Card found !"
                };
            }
            else
            {
                return new Result<CreditCardsDto>
                {
                    IsSuccssfull = true,
                    Data = new CreditCardsDto(userCredit)
                };
            }
        }
        /*
         Gets All the Credit Cards for one user
         */
        public async Task<Result<List<CreditCardsDto>>> GetAllAsync(int id)
        {
            List<UserCreditCard> cards = await _appDbContext.UserCreditCards.Where(x => x.UserId == id).ToListAsync();
            if (cards.Count == 0 | cards == null)
            {
                return new Result<List<CreditCardsDto>>
                {
                    IsSuccssfull = false,
                    Message = "User Doesn't Have any cards !"
                };
            }
            List<CreditCardsDto> cardsDtos = new List<CreditCardsDto>();
            foreach (var Card in cards)
            {

                cardsDtos.Add(new CreditCardsDto(Card));
            }
            return new Result<List<CreditCardsDto>> { IsSuccssfull = true, Data = cardsDtos };
        }

        public async Task<Result<CreditCardsDto>> UpdateAsync(int id, CreditCardsDto userCreditCard)
        {
            var transaction = _appDbContext.Database.BeginTransaction();
            try
            {
                var card = await _appDbContext.UserCreditCards.FindAsync(id);
                if (card == null)
                {
                    _logger.LogError("Credit Card with id '{id}' was not found", id);
                    return new Result<CreditCardsDto> { IsSuccssfull = false, Message = "No Credit Card Found With id = " + id };
                }
                else
                {
                    card.CreditCardNumber = userCreditCard.CreditCardNumber;
                    card.ExpireMonth = userCreditCard.ExpireMonth;
                    card.ExpireYear = userCreditCard.ExpireYear;
                    card.HolderName = userCreditCard.HolderName;
                    _appDbContext.UserCreditCards.Update(card);
                    await _appDbContext.SaveChangesAsync();
                    _logger.LogInformation("Update Card id '{id}' with Card number '{number}' holder name '{name}'", id, card.CreditCardNumber, card.HolderName);
                    transaction.Commit();
                    var data = new CreditCardsDto(card);
                    return new Result<CreditCardsDto>
                    {
                        IsSuccssfull = true,
                        Data = data
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to Update Card id '{id}' because of exception: '{message}'", id, ex.HResult);
                transaction.Rollback();

                return new Result<CreditCardsDto>
                {
                    IsSuccssfull = false,
                    Message = ex.Message
                };
            }

        }
    }
}
