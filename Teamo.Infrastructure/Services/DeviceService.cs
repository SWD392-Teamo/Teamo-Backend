using Teamo.Core.Entities;
using Teamo.Core.Interfaces;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications.Devices;

namespace Teamo.Infrastructure.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IGenericRepository<UserDevice> _deviceRepository;

        public DeviceService(IGenericRepository<UserDevice> deviceRepository) 
        {
            _deviceRepository = deviceRepository;
        }
        public async Task<bool> AddDeviceAsync(UserDevice device)
        {
            var result = false;

            // Check if the device has been added before
            bool isExisted = await CheckExistedDevice(device.FCMToken);

            // If the device toke was saved before then skip the adding
            if (!isExisted)
            {
                _deviceRepository.Add(device);
                result = await _deviceRepository.SaveAllAsync();
            }
            else result = true;

            return result;
        }

        public async Task<List<string>> GetAllDeviceTokens()
        {
            var devices = await _deviceRepository.ListAllAsync();

            // Project to devices to a list of tokens
            var deviceTokens = devices.Select(d => d.FCMToken).ToList();

            return deviceTokens;
        }

        public async Task<List<string>> GetDeviceTokensForUser(int userId)
        {
            // Get all devices that the current user logged onto
            var spec = new DeviceSpecification(userId);
            var devices = await _deviceRepository.ListAsync(spec);

            // Project to devices to a list of tokens
            var deviceTokens = devices.Select(d => d.FCMToken).ToList();

            return deviceTokens;
        }

        private async Task<bool> CheckExistedDevice(string token)
        {
            var spec = new DeviceSpecification(token);
            var device = await _deviceRepository.GetEntityWithSpec(spec);
            return (device != null);
        }
    }
}
