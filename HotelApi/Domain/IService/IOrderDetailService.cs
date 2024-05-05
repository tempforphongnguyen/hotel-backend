using Infrastructure.Entities;
using Infrastructure.ViewModel;
using Infrastructure.ViewModel.OrderDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IService
{
    public interface IOrderDetailService
    {
        Task<IList<OrderDetail>> GetListByPageWithOrderPropAndFilterSearchText(PagingListPropModel pagingListPropModel);
        Task<OrderDetailFormatVM> GetOrderDetailsByOrderId(Guid orderId, bool isOnlyRoom = false);
        Task<OrderDetail> GetById(Guid id);
        Task<Guid> Add(OrderDetail orderDetail);
        Task AddList(IList<OrderDetail> orderDetails);
        Task<string> AddListExtraService(List<BookExtraService> bookExtraService, Guid userId);
        Task<OrderDetail> Update(OrderDetail orderUpdate);
        Task UpdateOrderDetailByOrderId(Guid orderId, string paymentNote, string orderStatus);
        Task<Guid> GetRoomIdInOrderDetailByOrderId(Guid orderId);
        Task<IList<OrderDetail>> GetAll(bool orderDesByCreate = false);
        Task<string> Remove(Guid id);
        int GetTotal(PagingListPropModel pagingListPropModel);

    }
}
