using Domain.IService;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Infrastructure.ViewModel;
using Microsoft.IdentityModel.Tokens;
using Infrastructure.Enum;

namespace Domain.Service
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IRepository<Department> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService (IRepository<Department> repository, IUnitOfWork unitOfWork, IRoomService roomService)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<Department>> GetListByPageWithOrderPropAndFilterSearchText(PagingListPropModel pagingListPropModel)
        {
            var departments = pagingListPropModel.SearchText.IsNullOrEmpty()
                                        ? await _repository.GetWithPredicateAndPagingListWithOrderPropAsync(pagingListPropModel).ConfigureAwait(true)
                                        : await _repository.GetWithPredicateAndPagingListWithOrderPropAsync(pagingListPropModel: pagingListPropModel, predicate: s => s.Name.Contains(pagingListPropModel.SearchText)).ConfigureAwait(true);
            return departments.ToList();
        }

        public int GetTotal(PagingListPropModel pagingListPropModel)
        {
            var total = pagingListPropModel.SearchText.IsNullOrEmpty() ? _repository.GetTotalEntity() : _repository.GetTotalEntity(predicate: s => s.Name.Contains(pagingListPropModel.SearchText));
            return total;
        }

        public async Task<Department> GetById(Guid id)
        {
            return await _repository.SingleOrDefaultAsync(predicate: s => s.Id == id, include: d => d.Include(r => r.Rooms)).ConfigureAwait(true);
        }

        public async Task<Guid> Add(Department department)
        {
            _repository.Add(department);
            await _unitOfWork.CommitChangesAsync();

            return department.Id;
        }       

        public async Task<Department> Update(Department departmentUpdate)
        {
            var department = await _repository.SingleOrDefaultAsync(predicate: l => l.Id == departmentUpdate.Id).ConfigureAwait(true);
             if (department == null)
            {
                return null;
            }

            department.UpdateDate = DateTime.UtcNow;
            department.IsActive = departmentUpdate.IsActive;
            department.Status = departmentUpdate.Status;

            department.Location = departmentUpdate.Location;
            department.LocationLink = departmentUpdate.LocationLink;
            department.PhoneNumber = departmentUpdate.PhoneNumber;
            department.Name = departmentUpdate.Name;

            await _unitOfWork.CommitChangesAsync().ConfigureAwait(true);

            return department;
        }

        #region /*Do not use regularly*/
        public async Task<IList<Department>> GetAll(bool orderDesByCreate = false)
        {
            return _repository.GetAllAsync(orderDesByCreate: orderDesByCreate, include: d => d.Include(r => r.Rooms)).Result.ToList();
        }

        public async Task<string> Remove(Guid id)
        {
            var department = await _repository.SingleOrDefaultAsync(predicate: l => l.Id == id).ConfigureAwait(true);

            if (department == null)
            {
                return null;
            }

            _repository.Remove(department);
            await _unitOfWork.CommitChangesAsync().ConfigureAwait(true);

            return ResponseEnum.Success.ToString();
        }

        #endregion
    }
}
