using Domain.IService;
using Infrastructure.Entities;
using Infrastructure.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Service
{
    public class HistoryService : IHistoryService
    {
        private readonly IRepository<History> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public HistoryService(IRepository<History> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<History>> GetListByPageWithOrderPropAndFilterSearchText(PagingListPropModel pagingListPropModel)
        {
            var histories = pagingListPropModel.SearchText.IsNullOrEmpty()
                                        ? await _repository.GetWithPredicateAndPagingListWithOrderPropAsync(pagingListPropModel).ConfigureAwait(true)
                                        : await _repository.GetWithPredicateAndPagingListWithOrderPropAsync(pagingListPropModel: pagingListPropModel, predicate: s => s.Action.ToString().Contains(pagingListPropModel.SearchText)).ConfigureAwait(true);
            return histories.ToList();
        }

        public int GetTotal(PagingListPropModel pagingListPropModel)
        {
            var total = pagingListPropModel.SearchText.IsNullOrEmpty() ? _repository.GetTotalEntity() : _repository.GetTotalEntity(predicate: s => s.Action.Contains(pagingListPropModel.SearchText));
            return total;
        }

        public async Task<IList<History>> GetByOrderId(Guid orderId)
        {
            var histories = await _repository.GetAllAsync(predicate: s => s.OrderId == orderId, include: s => s.Include(u => u.User), orderDesByCreate: true).ConfigureAwait(true);
            return histories.ToList();
        }

        public async Task<Guid> Add(History history)
        {
            history.Status = "Created";
            _repository.Add(history);
            await _unitOfWork.CommitChangesAsync();

            return history.Id;
        }

        public async Task<History> GetById(Guid id)
        {
            return await _repository.SingleOrDefaultAsync(predicate: s => s.Id == id).ConfigureAwait(true);
        }

        public async Task<History> Update(History historyUpdate)
        {
            var history = await _repository.SingleOrDefaultAsync(predicate: l => l.Id == historyUpdate.Id).ConfigureAwait(true);
            if (history == null)
            {
                return null;
            }

            history.UpdateDate = DateTime.UtcNow;
            history.IsActive = historyUpdate.IsActive;
            history.Status = historyUpdate.Status;

            history.Action = historyUpdate.Action;

            await _unitOfWork.CommitChangesAsync().ConfigureAwait(true);

            return history;
        }

        #region Do not use regularly

        public async Task<string> Remove(Guid id)
        {
            var history = await _repository.SingleOrDefaultAsync(predicate: l => l.Id == id).ConfigureAwait(true);

            if (history == null)
            {
                return null;
            }

            _repository.Remove(history);
            await _unitOfWork.CommitChangesAsync().ConfigureAwait(true);

            return "Success";
        }

        public async Task<IList<History>> GetAll(bool orderDesByCreate = false)
        {
            return _repository.GetAllAsync(orderDesByCreate: orderDesByCreate).Result.ToList();
        }

        #endregion
    }
}
