using System;
using BubberBreakfast.Contracts.Breakfast;
using BubberBreakfast.Models;
using ErrorOr;

namespace BubberBreakfast.Services.Breakfasts
{
	public interface IBreakfastService
	{
		ErrorOr<Created> CreateBreakfast(Breakfast breakfast);
        ErrorOr<Deleted> DeleteBreakfast(Guid id);
        ErrorOr<Breakfast> GetBreakfast(Guid id);
        ErrorOr<UpsertedBreakfast> UpsertBreakfast(Breakfast breakfast);
    }
}

