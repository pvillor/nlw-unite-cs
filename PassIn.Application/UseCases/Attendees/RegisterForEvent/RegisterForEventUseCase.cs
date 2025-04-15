using System.Net.Mail;
using PassIn.Communication.Requests;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using PassIn.Infrastructure.Entities;

namespace PassIn.Application.UseCases.Attendees.RegisterForEvent;

public class RegisterForEventUseCase
{
    private readonly PassInDbContext _dbContext;

    public RegisterForEventUseCase()
    {
        _dbContext = new PassInDbContext();
    }

    public ResponseRegisteredJson Execute(Guid eventId, RequestRegisterEventJson request)
    {
        Validate(eventId, request);

        var entity = new Attendee
        {
            Name = request.Name,
            Email = request.Email,
            Event_Id = eventId,
            Created_At = DateTime.UtcNow,
        };

        _dbContext.Attendees.Add(entity);
        _dbContext.SaveChanges();

        return new ResponseRegisteredJson
        {
            Id = entity.Id,
        };
    }

    private void Validate(Guid eventId, RequestRegisterEventJson request)
    {
        var eventEntity = _dbContext.Events.Find(eventId);

        if (eventEntity is null)
        {
            throw new NotFoundException("An event with this id does not exist.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ErrorOnValidationException("Invalid name.");
        }

        var isEmailValid = EmailIsValid(request.Email);

        if (isEmailValid == false)
        {
            throw new ErrorOnValidationException("Invalid e-mail.");
        }

        var isAttendeeAlreadyRegistered = _dbContext
            .Attendees
            .Any(attendee => attendee.Email.Equals(request.Email) && attendee.Event_Id == eventId);

        if (isAttendeeAlreadyRegistered)
        {
            throw new ConflictException("You cannot register twice for the same event.");
        }

        var eventAttendeesCount = _dbContext.Attendees.Count(attendee => attendee.Event_Id == eventId);

        if (eventAttendeesCount == eventEntity.Maximum_Attendees)
        {
            throw new ErrorOnValidationException("Maximum attendees reached.");
        }
    }

    private static bool EmailIsValid(string email)
    {
        try
        {
            _ = new MailAddress(email);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
