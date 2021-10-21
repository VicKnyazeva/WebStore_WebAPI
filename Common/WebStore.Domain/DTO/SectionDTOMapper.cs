using System.Collections.Generic;
using System.Linq;

using WebStore.Domain.Entities;

namespace WebStore.Domain.DTO
{
    public static class SectionDTOMapper
    {
        public static SectionDTO ToDTO(this Section section) => section is null
            ? null
            : new SectionDTO
            {
                Id = section.Id,
                Name = section.Name,
                Order = section.Order,
                ParentId = section.ParentId,
            };

        public static Section FromDTO(this SectionDTO section) => section is null
            ? null
            : new Section
            {
                Id = section.Id,
                Name = section.Name,
                Order = section.Order,
                ParentId = section.ParentId,
            };

        public static IEnumerable<SectionDTO> ToDTO(this IEnumerable<Section> sections) => sections.Select(ToDTO);
        public static IEnumerable<Section> FromDTO(this IEnumerable<SectionDTO> sections) => sections.Select(FromDTO);
    }
}
