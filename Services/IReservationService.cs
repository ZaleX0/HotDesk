using HotDesk.DataTransferObjects;
using HotDesk.Models;

namespace HotDesk.Services;
public interface IReservationService
{
    void CreateReservation(CreateReservationDto dto);
    void DeleteReservation(int reservationId);
    CreateReservationViewModel GetCreateReservationViewModel(GetReservationInfoDto dto);
    ReservationViewModel GetReservationViewModel();
}