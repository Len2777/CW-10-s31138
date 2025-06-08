using CW10.DTOs;
using CW10.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CW10.Services;

public interface IDbService
{
    Task<object> GetTrips(int page, int pageSize);
    Task<IActionResult> DeleteClient(int id);
    Task<IActionResult> AssignClientToTrip(int idTrip, AssignClientDto dto);
}

public class DbService(MasterContext context) : IDbService

{


    public async Task<object> GetTrips(int page, int pageSize)
    {
        var totalCount = await context.Trips.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var trips = await context.Trips.Select(trip => new TripDto
            {
                Name = trip.Name,
                Description = trip.Description,
                DateFrom = trip.DateFrom,
                DateTo = trip.DateTo,
                MaxPeople = trip.MaxPeople,
                Countries = trip.IdCountries.Select(cnt => new Country()
                {
                    Name = cnt.Name
                }).ToList(),
                Clients = trip.ClientTrips.Select(ct => new Client()
                {
                    FirstName = ct.IdClientNavigation.FirstName,
                    LastName = ct.IdClientNavigation.LastName
                }).ToList()
            })
            .OrderByDescending(trip => trip.DateFrom)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new
        {
            pageNum = page,
            pageSize,
            allPages = totalPages,
            trips
        };
    }

    public async Task<IActionResult> DeleteClient(int id)
    {
        var client = await context.Clients
            .Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(c => c.IdClient == id);

        if (client == null)
            return new NotFoundObjectResult("Client not found");

        if (client.ClientTrips.Any())
            return new BadRequestObjectResult("Client is assigned to a trip");

        context.Clients.Remove(client);
        await context.SaveChangesAsync();

        return new NoContentResult();
    }

    public async Task<IActionResult> AssignClientToTrip(int idTrip, AssignClientDto dto)
    {
        
        var trip = await context.Trips
            .Include(t => t.ClientTrips)
            .FirstOrDefaultAsync(t => t.IdTrip == idTrip);

        if (trip == null)
            return new BadRequestObjectResult("Trip not found");

        if (trip.DateFrom <= DateTime.Now)
            return new BadRequestObjectResult("Trip has already started");

        
        var existingClient = await context.Clients
            .FirstOrDefaultAsync(c => c.Pesel == dto.Pesel);

        
        if (existingClient != null)
        {
            bool isAlreadyAssigned = await context.ClientTrips
                .AnyAsync(ct => ct.IdClient == existingClient.IdClient && ct.IdTrip == idTrip);

            if (isAlreadyAssigned)
                return new BadRequestObjectResult("Client already assigned to this trip");
        }

       
        if (trip.ClientTrips.Count >= trip.MaxPeople)
            return new BadRequestObjectResult("Trip is full");

      
        var client = existingClient ?? new Client
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Telephone = dto.Telephone,
            Pesel = dto.Pesel
        };

        if (existingClient == null)
        {
            context.Clients.Add(client);
            await context.SaveChangesAsync();  
        }

        
        var clientTrip = new ClientTrip
        {
            IdClient = client.IdClient,
            IdTrip = idTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = dto.PaymentDate
        };

        context.ClientTrips.Add(clientTrip);
        await context.SaveChangesAsync();

        return new OkResult();
    }
}