using Infrastructure.Entities;
using Infrastructure.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IService
{
    public interface IDepartmentService
    {
        Task<Guid> Add(Department department);
        Task<IList<Department>> GetAll(bool orderDesByCreate = false);
        Task<Department> GetById(Guid id);
        Task<Department> Update(Department departmentUpdate);
        Task<string> Remove(Guid id);
        Task<IList<Department>> GetListByPageWithOrderPropAndFilterSearchText(PagingListPropModel pagingListPropModel);
        int GetTotal(PagingListPropModel pagingListPropModel);
    }
}
