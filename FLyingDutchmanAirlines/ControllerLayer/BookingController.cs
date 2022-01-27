using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using FlyingDutchmanAirlines.ControllerLayer.JsonData;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.ServiceLayer;

using Microsoft.AspNetCore.Mvc;

namespace FlyingDutchmanAirlines.ControllerLayer;

[Route("{controller}")]
public class BookingController : Controller
{
    private readonly BookingService _bookingService;

    public BookingController(BookingService bookingService) => _bookingService = bookingService;

    [HttpPost("{flightNumber}")]
    public async Task<IActionResult> CreateBookingAsync([FromBody] /*[ModelBinder(typeof(BookingModelBinder))]*/ BookingData bookingData,
                                                        int flightNumber)
    {
        if (!ModelState.IsValid && flightNumber.IsPositive())
        {
            string fullName = $"{bookingData.FirstName} {bookingData.LastName}";
            Exception? exception = await _bookingService.CreateBookingAsync(fullName, flightNumber);
            return exception is null
                ? StatusCode((int)HttpStatusCode.Created)
                : exception is CouldNotAddBookingToDatabaseException 
                    ? StatusCode((int)HttpStatusCode.NotFound) 
                    : StatusCode((int)HttpStatusCode.InternalServerError, exception.Message);
            ;
        }

        return StatusCode((int)HttpStatusCode.InternalServerError, ModelState.Root.Errors.First().ErrorMessage);
    }
}
