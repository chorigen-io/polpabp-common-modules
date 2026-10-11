using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;

namespace PolpAbp.Contact.Dtos
{
    public class ContactCardOutputDto : ContactCardInputDto, IHasCreationTime, IEntityDto<Guid>
    {
        public Guid Id { get; set; }

        public virtual string GetObjectKey()
        {
            return Id.ToString();
        }
        public DateTime CreationTime { get; set; }
    }
}

