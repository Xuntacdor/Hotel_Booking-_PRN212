using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer
{
    public class ServiceDAO
    {
        // Get all services
        public List<Service> GetAllServices()
        {
            using (var context = new FuminiHotelProjectPrn212Context())
            {
                return context.Services.ToList();
            }
        }

        // Search services by name
        public List<Service> SearchServices(string name)
        {
            using (var context = new FuminiHotelProjectPrn212Context())
            {
                return context.Services
                    .Where(s => s.ServiceName.Contains(name))
                    .ToList();
            }
        }

        // Add new service
        public void AddService(Service service)
        {
            using (var context = new FuminiHotelProjectPrn212Context())
            {
                context.Services.Add(service);
                context.SaveChanges();
            }
        }

        // Update service
        public void UpdateService(Service service)
        {
            using (var context = new FuminiHotelProjectPrn212Context())
            {
                context.Entry(service).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        // Delete service
        public void DeleteService(int serviceId)
        {
            using (var context = new FuminiHotelProjectPrn212Context())
            {
                var service = context.Services.Find(serviceId);
                if (service != null)
                {
                    context.Services.Remove(service);
                    context.SaveChanges();
                }
            }
        }

        // Get service by ID
        public Service GetServiceById(int serviceId)
        {
            using (var context = new FuminiHotelProjectPrn212Context())
            {
                return context.Services.Find(serviceId);
            }
        }
    }
}