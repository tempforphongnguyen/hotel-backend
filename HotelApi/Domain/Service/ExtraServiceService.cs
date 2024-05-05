using Domain.IService;
using Infrastructure.Entities;
using Infrastructure.Enum;
using Infrastructure.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Service
{
    public class ExtraServiceService : IExtraServiceService
    {
        private readonly IRepository<ExtraService> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ExtraServiceService(IRepository<ExtraService> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IList<ExtraService>> GetListByPageWithOrderPropAndFilterSearchText(PagingListPropModel pagingListPropModel)
        {
            var extraServices = pagingListPropModel.SearchText.IsNullOrEmpty()
                                        ? await _repository.GetWithPredicateAndPagingListWithOrderPropAsync(pagingListPropModel).ConfigureAwait(true)
                                        : await _repository.GetWithPredicateAndPagingListWithOrderPropAsync(pagingListPropModel: pagingListPropModel, predicate: s => s.Name.Contains(pagingListPropModel.SearchText)).ConfigureAwait(true);
            return extraServices.ToList();
        }

        public int GetTotal(PagingListPropModel pagingListPropModel)
        {
            var total = pagingListPropModel.SearchText.IsNullOrEmpty() ? _repository.GetTotalEntity() : _repository.GetTotalEntity(predicate: s => s.Name.Contains(pagingListPropModel.SearchText));
            return total;
        }
        public async Task<ExtraService> GetById(Guid id)
        {
            return await _repository.SingleOrDefaultAsync(predicate: s => s.Id == id).ConfigureAwait(true);
        }

        public async Task<Guid> Add(ExtraService extraService)
        {
            _repository.Add(extraService);
            await _unitOfWork.CommitChangesAsync();

            return extraService.Id;
        }

        public async Task<ExtraService> Update(ExtraService extraServiceUpdate)
        {
            var extraService = await _repository.SingleOrDefaultAsync(predicate: l => l.Id == extraServiceUpdate.Id).ConfigureAwait(true);
            if (extraService == null)
            {
                return null;
            }

            extraService.IsActive = extraServiceUpdate.IsActive;
            extraService.UpdateDate = DateTime.UtcNow;
            extraService.Status = extraServiceUpdate.Status;

            extraService.Description = extraServiceUpdate.Description;
            extraService.Name = extraServiceUpdate.Name;
            extraService.Price = extraServiceUpdate.Price;
            

            await _unitOfWork.CommitChangesAsync().ConfigureAwait(true);

            return extraService;
        }

        #region Do not use regularly
        public async Task<IList<ExtraService>> GetAll(bool orderDesByCreate = false)
        {
            return _repository.GetAllAsync(orderDesByCreate: orderDesByCreate).Result.ToList();
        }

        public async Task<string> Remove(Guid id)
        {
            var extraService = await _repository.SingleOrDefaultAsync(predicate: l => l.Id == id).ConfigureAwait(true);

            if (extraService == null)
            {
                return null;
            }

            _repository.Remove(extraService);
            await _unitOfWork.CommitChangesAsync().ConfigureAwait(true);

            return ResponseEnum.Success.ToString();
        }
        #endregion
    }
}
