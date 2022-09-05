using System;
using System.ComponentModel.DataAnnotations;
using BubberBreakfast.Contracts.Breakfast;
using BubberBreakfast.Services;
using ErrorOr;

namespace BubberBreakfast.Models
{
	public class Breakfast
    {
        public const int MIN_NAME_LENGTH = 3;
        public const int MAX_NAME_LENGTH = 50;
        public const int MIN_DESC_LENGTH = 5;
        public const int MAX_DESC_LENGTH = 250;

        public Guid Id { get; }

        [MaxLength(50), MinLength(3)]
        public string Name { get; }

        [MaxLength(250), MinLength(5)]
        public string Description { get; }

        public DateTime StartDateTime { get; }
        public DateTime EndDateTime { get; }
        public DateTime LastModifiedDateTime { get; }
        public List<string> Savory { get; }
        public List<string> Sweet { get; }

        public Breakfast(Guid id, 
                         string name,                
                         string description,
                         DateTime startDateTime,
                         DateTime endDateTime,
                         DateTime lastModifiedDateTime,
                         List<string> savory,
                         List<string> sweet)
        {
            Id = id;
            Name = name;
            Description = description;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            LastModifiedDateTime = lastModifiedDateTime;
            Savory = savory;
            Sweet = sweet;
        }

        public static ErrorOr<Breakfast> Create(
                             string name,
                             string description,
                             DateTime startDateTime,
                             DateTime endDateTime,
                             List<string> savory,
                             List<string> sweet,
                             Guid? id = null)
        {
            List<Error> errors = new();

            if (name.Length is < MIN_NAME_LENGTH or > MAX_NAME_LENGTH)
            {
                errors.Add(Errors.Breakfast.InvalidName);
            }

            if (description.Length is < MIN_DESC_LENGTH or > MAX_DESC_LENGTH)
            {
                errors.Add(Errors.Breakfast.InvalidDescription);
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            return new Breakfast(
                id ?? Guid.NewGuid(),
                name,
                description,
                startDateTime,
                endDateTime,
                DateTime.UtcNow,
                savory,
                sweet);
        }

        public static ErrorOr<Breakfast> From(CreateBreakfastRequest request)
        {
            return Create(
                request.Name,
                request.Description,
                request.StartDateTime,
                request.EndDateTime,
                request.Savory,
                request.Sweet);
        }

        public static ErrorOr<Breakfast> From(Guid id, UpsertBreakfastRequest request)
        {
            return Create(
                request.Name,
                request.Description,
                request.StartDateTime,
                request.EndDateTime,
                request.Savory,
                request.Sweet,
                id);
        }
    }
}

