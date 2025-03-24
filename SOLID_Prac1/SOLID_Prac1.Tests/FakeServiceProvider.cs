using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOLID_Prac1.Tests
{
    public class FakeServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, object> _services = new();

        public void Register<T>(T instance) => _services[typeof(T)] = instance!;

        public object? GetService(Type serviceType)
        {
            return _services.TryGetValue(serviceType, out var service)
                ? service : null;
        }
    }
}
