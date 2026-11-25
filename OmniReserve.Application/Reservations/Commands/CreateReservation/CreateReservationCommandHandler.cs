using MediatR;
using OmniReserve.Application.Common.Interfaces;
using OmniReserve.Domain.Entities;

namespace OmniReserve.Application.Reservations.Commands.CreateReservation;

public record CreateReservationCommand(Guid UserId, Guid RoomId, DateTime StartDate, DateTime EndDate) : IRequest<Guid>;
public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, Guid>
{
    private readonly IReservationRepository _reservationRepository;

    public CreateReservationCommandHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<Guid> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        // 1. REGLA INVARIANTE: Validación de Disponibilidad Dependiente del Estado
        bool isAvailable = await _reservationRepository.IsRoomAvailableAsync(
            request.RoomId,
            request.StartDate,
            request.EndDate);

        if (!isAvailable)
        {
            // Rechaza la operación arrojando excepción.
            // En el futuro interceptaremos este error genérico con Middlewares.
            throw new Exception("La habitación seleccionada ya está ocupada en el rango de fechas solicitado.");
        }

        // 2. Creación y Persistencia (Solo se ejecuta si hay disponibilidad)
        var reservation = new Reservation(request.RoomId, request.UserId, request.StartDate, request.EndDate);

        await _reservationRepository.AddAsync(reservation);

        return reservation.Id;
    }
}