using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;

namespace Brixel.Iot.DeviceManager
{
    class Program
    {
        private const string ConnectionString = "";
        static RegistryManager _registryManager;
        static void Main(string[] args)
        {
            try
            {
                
                _registryManager = RegistryManager.CreateFromConnectionString(ConnectionString);
                System.Console.CancelKeyPress += (s, e) =>
                {
                    e.Cancel = true;
                    Console.WriteLine("Exiting...");
                };
                var input = "";
                while (String.Equals(input, String.Empty, StringComparison.Ordinal))
                {
                    Console.WriteLine();
                    Console.WriteLine("What do you want to do? Enter the number for the related action");
                    Console.WriteLine("List devices: 1");
                    Console.WriteLine("Create a device: 2");
                    input = Console.ReadLine();
                    if (input == "1")
                    {
                        ListDevices();
                    }
                    if (input == "2")
                    {
                        AddDeviceAsync().Wait();
                    }
                    input = "";
                }
            }
            catch (AggregateException ex)
            {
                foreach (Exception exception in ex.InnerExceptions)
                {
                    Console.WriteLine();
                    Console.WriteLine("Error in sample: {0}", exception);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Error in sample: {0}", ex.Message);
            }
        }

        private static void ListDevices()
        {
            Console.WriteLine("------------------------" + Environment.NewLine +
                              "Devices list for " + Environment.NewLine +
                              "Brixel IoT Hub " + Environment.NewLine +
                              "------------------------");
            var listOfDevices = _registryManager.GetDevicesAsync(200).Result;
            foreach (var device in listOfDevices)
            {
                Console.WriteLine($"Device {device.Id} is {device.ConnectionState}. It was last seen {device.LastActivityTime:dd-MM-yyyy HH:mm:ss}");
            }
        }

        private static async Task<Device> AddDeviceAsync()
        {
            Console.WriteLine("Please enter a device name");
            string deviceId = Console.ReadLine();
            Device device;
            try
            {
                device = await _registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                Console.WriteLine("Device name already exists");
                device = await _registryManager.GetDeviceAsync(deviceId);
            }
            Console.WriteLine("Generated device key: {0}", device.Authentication.SymmetricKey.PrimaryKey);
            return device;
            
        }
    }
}
