using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;

namespace PolpAbp.Directory.Dtos
{
    public class CountryOutputDto : CountryInputDto, IEntityDto<Guid>
    {
        public Guid Id { get; set; }

        public virtual string GetObjectKey()
        {
            return Id.ToString();
        }
    }
}

