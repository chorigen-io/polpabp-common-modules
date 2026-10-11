using System;
using Volo.Abp.Application.Dtos;

namespace PolpAbp.Directory.Dtos
{
    public class StateProvinceOutputDto : StateProvinceInputDto, IEntityDto<Guid>
    {
        public Guid Id { get; set; }

        public virtual string GetObjectKey()
        {
            return Id.ToString();
        }
    }
}

