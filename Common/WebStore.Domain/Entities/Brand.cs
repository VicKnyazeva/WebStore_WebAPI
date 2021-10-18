
using Microsoft.EntityFrameworkCore;

using WebStore.Domain.Entities.Base;
using WebStore.Domain.Entities.Base.Interfaces;

namespace WebStore.Domain.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Brand : NamedEntity, IOrderedEntity
    {
        public int Order { get; set; }
    }
}
