using System;
using Microsoft.Extensions.DependencyInjection;

namespace ScopedExample
{
    public interface IScopedService
    {
        void PrintId();
    }

    public class ScopedService : IScopedService
    {
        private readonly Guid _id;

        public ScopedService()
        {
            _id = Guid.NewGuid();
        }

        public void PrintId()
        {
            Console.WriteLine($"Scoped Service Id: {_id}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Создание коллекции сервисов
            var serviceProvider = new ServiceCollection()
                .AddScoped<IScopedService, ScopedService>()
                .BuildServiceProvider();

            // Получение фабрики областей
            var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            // Создание области
            using (var scope = scopeFactory.CreateScope())
            {
                // Получение сервиса из области
                var scopedService1 = scope.ServiceProvider.GetService<IScopedService>();
                var scopedService2 = scope.ServiceProvider.GetService<IScopedService>();

                // Вывод идентификаторов сервисов
                Console.WriteLine("Scoped Services in Scope 1:");
                scopedService1.PrintId();
                scopedService2.PrintId();
            }

            // Создание новой области
            using (var scope = scopeFactory.CreateScope())
            {
                // Получение сервиса из новой области
                var scopedService3 = scope.ServiceProvider.GetService<IScopedService>();
                var scopedService4 = scope.ServiceProvider.GetService<IScopedService>();

                // Вывод идентификаторов сервисов в новой области
                Console.WriteLine("\nScoped Services in Scope 2:");
                scopedService3.PrintId();
                scopedService4.PrintId();
            }
        }
    }
}