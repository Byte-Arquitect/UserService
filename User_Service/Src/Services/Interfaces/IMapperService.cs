using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User_Service.Src.Services.Interfaces
{
    public interface IMapperService
    {
        public TDestination Map<TSource, TDestination>(TSource source);

        public List<TDestination> MapList<TSource, TDestination>(List<TSource> sourceItems);
    }
}
