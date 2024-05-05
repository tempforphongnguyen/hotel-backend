using Domain.IService;
using Infrastructure.Entities;
using Infrastructure.Enum;
using Infrastructure.ViewModel;
using Infrastructure.ViewModel.Order;
using Infrastructure.ViewModel.OrderDetail;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Service
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IRepository<OrderDetail> _repository;
        private readonly IHistoryService _historyService;
        private readonly IAuthService _authService;
        private readonly IUnitOfWork _unitOfWork;

        public OrderDetailService(IRepository<OrderDetail> repository, IUnitOfWork unitOfWork, IHistoryService historyService, IAuthService authService)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _historyService = historyService;
            _authService = authService;
        }

        public async Task<IList<OrderDetail>> GetListByPageWithOrderPropAndFilterSearchText(PagingListPropModel pagingListPropModel)
        {
            var orderDetails = pagingListPropModel.SearchText.IsNullOrEmpty()
                                        ? await _repository.GetWithPredicateAndPagingListWithOrderPropAsync(pagingListPropModel).ConfigureAwait(true)
                                        : await _repository.GetWithPredicateAndPagingListWithOrderPropAsync(pagingListPropModel: pagingListPropModel, predicate: s => s.ProductName.Contains(pagingListPropModel.SearchText)).ConfigureAwait(true);
            return orderDetails.ToList();
        }
        public int GetTotal(PagingListPropModel pagingListPropModel)
        {
            var total = pagingListPropModel.SearchText.IsNullOrEmpty() ? _repository.GetTotalEntity() : _repository.GetTotalEntity(predicate: s => s.ProductName.Contains(pagingListPropModel.SearchText));
            return total;
        }

        public async Task<OrderDetail> GetById(Guid id)
        {
            return await _repository.SingleOrDefaultAsync(predicate: s => s.Id == id).ConfigureAwait(true);
        }

        public async Task<OrderDetailFormatVM> GetOrderDetailsByOrderId(Guid orderId, bool isOnlyRoom = false)
        {
            var orderDetails = isOnlyRoom ? await _repository.GetWithPredicateAsync(predicate: od => od.OrderId == orderId && od.IsRoom == true).ConfigureAwait(true)
                                        : await _repository.GetWithPredicateAsync(predicate: od => od.OrderId == orderId, include: od => od.Include(o => o.Order)).ConfigureAwait(true);

            var orderDetailRoom = orderDetails.FirstOrDefault(s => s.IsRoom == true);
            
            var roomInformation = new RoomInformation()
            {
                OriginalId = orderDetailRoom.OriginalId,
                ProductName = orderDetailRoom.ProductName,
                IsRoom = orderDetailRoom.IsRoom,
                Paid = orderDetailRoom.Paid,
                StartDate = orderDetailRoom.StartDate,
                OriginalRoomTypeId = orderDetailRoom.OriginalRoomTypeId,
                Quantity = orderDetailRoom.Quantity,
                RoomTypeName = orderDetailRoom.RoomTypeName,
                UnitPrice = orderDetailRoom.Price,
                Id = orderDetailRoom.Id,
                EndDate = orderDetailRoom.EndDate,
                MaxPerson = orderDetailRoom.MaxPerson
            };
            var userId = orderDetails.Select(s => s.Order).FirstOrDefault().UserId;

            var userInfo = await _authService.GetUserInfo(userId);

            var orderDetailFormat = new OrderDetailFormatVM()
            {
                OrderId = orderId,
                RoomInformation = roomInformation,
                UserInfo = userInfo
            };

            if (!isOnlyRoom)
            {
                var orderDetailExtras = orderDetails.Where(s => s.IsRoom == false).ToList();

                var extraServiceInfos = new List<ExtraServiceInformation>();

                foreach (var orderDetailExtra in orderDetailExtras)
                {
                    var extraServiceInfo = new ExtraServiceInformation()
                    {
                        Id = orderDetailExtra.Id,
                        OriginalId = orderDetailExtra.OriginalId,
                        Paid = orderDetailExtra.Paid,
                        Price = orderDetailExtra.Price,
                        ProductName = orderDetailExtra.ProductName,
                        Quantity = orderDetailExtra.Quantity,
                        PaymentNote = orderDetailExtra.PaymentNote,
                    };
                    extraServiceInfos.Add(extraServiceInfo);
                }

                orderDetailFormat.ExtraServiceInformations = extraServiceInfos;

                orderDetailFormat.Order = orderDetails.Select(s => s.Order).FirstOrDefault();
            }

            return orderDetailFormat;
        }

        public async Task AddList(IList<OrderDetail> orderDetails)
        {
            await _repository.AddListAsync(orderDetails);
            await _unitOfWork.CommitChangesAsync();
        }

        public async Task<string> AddListExtraService(List<BookExtraService> bookExtraService, Guid userId)
        {
            var orderDetails = new List<OrderDetail>();
            var orderId = bookExtraService.Select(s => s.OrderId).Distinct().SingleOrDefault();

            foreach(BookExtraService bookExtra in bookExtraService)
            {
                var orderDetail = new OrderDetail()
                {
                    CreateDate = DateTime.UtcNow,
                    IsActive = true,
                    OriginalId = bookExtra.ExtraServiceId,
                    OrderId = orderId,
                    IsRoom = false,
                    Paid = false,
                    Price = bookExtra.UnitPrice,
                    Quantity = bookExtra.Quantity,
                    ProductName = bookExtra.ExtraServiceName,
                    PaymentNote = ""
                };
                orderDetails.Add(orderDetail);
            }

            await _repository.AddListAsync(orderDetails);
            await _unitOfWork.CommitChangesAsync();

            var history = new History()
            {
                Action = HistoryActionEnum.AddService.ToString(),
                UserId = userId,
                OrderId = orderId,
                IsActive = true,
            };

            await _historyService.Add(history);

            return ResponseEnum.Success.ToString();
        }

        public async Task UpdateOrderDetailByOrderId (Guid orderId, string paymentNote, string orderStatus)
        {
            IEnumerable<OrderDetail> orderDetailsQuery = null;
            
            switch (Enum.Parse(typeof(StatusOrderEnum), orderStatus))
            {
                case StatusOrderEnum.New:
                    orderDetailsQuery = await _repository.GetAllAsync(predicate: s => s.OrderId == orderId).ConfigureAwait(true);
                    break;
                
                case StatusOrderEnum.Checkout:
                    orderDetailsQuery = await _repository.GetAllAsync(predicate: s => s.OrderId == orderId && s.Paid == false).ConfigureAwait(true);
                    break;

                default: break;
            }

            var orderDetailsToList = orderDetailsQuery?.ToList();
            
            foreach(var orderDetail in orderDetailsToList)
            {
                orderDetail.Paid = true;
                orderDetail.UpdateDate = DateTime.UtcNow;
                orderDetail.PaymentNote = paymentNote;
            }

            await _unitOfWork.CommitChangesAsync();
        }

        public async Task<Guid> GetRoomIdInOrderDetailByOrderId(Guid orderId)
        {
            var orderDetail = await _repository.SingleOrDefaultAsync(s=>s.OrderId == orderId && s.IsRoom == true).ConfigureAwait(true);
            return orderDetail.OriginalId;
        }

        #region

        public async Task<Guid> Add(OrderDetail orderDetail)
        {
            _repository.Add(orderDetail);
            await _unitOfWork.CommitChangesAsync();

            return orderDetail.Id;
        }


        public async Task<OrderDetail> Update(OrderDetail orderUpdate)
        {
            var orderDetail = await _repository.SingleOrDefaultAsync(predicate: l => l.Id == orderUpdate.Id).ConfigureAwait(true);
            if (orderDetail == null)
            {
                return null;
            }

            orderDetail.UpdateDate = DateTime.UtcNow;
            orderDetail.Status = orderUpdate.Status;
            orderDetail.IsActive = orderUpdate.IsActive;

            await _unitOfWork.CommitChangesAsync().ConfigureAwait(true);

            return orderDetail;
        }

        public async Task<IList<OrderDetail>> GetAll(bool orderDesByCreate = false)
        {
            var orderDetails = await _repository.GetAllAsync(orderDesByCreate: orderDesByCreate).ConfigureAwait(true);
            return orderDetails.ToList();
        }

        public async Task<string> Remove(Guid id)
        {
            var order = await _repository.SingleOrDefaultAsync(predicate: l => l.Id == id).ConfigureAwait(true);

            if (order == null)
            {
                return null;
            }

            _repository.Remove(order);
            await _unitOfWork.CommitChangesAsync().ConfigureAwait(true);

            return "Success";
        }

        #endregion
    }
}
