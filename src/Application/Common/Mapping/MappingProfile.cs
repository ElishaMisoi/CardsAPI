using Application.Common.Interfaces;
using AutoMapper;
using System.Reflection;

namespace Application.Common.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                .Where(t => t
                    .GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapper<>)));

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);

                var iMapMethodInfo = type.GetMethod("Mapping");

                iMapMethodInfo?.Invoke(instance, new object[] { this });
            }
        }
    }
}
