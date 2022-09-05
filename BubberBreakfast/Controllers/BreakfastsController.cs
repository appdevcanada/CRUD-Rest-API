using static BubberBreakfast.Services.Errors;
using Breakfast = BubberBreakfast.Models.Breakfast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BubberBreakfast.Contracts.Breakfast;
using BubberBreakfast.Models;
using BubberBreakfast.Services;
using BubberBreakfast.Services.Breakfasts;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace BubberBreakfast.Controllers
{
    public class BreakfastsController : ApiController
    {
        private readonly IBreakfastService _breakfastService;

        public BreakfastsController(IBreakfastService breakfastService)
        {
            _breakfastService = breakfastService;
        }

        [HttpPost]
        public IActionResult CreateBreakfast(CreateBreakfastRequest request)
        {
            ErrorOr<Breakfast> requestToBreakfastResult = Breakfast.From(request);

            if (requestToBreakfastResult.IsError)
            {
                return Problem(requestToBreakfastResult.Errors);
            }

            var breakfast = requestToBreakfastResult.Value;
            ErrorOr<Created> createdBreakfastResult = _breakfastService.CreateBreakfast(breakfast);

            if (createdBreakfastResult.IsError)
            {
                return Problem(createdBreakfastResult.Errors);
            }

            return createdBreakfastResult.Match(
                created => CreatedAtGetBreakfast(breakfast),
                errors => Problem(errors)
                );
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetBreakfast(Guid id)
        {
            ErrorOr<Breakfast> getBreakfastResult = _breakfastService.GetBreakfast(id);

            return getBreakfastResult.Match(
                breakfast => Ok(MapBreakfastResponse(breakfast)),
                errors => Problem(errors)
                );
        }

        [HttpPut("{id:guid}")]
        public IActionResult UpsertBreakfast(Guid id, UpsertBreakfastRequest request)
        {
            ErrorOr<Breakfast> requestToBreakfastResult = Breakfast.From(id, request);

            if (requestToBreakfastResult.IsError)
            {
                return Problem(requestToBreakfastResult.Errors);
            }

            var breakfast = requestToBreakfastResult.Value;
            ErrorOr<UpsertedBreakfast> upsertedBreakfastResult = _breakfastService.UpsertBreakfast(breakfast);

            return upsertedBreakfastResult.Match(
                upserted => upserted.IsNewlyCreated ? CreatedAtGetBreakfast(breakfast) : NoContent(),
                errors => Problem(errors)
                );
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteBreakfast(Guid id)
        {
            ErrorOr<Deleted> deletedBreakfastResult = _breakfastService.DeleteBreakfast(id);

            return deletedBreakfastResult.Match(
                deleted => NoContent(),
                errors => Problem(errors)
                );
        }

        private static BreakfastResponse MapBreakfastResponse(Breakfast breakfast)
        {
            return new BreakfastResponse(
                breakfast.Id,
                breakfast.Name,
                breakfast.Description,
                breakfast.StartDateTime,
                breakfast.EndDateTime,
                breakfast.LastModifiedDateTime,
                breakfast.Savory,
                breakfast.Sweet);
        }

        private CreatedAtActionResult CreatedAtGetBreakfast(Breakfast breakfast)
        {
            return CreatedAtAction(
                nameof(GetBreakfast),
                new { id = breakfast.Id },
                MapBreakfastResponse(breakfast)
                );
        }
    }
}

