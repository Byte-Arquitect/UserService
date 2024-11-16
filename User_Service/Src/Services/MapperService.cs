using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using User_Service.Src.Services.Interfaces;

namespace User_Service.Src.Services
{
    public class MapperService : IMapperService
    {
        private readonly IMapper _mapper;

        public MapperService(IMapper mapper)
        {
            _mapper = mapper;
        }
        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return _mapper.Map<TDestination>(source);
        }

        public List<TDestination> MapList<TSource, TDestination>(List<TSource> sourceItems)
        {
            var mappedObjects = sourceItems.Select(x => _mapper.Map<TDestination>(x)).ToList();
            return mappedObjects;
        }
    }
}