using Microsoft.Extensions.DependencyInjection;
using Vyshkat.ConsoleApp;
using Vyshkat.Domain.Interfaces;
using Vyshkat.Domain.Services;

var services = new ServiceCollection();

services.AddSingleton<ServiceCenter, CheckVehicle>();
services.AddSingleton<InventoryManager>();
services.AddSingleton<VehicleFactory>();
services.AddSingleton<ConsoleMenu>();

using ServiceProvider provider = services.BuildServiceProvider();
provider.GetRequiredService<ConsoleMenu>().Run();
