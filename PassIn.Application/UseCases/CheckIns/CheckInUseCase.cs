using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using PassIn.Infrastructure.Entities;

namespace PassIn.Application.UseCases.CheckIns;

public class CheckInUseCase 
{
    private readonly PassInDbContext _dbContext;

    public CheckInUseCase()
    {
        _dbContext = new PassInDbContext();
    }

    public ResponseRegisteredJson Execute(Guid attendeeId)
    {
        Validate(attendeeId);

        var entity = new CheckIn
        {
            Attendee_Id = attendeeId,
            Created_at = DateTime.UtcNow,
        };

        _dbContext.CheckIns.Add(entity);
        _dbContext.SaveChanges();

        return new ResponseRegisteredJson
        {
            Id = entity.Id,
        };
    }

    private void Validate(Guid attendeeId)
    {
        var attendeeExists = _dbContext.Attendees.Any(attendee => attendee.Id == attendeeId);

        if(attendeeExists == false)
        {
            throw new NotFoundException("The attendee with this id was not found.");
        }

        var checkInExists = _dbContext.CheckIns.Any(ch => ch.Attendee_Id == attendeeId);

        if (checkInExists)
        {
            throw new ConflictException("Attendee cannot check in twice in the same event.");
        }
    }
}
