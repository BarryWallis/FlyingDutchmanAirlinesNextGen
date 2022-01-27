using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using FlyingDutchmanAirlines.ControllerLayer.JsonData;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FlyingDutchmanAirlines.ControllerLayer;
public class BookingModelBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        _ = bindingContext ?? throw new ArgumentNullException(nameof(bindingContext));
        ReadResult readResult = await bindingContext.HttpContext.Request.BodyReader.ReadAsync();
        ReadOnlySequence<byte> buffer= readResult.Buffer;
        string body = Encoding.UTF8.GetString(buffer.FirstSpan);
        BookingData? bookingData = JsonSerializer.Deserialize<BookingData>(body);
        Debug.Assert(bookingData is not null);
        bindingContext.Result = ModelBindingResult.Success(bookingData);
    }
}
