using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ImageGallery.API.Entities;

namespace ImageGallery.Api.Helpers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            // Map from Image (entity) to Image, and back
            CreateMap<Image, Model.Image>().ReverseMap();

            // Map from ImageForCreation to Image
            // Ignore properties that shouldn't be mapped
            CreateMap<Model.ImageForCreation, Image>()
                .ForMember(m => m.FileName, options => options.Ignore())
                .ForMember(m => m.Id, options => options.Ignore())
                .ForMember(m => m.OwnerId, options => options.Ignore());

            // Map from ImageForUpdate to Image
            // ignore properties that shouldn't be mapped
            CreateMap<Model.ImageForUpdate, Image>()
                .ForMember(m => m.FileName, options => options.Ignore())
                .ForMember(m => m.Id, options => options.Ignore())
                .ForMember(m => m.OwnerId, options => options.Ignore());
        }
    }
}
