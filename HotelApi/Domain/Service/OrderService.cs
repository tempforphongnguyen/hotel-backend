using Domain.IService;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Infrastructure.ViewModel.RoomType;
using Infrastructure.Enum;
using Infrastructure.ViewModel.Order;
using Infrastructure.ViewModel;
using Microsoft.IdentityModel.Tokens;
using Infrastructure.ViewModel.Auth;
using System.Globalization;
using Infrastructure.ViewModel.OrderDetail;
using System.Linq.Expressions;
using Domain.Static;
using Amazon.RuntimeDependencies;
using Infrastructure.Mail;

namespace Domain.Service
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _repository;
        private readonly IRoomService _roomService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authenService;
        private readonly IHistoryService _historyService;
        private readonly INotificationService _notificationService;
        private readonly ISendMailService _sendMailService;

        public OrderService(IRepository<Order> repository
                            , IUnitOfWork unitOfWork
                            , IRoomService roomService
                            , IOrderDetailService orderDetailService
                            , IAuthService authenService
                            , IHistoryService historyService
                            , INotificationService notificationService
                            , ISendMailService sendMailService)
        {
            _repository = repository;
            _roomService = roomService;
            _unitOfWork = unitOfWork;
            _orderDetailService = orderDetailService;
            _authenService = authenService;
            _historyService = historyService;
            _notificationService = notificationService;
            _sendMailService = sendMailService;
        }

        public async Task<IList<Order>> GetListByPageWithOrderPropAndFilterSearchText(PagingListPropModel pagingListPropModel)
        {
            var orders = pagingListPropModel.SearchText.IsNullOrEmpty()
                                        ? await _repository.GetWithPredicateAndPagingListWithOrderPropAsync(pagingListPropModel).ConfigureAwait(true)
                                        : await _repository.GetWithPredicateAndPagingListWithOrderPropAsync(pagingListPropModel: pagingListPropModel, predicate: s => s.Id.ToString().ToLower().Contains(pagingListPropModel.SearchText.ToLower())).ConfigureAwait(true);
            return orders.ToList();
        }

        public async Task<IList<BookingHistoryVM>> GetListByPageWithOrderPropAndFilterSearchText(FilterOrderVM filterOrder)
        {
            Expression<Func<Order, bool>> mainCondition = null;
            Expression<Func<Order, bool>> userOrRoomCondition = null;

            if (!filterOrder.SearchEmailOrRoom.IsNullOrEmpty() && filterOrder.UserId == Guid.Empty)
            {
                userOrRoomCondition = s => s.User.Email.Contains(filterOrder.SearchEmailOrRoom) || s.OrderDetails.Any(od => od.IsRoom == true && od.ProductName.Contains(filterOrder.SearchEmailOrRoom));
                mainCondition = userOrRoomCondition;
            }

            if (filterOrder.UserId != Guid.Empty)
            {
                userOrRoomCondition = s => s.UserId == filterOrder.UserId;
                mainCondition = userOrRoomCondition;
            }

            Expression<Func<Order, bool>> statusCondition = null;
            if (!filterOrder.Status.IsNullOrEmpty() && filterOrder.Status.Count > 0)
            {
                statusCondition = s => filterOrder.Status.Contains(s.Status);

                if (mainCondition != null)
                {
                    mainCondition = HelperStaticService.And(mainCondition, statusCondition);
                }
                else
                {
                    mainCondition = statusCondition;
                }
            }

            Expression<Func<Order, bool>> departmentCondition = null;
            if (filterOrder.DepartmentId != Guid.Empty)
            {
                departmentCondition = s => s.DepartmentId == filterOrder.DepartmentId;

                if (mainCondition != null)
                {
                    mainCondition = HelperStaticService.And(mainCondition, departmentCondition);
                }
                else
                {
                    mainCondition = departmentCondition;
                }
            }

            if (filterOrder.IsReviewFilter)
            {
                Expression<Func<Order, bool>> reviewCondition = null;
                reviewCondition = filterOrder.IsReviewed ? s => s.Review != null : s => s.Review == null;

                if (mainCondition != null)
                {
                    mainCondition = HelperStaticService.And(mainCondition, reviewCondition);
                }
                else
                {
                    mainCondition = reviewCondition;
                }
            }

            var orders = mainCondition == null ? await _repository.GetWithPredicateAndPagingListWithOrderPropAsync(
                                                                        filterOrder.PagingListPropModel,
                                                                        include: s=>s.Include(t=>t.Review))
                                                                    .ConfigureAwait(true)
                                       : await _repository.GetWithPredicateAndPagingListWithOrderPropAsync(
                                                                pagingListPropModel: filterOrder.PagingListPropModel,
                                                                predicate: mainCondition,
                                                                include: s=>s.Include(t=>t.Review))
                                                            .ConfigureAwait(true);

            var bookingHistories = new List<BookingHistoryVM>();
            foreach(var order in orders)
            {
                var bookHistory = new BookingHistoryVM()
                {
                    Order = order
                };
                
                var orderDetail = await _orderDetailService.GetOrderDetailsByOrderId(order.Id, true);
                bookHistory.OrderDetail = orderDetail;
                
                bookingHistories.Add(bookHistory);
            }

            return bookingHistories;
        }

        public int GetTotal(PagingListPropModel pagingListPropModel)
        {
            var total = pagingListPropModel.SearchText.IsNullOrEmpty() ? _repository.GetTotalEntity() : _repository.GetTotalEntity(predicate: s => s.Note.Contains(pagingListPropModel.SearchText));
            return total;
        }

        public int GetTotal(FilterOrderVM filterOrder)
        {
            Expression<Func<Order, bool>> mainCondition = null;
            
            Expression<Func<Order, bool>> reviewCondition = null;
            reviewCondition = filterOrder.IsReviewed ? s => s.Review != null : s => s.Review == null;
            mainCondition = reviewCondition;


            Expression<Func<Order, bool>> emailOrRoomCondition = null;

            if (!filterOrder.SearchEmailOrRoom.IsNullOrEmpty())
            {
                emailOrRoomCondition = s => s.User.Email.Contains(filterOrder.SearchEmailOrRoom) || s.OrderDetails.Any(od => od.IsRoom == true && od.ProductName.Contains(filterOrder.SearchEmailOrRoom));
                mainCondition = HelperStaticService.And(mainCondition, emailOrRoomCondition); ;
            }

            Expression<Func<Order, bool>> statusCondition = null;
            if (!filterOrder.Status.IsNullOrEmpty() && filterOrder.Status.Count > 0)
            {
                statusCondition = s => filterOrder.Status.Contains(s.Status);
                mainCondition = HelperStaticService.And(mainCondition, statusCondition);
            }

            Expression<Func<Order, bool>> departmentCondition = null;
            if (filterOrder.DepartmentId != Guid.Empty)
            {
                departmentCondition = s => s.DepartmentId == filterOrder.DepartmentId;
                mainCondition = HelperStaticService.And(mainCondition, departmentCondition);
            }

            var total = mainCondition == null ? _repository.GetTotalEntity() : _repository.GetTotalEntity(predicate: mainCondition);

            return total;
        }

        public async Task<Order> GetById(Guid id)
        {
            return await _repository.SingleOrDefaultAsync(predicate: s => s.Id == id).ConfigureAwait(true);
        }

        public async Task<Guid> Add(Order order)
        {
            _repository.Add(order);
            await _unitOfWork.CommitChangesAsync();

            return order.Id;
        }

        public async Task<Order> Update(Order orderUpdate)
        {
            var order = await _repository.SingleOrDefaultAsync(predicate: l => l.Id == orderUpdate.Id).ConfigureAwait(true);
            if (order == null)
            {
                return null;
            }

            order.UpdateDate = DateTime.UtcNow;
            order.Status = orderUpdate.Status;
            order.IsActive = orderUpdate.IsActive;

            await _unitOfWork.CommitChangesAsync().ConfigureAwait(true);

            return order;
        }

        public async Task<string> UpdateStatusOrderAndPayment(UpdateOrderVM updateOrder)
        {
            var order = await _repository.SingleOrDefaultAsync(predicate: s => s.Id == updateOrder.OrderId).ConfigureAwait(true);

            order.UpdateDate = DateTime.UtcNow;

            var history = new History()
            {
                OrderId = order.Id,
                UserId = updateOrder.StaffUserId,
                Action = updateOrder.Status,
                IsActive = true
            };

            Guid roomId = Guid.Empty;

            switch (Enum.Parse(typeof(StatusOrderEnum), updateOrder.Status))
            {
                case StatusOrderEnum.New:
                    order.Status = StatusOrderEnum.New.ToString();
                    await _orderDetailService.UpdateOrderDetailByOrderId(updateOrder.OrderId, updateOrder.PaymentNote, updateOrder.Status);
                    await _notificationService.AutoCreateWithType(CreateNotiTypeEnum.OrderStatus, "Successful booking", order.UserId);
                    await BodyMailSuccessBooking(order);
                    break;

                case StatusOrderEnum.Checkin:
                    order.Status = updateOrder.Status;
                    await _historyService.Add(history);
                    roomId = await _orderDetailService.GetRoomIdInOrderDetailByOrderId(updateOrder.OrderId).ConfigureAwait(true);
                    await _roomService.UpdateStatusToOrder(roomId, StatusRoomEnum.NotAvailable.ToString());
                    await _notificationService.AutoCreateWithType(CreateNotiTypeEnum.OrderStatus, StatusOrderEnum.Checkin.ToString(), order.UserId);
                    break;

                case StatusOrderEnum.Checkout:
                    order.Status = updateOrder.Status;
                    
                    await _authenService.UpdateUserMerit(order.UserId, order.TotalPrice);
                    
                    await _orderDetailService.UpdateOrderDetailByOrderId(updateOrder.OrderId, updateOrder.PaymentNote, updateOrder.Status);
                    roomId = await _orderDetailService.GetRoomIdInOrderDetailByOrderId(updateOrder.OrderId).ConfigureAwait(true);
                    
                    await _roomService.UpdateStatusToOrder(roomId, StatusRoomEnum.Available.ToString());
                    
                    await _notificationService.AutoCreateWithType(CreateNotiTypeEnum.OrderStatus, StatusOrderEnum.Checkout.ToString(), order.UserId);
                    
                    await _historyService.Add(history);
                    break;

                case StatusOrderEnum.Delete:
                    order.Status = updateOrder.Status;
                    break;

                default: break;
            }

            await _unitOfWork.CommitChangesAsync();

            return ResponseEnum.Success.ToString();
        }
        
        public async Task<Guid> AddNewOrderForBooking(CreateOrderVM createOrder)
        {
            await UpdateOrderWaitingPayment();

            //Get room 
            createOrder.BookingRoom.EndDate = DateTime.Parse(createOrder.BookingRoom.EndDate.ToString(), null, DateTimeStyles.AdjustToUniversal).AddHours(12);
            createOrder.BookingRoom.StartDate = DateTime.Parse(createOrder.BookingRoom.StartDate.ToString(), null, DateTimeStyles.AdjustToUniversal).AddHours(14);

            var filterRoom = new FilterRoom()
            {
                DepartmentId = createOrder.BookingRoom.DepartmentId,
                EndDate = createOrder.BookingRoom.EndDate,
                StartDate = DateTime.Parse(createOrder.BookingRoom.StartDate.ToString(), null, DateTimeStyles.AdjustToUniversal).AddHours(14),
                MaxPerson = createOrder.BookingRoom.MaxPerson,
                RoomTypeId = createOrder.BookingRoom.RoomTypeId,
            };

            var roomAvailable = await _roomService.GetRoomAvailableInDepartmentForBooking(filterRoom);

            if (roomAvailable == null)
            {
                return Guid.Empty;
            }

            // Create order
            var newOrder = new Order()
            {
                CreateDate = DateTime.UtcNow,
                Status = StatusOrderEnum.WaitingPayment.ToString(),
                UserId = createOrder.UserId,
                Discount = createOrder.Discount,
                TotalPrice = createOrder.TotalPrice,
                Note = createOrder.Note,
                IsActive = true,
                DepartmentId = createOrder.BookingRoom.DepartmentId
            };

            _repository.Add(newOrder);
            await _unitOfWork.CommitChangesAsync();

            //Create orderDetails
            var orderDetails = new List<OrderDetail>()
            {
                new OrderDetail()
                {
                    CreateDate = DateTime.UtcNow,
                    IsActive = true,
                    IsRoom = true,
                    OriginalId = roomAvailable.Id,
                    Paid = createOrder.Paid,
                    EndDate = createOrder.BookingRoom.EndDate,
                    StartDate = createOrder.BookingRoom.StartDate,
                    Price = createOrder.BookingRoom.UnitRoomTypePrice,
                    OriginalRoomTypeId = createOrder.BookingRoom.RoomTypeId,
                    ProductName = roomAvailable.Name,
                    OrderId = newOrder.Id,
                    Quantity = createOrder.BookingRoom.Quantity,
                    RoomTypeName =   createOrder.BookingRoom.RoomTypeName,
                    PaymentNote = createOrder.PaymentNote,
                    MaxPerson = createOrder.BookingRoom.MaxPerson,
                }
            };
            
            foreach (var orderDetailService in createOrder.BookExtraService)
            {
                var orderDetail = new OrderDetail()
                {
                    CreateDate = DateTime.UtcNow,
                    IsActive = true,
                    IsRoom = false,
                    OriginalId = orderDetailService.ExtraServiceId,
                    Paid = createOrder.Paid,
                    Price = orderDetailService.UnitPrice,
                    ProductName = orderDetailService.ExtraServiceName,
                    OrderId = newOrder.Id,
                    Quantity = orderDetailService.Quantity,
                    PaymentNote = createOrder.PaymentNote
                };
                orderDetails.Add(orderDetail);
            }

            await _orderDetailService.AddList(orderDetails);

            return newOrder.Id;
        }

        public async Task UpdateOrderWaitingPayment()
        {
            var expPaymentTime = DateTime.UtcNow.AddMinutes(-15);

            var expPaymentOrders = await _repository.GetAllAsync(predicate: o => o.Status == StatusOrderEnum.WaitingPayment.ToString() && o.CreateDate < expPaymentTime).ConfigureAwait(true);
            var orders = expPaymentOrders.ToList();

            if (!orders.IsNullOrEmpty())
            {
                foreach (var order in orders)
                {
                    order.Status = StatusOrderEnum.Delete.ToString();
                    order.UpdateDate = DateTime.UtcNow;
                }

                await _unitOfWork.CommitChangesAsync();
            }
        }

        public async Task UpdateOrderExpCheckin()
        {
            var expCheckinTime = DateTime.UtcNow;
            var expCheckinOrder = await _repository.GetAllAsync(predicate: o => o.Status == StatusOrderEnum.New.ToString() && o.OrderDetails.Any(od=>od.IsRoom == true && od.EndDate < expCheckinTime)).ConfigureAwait(true);
            
            var orders = expCheckinOrder.ToList();
            if (!orders.IsNullOrEmpty())
            {
                foreach (var order in orders)
                {
                    order.Status = StatusOrderEnum.Checkout.ToString();
                    order.UpdateDate = DateTime.UtcNow;
                    await _notificationService.AutoCreateWithType(CreateNotiTypeEnum.OrderStatus, "Exp checkin", order.UserId);
                    await _authenService.UpdateUserMerit(order.UserId, order.TotalPrice);
                }
            }
            
            await _unitOfWork.CommitChangesAsync();
        }

        public async Task<IList<ReportResponseVM>> GetTotalOrderByDateTime(ReportOrderRequest reportOrderRequest)
        {
            var result = new List<ReportResponseVM>();

            Expression<Func<Order, bool>> departmentCondition = null;

            if (reportOrderRequest.DepartmentId != Guid.Empty)
            {
                departmentCondition = s => s.DepartmentId == reportOrderRequest.DepartmentId;
            }

            foreach (var dateUnit in reportOrderRequest.DateUnits)
            {
                var startDateFormat =  DateTime.Parse(dateUnit.StartDate.ToString(), null, DateTimeStyles.AdjustToUniversal).Date;
                var endDateFormat = DateTime.Parse(dateUnit.EndDate.ToString(), null, DateTimeStyles.AdjustToUniversal).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                Expression<Func<Order, bool>> mainCondition = s => s.Status == StatusOrderEnum.Checkout.ToString() &&
                                                                (s.UpdateDate <= endDateFormat && s.UpdateDate >= startDateFormat);

                mainCondition = departmentCondition == null ? mainCondition : HelperStaticService.And(mainCondition, departmentCondition);

                var totalPriceByDateUnit = await _repository.ReportOrder(predicate: mainCondition)
                                                            .ConfigureAwait(true);
                
                result.Add(new ReportResponseVM
                {
                    DateUnits = dateUnit.StartDate.ToShortDateString() + "-" + dateUnit.EndDate.ToShortDateString(),
                    Total = totalPriceByDateUnit
                });
            }

            return result;
        }

        public async Task<string> UpdateTotalPrice(Guid orderId, IList<BookExtraService> bookExtraServices)
        {
            var order = await _repository.SingleOrDefaultAsync(predicate: s => s.Id == orderId).ConfigureAwait(true);

            order.TotalPrice = bookExtraServices.Select(s => s.TotalPrice).FirstOrDefault();


            await _unitOfWork.CommitChangesAsync().ConfigureAwait(true);

            return ResponseEnum.Success.ToString();
        }

        private async Task BodyMailSuccessBooking(Order order)
        {
            var link = "https://quintessential-hotels.vercel.app/en/dashboard/user/booking-history";
            var user = await _authenService.GetUserInfo(order.UserId);

            var orderDetails = await _orderDetailService.GetOrderDetailsByOrderId(order.Id);

            string bodyGreeting = "Hi " + user.FirstName +" "+ user.LastName + "<br>";
            string bodyDetail = "Your booking code for room "+ "<strong>" + orderDetails.RoomInformation.RoomTypeName + " - " + orderDetails.RoomInformation.ProductName+ "</strong> is:  <strong>" + order.Id + "</strong>" + "<br>";
            string bodyNote = "You can view deatil in: " + "https://quintessential-hotels.vercel.app/en/dashboard/user/booking-history" + "<br>";
            string bodyThank = "Please show the code for Staff when you checkin.<br> Thanks for your booking.";


            var mailRequest = new MailRequest()
            {
                ToEmail = user.Email,
                Subject = "Booking Success",
                Body = bodyGreeting + bodyDetail + bodyNote + bodyThank
            };

            await _sendMailService.SendEmailWithoutAttachmentAsync(mailRequest);

        }

        #region

        public async Task<IList<Order>> GetAll(bool orderDesByCreate = false)
        {
            var orders = await _repository.GetAllAsync(orderDesByCreate: orderDesByCreate).ConfigureAwait(true);
            return orders.ToList();
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
