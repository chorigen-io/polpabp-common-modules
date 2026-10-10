using PolpAbp.Contact.Domain.Entities;
using PolpAbp.Contact.Dtos;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace PolpAbp.Contact;

/* Mapperly mappers (formerly ContactApplicationAutoMapperProfile).
 * One [Mapper] class per type pair; ABP registers each one conventionally
 * and IObjectMapper.Map<,> resolves it.
 * Former IgnoreSourceMissingProperties() = RequiredMappingStrategy.Target plus an
 * explicit [MapperIgnoreTarget] for every settable target member with no source. */

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class AddressInputDtoToAddressMapper : MapperBase<AddressInputDto, Address>
{
    [MapperIgnoreTarget(nameof(Address.TenantId))]
    [MapperIgnoreTarget(nameof(Address.OwnerId))]
    [MapperIgnoreTarget(nameof(Address.RoleId))]
    [MapperIgnoreTarget(nameof(Address.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(Address.ExtraProperties))]
    [MapperIgnoreTarget(nameof(Address.LastModificationTime))]
    [MapperIgnoreTarget(nameof(Address.LastModifierId))]
    [MapperIgnoreTarget(nameof(Address.IsDeleted))]
    [MapperIgnoreTarget(nameof(Address.DeleterId))]
    [MapperIgnoreTarget(nameof(Address.DeletionTime))]
    [MapperIgnoreTarget(nameof(Address.CreationTime))]
    [MapperIgnoreTarget(nameof(Address.CreatorId))]
    public override partial Address Map(AddressInputDto source);

    [MapperIgnoreTarget(nameof(Address.TenantId))]
    [MapperIgnoreTarget(nameof(Address.OwnerId))]
    [MapperIgnoreTarget(nameof(Address.RoleId))]
    [MapperIgnoreTarget(nameof(Address.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(Address.ExtraProperties))]
    [MapperIgnoreTarget(nameof(Address.LastModificationTime))]
    [MapperIgnoreTarget(nameof(Address.LastModifierId))]
    [MapperIgnoreTarget(nameof(Address.IsDeleted))]
    [MapperIgnoreTarget(nameof(Address.DeleterId))]
    [MapperIgnoreTarget(nameof(Address.DeletionTime))]
    [MapperIgnoreTarget(nameof(Address.CreationTime))]
    [MapperIgnoreTarget(nameof(Address.CreatorId))]
    public override partial void Map(AddressInputDto source, Address destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class AddressToAddressOutputDtoMapper : MapperBase<Address, AddressOutputDto>
{
    public override partial AddressOutputDto Map(Address source);

    public override partial void Map(Address source, AddressOutputDto destination);
}

// The source derives from the target: deep cloning makes Mapperly build a new
// target (as AutoMapper did) instead of returning the source instance itself.
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target, UseDeepCloning = true)]
public partial class AddressOutputDtoToAddressInputDtoMapper : MapperBase<AddressOutputDto, AddressInputDto>
{
    public override partial AddressInputDto Map(AddressOutputDto source);

    public override partial void Map(AddressOutputDto source, AddressInputDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ContactCardInputDtoToContactCardMapper : MapperBase<ContactCardInputDto, ContactCard>
{
    [MapperIgnoreTarget(nameof(ContactCard.TenantId))]
    [MapperIgnoreTarget(nameof(ContactCard.OwnerId))]
    [MapperIgnoreTarget(nameof(ContactCard.RoleId))]
    [MapperIgnoreTarget(nameof(ContactCard.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(ContactCard.ExtraProperties))]
    [MapperIgnoreTarget(nameof(ContactCard.LastModificationTime))]
    [MapperIgnoreTarget(nameof(ContactCard.LastModifierId))]
    [MapperIgnoreTarget(nameof(ContactCard.IsDeleted))]
    [MapperIgnoreTarget(nameof(ContactCard.DeleterId))]
    [MapperIgnoreTarget(nameof(ContactCard.DeletionTime))]
    [MapperIgnoreTarget(nameof(ContactCard.CreationTime))]
    [MapperIgnoreTarget(nameof(ContactCard.CreatorId))]
    public override partial ContactCard Map(ContactCardInputDto source);

    [MapperIgnoreTarget(nameof(ContactCard.TenantId))]
    [MapperIgnoreTarget(nameof(ContactCard.OwnerId))]
    [MapperIgnoreTarget(nameof(ContactCard.RoleId))]
    [MapperIgnoreTarget(nameof(ContactCard.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(ContactCard.ExtraProperties))]
    [MapperIgnoreTarget(nameof(ContactCard.LastModificationTime))]
    [MapperIgnoreTarget(nameof(ContactCard.LastModifierId))]
    [MapperIgnoreTarget(nameof(ContactCard.IsDeleted))]
    [MapperIgnoreTarget(nameof(ContactCard.DeleterId))]
    [MapperIgnoreTarget(nameof(ContactCard.DeletionTime))]
    [MapperIgnoreTarget(nameof(ContactCard.CreationTime))]
    [MapperIgnoreTarget(nameof(ContactCard.CreatorId))]
    public override partial void Map(ContactCardInputDto source, ContactCard destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ContactCardToContactCardOutputDtoMapper : MapperBase<ContactCard, ContactCardOutputDto>
{
    public override partial ContactCardOutputDto Map(ContactCard source);

    public override partial void Map(ContactCard source, ContactCardOutputDto destination);
}

// The source derives from the target: deep cloning makes Mapperly build a new
// target (as AutoMapper did) instead of returning the source instance itself.
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target, UseDeepCloning = true)]
public partial class ContactCardOutputDtoToContactCardInputDtoMapper : MapperBase<ContactCardOutputDto, ContactCardInputDto>
{
    public override partial ContactCardInputDto Map(ContactCardOutputDto source);

    public override partial void Map(ContactCardOutputDto source, ContactCardInputDto destination);
}
