using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _NET_7_WEB_API___ENTITY_FRAMEWORK_JUMPSTART
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDto>();
            CreateMap<AddCharacterDto, Character>();
        }
    }
}