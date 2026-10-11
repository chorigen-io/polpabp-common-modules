using System;
using PolpAbp.MultiTenancy.Domain.Entities;
using PolpAbp.MultiTenancy.Dtos;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace PolpAbp.MultiTenancy;

/* Mapperly mappers (formerly MultiTenancyApplicationAutoMapperProfile).
 * One [Mapper] class per type pair; ABP registers each one conventionally
 * and IObjectMapper.Map<,> resolves it.
 * Former IgnoreSourceMissingProperties() = RequiredMappingStrategy.Target plus an
 * explicit [MapperIgnoreTarget] for every settable target member with no source.
 * The entities' parameterless constructors are protected (AutoMapper used them
 * through reflection); each entity-target mapper has an [ObjectFactory] that calls
 * the public constructor with Guid.Empty, which builds the same object.
 * The get-only PictureRole members (former ForMember(PictureRole, Ignore)) are not
 * writable, so Mapperly never targets them. */

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class TenantAddOnInputDtoToTenantAddOnMapper : MapperBase<TenantAddOnInputDto, TenantAddOn>
{
    [MapperIgnoreTarget(nameof(TenantAddOn.TenantId))]
    [MapperIgnoreTarget(nameof(TenantAddOn.ContactMaps))]
    [MapperIgnoreTarget(nameof(TenantAddOn.AddressMaps))]
    [MapperIgnoreTarget(nameof(TenantAddOn.PictureMaps))]
    [MapperIgnoreTarget(nameof(TenantAddOn.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(TenantAddOn.ExtraProperties))]
    [MapperIgnoreTarget(nameof(TenantAddOn.CreationTime))]
    [MapperIgnoreTarget(nameof(TenantAddOn.CreatorId))]
    [MapperIgnoreTarget(nameof(TenantAddOn.LastModificationTime))]
    [MapperIgnoreTarget(nameof(TenantAddOn.LastModifierId))]
    [MapperIgnoreTarget(nameof(TenantAddOn.IsDeleted))]
    [MapperIgnoreTarget(nameof(TenantAddOn.DeleterId))]
    [MapperIgnoreTarget(nameof(TenantAddOn.DeletionTime))]
    public override partial TenantAddOn Map(TenantAddOnInputDto source);

    [MapperIgnoreTarget(nameof(TenantAddOn.TenantId))]
    [MapperIgnoreTarget(nameof(TenantAddOn.ContactMaps))]
    [MapperIgnoreTarget(nameof(TenantAddOn.AddressMaps))]
    [MapperIgnoreTarget(nameof(TenantAddOn.PictureMaps))]
    [MapperIgnoreTarget(nameof(TenantAddOn.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(TenantAddOn.ExtraProperties))]
    [MapperIgnoreTarget(nameof(TenantAddOn.CreationTime))]
    [MapperIgnoreTarget(nameof(TenantAddOn.CreatorId))]
    [MapperIgnoreTarget(nameof(TenantAddOn.LastModificationTime))]
    [MapperIgnoreTarget(nameof(TenantAddOn.LastModifierId))]
    [MapperIgnoreTarget(nameof(TenantAddOn.IsDeleted))]
    [MapperIgnoreTarget(nameof(TenantAddOn.DeleterId))]
    [MapperIgnoreTarget(nameof(TenantAddOn.DeletionTime))]
    public override partial void Map(TenantAddOnInputDto source, TenantAddOn destination);

    [ObjectFactory]
    private TenantAddOn CreateTenantAddOn() => new TenantAddOn(Guid.Empty);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class TenantAddOnToTenantAddOnOutputDtoMapper : MapperBase<TenantAddOn, TenantAddOnOutputDto>
{
    [MapperIgnoreTarget(nameof(TenantAddOnOutputDto.AddresseMaps))]
    [MapperIgnoreTarget(nameof(TenantAddOnOutputDto.ContactMaps))]
    [MapperIgnoreTarget(nameof(TenantAddOnOutputDto.PictureMaps))]
    public override partial TenantAddOnOutputDto Map(TenantAddOn source);

    [MapperIgnoreTarget(nameof(TenantAddOnOutputDto.AddresseMaps))]
    [MapperIgnoreTarget(nameof(TenantAddOnOutputDto.ContactMaps))]
    [MapperIgnoreTarget(nameof(TenantAddOnOutputDto.PictureMaps))]
    public override partial void Map(TenantAddOn source, TenantAddOnOutputDto destination);
}

// The source derives from the target: deep cloning makes Mapperly build a new
// target (as AutoMapper did) instead of returning the source instance itself.
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target, UseDeepCloning = true)]
public partial class TenantAddOnOutputDtoToTenantAddOnInputDtoMapper : MapperBase<TenantAddOnOutputDto, TenantAddOnInputDto>
{
    public override partial TenantAddOnInputDto Map(TenantAddOnOutputDto source);

    public override partial void Map(TenantAddOnOutputDto source, TenantAddOnInputDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class TenantAddressMapToTenantAddressMapOutputDtoMapper : MapperBase<TenantAddressMap, TenantAddressMapOutputDto>
{
    public override partial TenantAddressMapOutputDto Map(TenantAddressMap source);

    public override partial void Map(TenantAddressMap source, TenantAddressMapOutputDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class TenantAddressMapInputDtoToTenantAddressMapMapper : MapperBase<TenantAddressMapInputDto, TenantAddressMap>
{
    [MapperIgnoreTarget(nameof(TenantAddressMap.CreationTime))]
    [MapperIgnoreTarget(nameof(TenantAddressMap.DeletionTime))]
    [MapperIgnoreTarget(nameof(TenantAddressMap.IsDeleted))]
    [MapperIgnoreTarget(nameof(TenantAddressMap.TenantId))]
    [MapperIgnoreTarget(nameof(TenantAddressMap.LastModificationTime))]
    public override partial TenantAddressMap Map(TenantAddressMapInputDto source);

    [MapperIgnoreTarget(nameof(TenantAddressMap.CreationTime))]
    [MapperIgnoreTarget(nameof(TenantAddressMap.DeletionTime))]
    [MapperIgnoreTarget(nameof(TenantAddressMap.IsDeleted))]
    [MapperIgnoreTarget(nameof(TenantAddressMap.TenantId))]
    [MapperIgnoreTarget(nameof(TenantAddressMap.LastModificationTime))]
    public override partial void Map(TenantAddressMapInputDto source, TenantAddressMap destination);

    [ObjectFactory]
    private TenantAddressMap CreateTenantAddressMap() => new TenantAddressMap(Guid.Empty);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class TenantContactMapToTenantContactMapOutputDtoMapper : MapperBase<TenantContactMap, TenantContactMapOutputDto>
{
    public override partial TenantContactMapOutputDto Map(TenantContactMap source);

    public override partial void Map(TenantContactMap source, TenantContactMapOutputDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class TenantContactMapInputDtoToTenantContactMapMapper : MapperBase<TenantContactMapInputDto, TenantContactMap>
{
    [MapperIgnoreTarget(nameof(TenantContactMap.CreationTime))]
    [MapperIgnoreTarget(nameof(TenantContactMap.DeletionTime))]
    [MapperIgnoreTarget(nameof(TenantContactMap.LastModificationTime))]
    [MapperIgnoreTarget(nameof(TenantContactMap.IsDeleted))]
    [MapperIgnoreTarget(nameof(TenantContactMap.TenantId))]
    public override partial TenantContactMap Map(TenantContactMapInputDto source);

    [MapperIgnoreTarget(nameof(TenantContactMap.CreationTime))]
    [MapperIgnoreTarget(nameof(TenantContactMap.DeletionTime))]
    [MapperIgnoreTarget(nameof(TenantContactMap.LastModificationTime))]
    [MapperIgnoreTarget(nameof(TenantContactMap.IsDeleted))]
    [MapperIgnoreTarget(nameof(TenantContactMap.TenantId))]
    public override partial void Map(TenantContactMapInputDto source, TenantContactMap destination);

    [ObjectFactory]
    private TenantContactMap CreateTenantContactMap() => new TenantContactMap(Guid.Empty);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class TenantPictureMapInputDtoToTenantPictureMapMapper : MapperBase<TenantPictureMapInputDto, TenantPictureMap>
{
    [MapperIgnoreTarget(nameof(TenantPictureMap.CreationTime))]
    [MapperIgnoreTarget(nameof(TenantPictureMap.LastModificationTime))]
    [MapperIgnoreTarget(nameof(TenantPictureMap.DeletionTime))]
    [MapperIgnoreTarget(nameof(TenantPictureMap.IsDeleted))]
    [MapperIgnoreTarget(nameof(TenantPictureMap.TenantId))]
    public override partial TenantPictureMap Map(TenantPictureMapInputDto source);

    [MapperIgnoreTarget(nameof(TenantPictureMap.CreationTime))]
    [MapperIgnoreTarget(nameof(TenantPictureMap.LastModificationTime))]
    [MapperIgnoreTarget(nameof(TenantPictureMap.DeletionTime))]
    [MapperIgnoreTarget(nameof(TenantPictureMap.IsDeleted))]
    [MapperIgnoreTarget(nameof(TenantPictureMap.TenantId))]
    public override partial void Map(TenantPictureMapInputDto source, TenantPictureMap destination);

    [ObjectFactory]
    private TenantPictureMap CreateTenantPictureMap() => new TenantPictureMap(Guid.Empty);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class TenantPictureMapToTenantPictureMapOutputDtoMapper : MapperBase<TenantPictureMap, TenantPictureMapOutputDto>
{
    public override partial TenantPictureMapOutputDto Map(TenantPictureMap source);

    public override partial void Map(TenantPictureMap source, TenantPictureMapOutputDto destination);
}
