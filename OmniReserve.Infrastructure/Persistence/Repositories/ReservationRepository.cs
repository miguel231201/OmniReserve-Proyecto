using Microsoft.EntityFrameworkCore;
using OmniReserve.Application.Common.Interfaces;
using OmniReserve.Domain.Entities;

namespace OmniReserve.Infrastructure.Persistence.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly ApplicationDbContext _context;

    public ReservationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Reservation?> GetReservationWithDetailsAsync(Guid id)
    {
        // El orden de los Include no altera el resultado (LEFT JOINs)
        return await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Room)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
    public async Task<bool> IsRoomAvailableAsync(Guid roomId, DateTime start, DateTime end)
    {
        // Regla: Hay colisión si (ReservaExistente.Inicio < FinSolicitado) Y (ReservaExistente.Fin > InicioSolicitado)
        bool isOccupied = await _context.Reservations
            .AnyAsync(r => r.RoomId == roomId &&
                           r.StartDate < end &&
                           r.EndDate > start);

        // Si está ocupada, NO está disponible
        return !isOccupied;
    }
}