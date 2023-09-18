using AutoMapper;
using Ordering.Application.Feastures.Orders.Commands.CheckoutOrder;
using Ordering.Application.Feastures.Orders.Commands.UpdateOrder;
using Ordering.Application.Feastures.Orders.Queries.GetOrdersList;
using Ordering.Domain.Entities;

namespace Ordering.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, OrdersVm>().ReverseMap();
            CreateMap<Order, CheckoutOrderCommand>().ReverseMap();
            CreateMap<Order, UpdateOrderCommand>().ReverseMap();
        }
    }
}
