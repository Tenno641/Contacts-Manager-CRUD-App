using Microsoft.AspNetCore.Identity;

namespace ContactsManager.Core.Domain.IdentityEntities;

public class Role : IdentityRole<Guid>
{
    public Role()
    {
        Id = Guid.CreateVersion7();
    }
}
