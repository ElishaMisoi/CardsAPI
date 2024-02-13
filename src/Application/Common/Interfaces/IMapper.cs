using AutoMapper;

namespace Application.Common.Interfaces
{
    public interface IMapper<T> where T : class
    {
        void Mapping(Profile profile);
    }
}
