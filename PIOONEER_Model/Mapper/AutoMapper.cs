﻿using AutoMapper;
using PIOONEER_Model.DTO;
using PIOONEER_Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Model.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserResponse>();
            CreateMap<UserRequest, User>();
            CreateMap<User, LoginResponse>();
            CreateMap<Product, ProductResponeDTO>();
            CreateMap<ProductAddDTO,Product>();
            CreateMap<ProductUpdateDto,Product>();
            CreateMap<ProductBUDTO, ProductByUser>();
            CreateMap<ProductByUser, ProductBUResponseDTO>();

            CreateMap<Order, OrderResponse>()
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails));
            CreateMap<OrderAddDTO, Order>();
            CreateMap<OrderUpDTO, Order>();
            CreateMap<Order, OrderUpdateResponse>();
            CreateMap<OrderResponse, EmailSendDTO>();
            CreateMap<userAndOrderAndOrderdetailsDTO, Order>();

            CreateMap<OrderDetails, OrderDetailsResponse>();
            CreateMap<OrderDetailsAddDTO, OrderDetails>();
            CreateMap<OrderDetailsUpDTO, OrderDetails>();
            CreateMap<userAndOrderAndOrderdetailsDTO, OrderDetails>();

            CreateMap<userAndOrderDTO, User>();
            CreateMap<userAndOrderDTO, Order>();
            CreateMap<userAndOrderDTO, OrderDetails>();
            CreateMap<userAndOrderAndOrderdetailsDTO, User>();

            CreateMap<Category, CategoryResponse>();
            CreateMap<CategoryRequest, Category>();

        }
    }
}
