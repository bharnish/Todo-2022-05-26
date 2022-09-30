using AutoMapper;

namespace Todo.WebAPI.DTOs
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<Data.Todo, TodoDTO>();
        }
    }
}
