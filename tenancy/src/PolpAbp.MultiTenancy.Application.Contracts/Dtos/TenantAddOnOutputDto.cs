using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;

namespace PolpAbp.MultiTenancy.Dtos
{
    public class TenantAddOnOutputDto : TenantAddOnInputDto, IHasCreationTime, IEntityDto<Guid>, IHasModificationTime
    {
        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public Guid Id { get; set; }

        public virtual string GetObjectKey()
        {
            return Id.ToString();
        }

        public List<TenantAddressMapOutputDto> AddresseMaps { get; set; }

        public List<TenantContactMapOutputDto> ContactMaps { get; set; }

        public List<TenantPictureMapOutputDto> PictureMaps { get; set; }

        public TenantAddOnOutputDto()
        {
            AddresseMaps= new List<TenantAddressMapOutputDto>();
            ContactMaps= new List<TenantContactMapOutputDto>();
            PictureMaps= new List<TenantPictureMapOutputDto>();
        }
    }
}

