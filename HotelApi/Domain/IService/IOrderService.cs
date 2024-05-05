using Infrastructure.Entities;
using Infrastructure.ViewModel;
using Infrastructure.ViewModel.Order;
using Infrastructure.ViewModel.OrderDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IService
{
    public interface IOrderService
    {
        Task<Guid> Add(Order order);
        Task<IList<Order>> GetAll(bool orderDesByCreate = false);
        Task<Order> GetById(Guid id);
        Task<Order> Update(Order orderUpdate);
        Task<string> Remove(Guid id);
        Task<IList<Order>> GetListByPageWithOrderPropAndFilterSearchText(PagingListPropModel pagingListPropModel);
        Task<Guid> AddNewOrderForBooking(CreateOrderVM createOrder);
        int GetTotal(PagingListPropModel pagingListPropModel);
        Task<string> UpdateTotalPrice(Guid orderId, IList<BookExtraService> bookExtraServices);
        Task UpdateOrderWaitingPayment();
        Task UpdateOrderExpCheckin();
        Task<string> UpdateStatusOrderAndPayment(UpdateOrderVM updateOrder);
        Task<IList<BookingHistoryVM>> GetListByPageWithOrderPropAndFilterSearchText(FilterOrderVM filterOrder);
        int GetTotal(FilterOrderVM filterOrder);
        Task<IList<ReportResponseVM>> GetTotalOrderByDateTime(ReportOrderRequest reportOrderRequest);
    }
}
