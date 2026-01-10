using Microsoft.AspNetCore.Identity;

namespace ContactsManager.Core.Domain.IdentityEntities;

public class User : IdentityUser<Guid>
{
    public User()
    {
        Id = Guid.CreateVersion7();
    }
    public required string Name { get; set; }
}
