using PolpAbp.Directory.Domain.Entities;
using PolpAbp.Directory.Dtos;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace PolpAbp.Directory;

/* Mapperly mappers (formerly DirectoryApplicationAutoMapperProfile).
 * One [Mapper] class per type pair; ABP registers each one conventionally
 * and IObjectMapper.Map<,> resolves it.
 * Former IgnoreSourceMissingProperties() = RequiredMappingStrategy.Target plus an
 * explicit [MapperIgnoreTarget] for every settable target member with no source. */

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CountryInputDtoToCountryMapper : MapperBase<CountryInputDto, Country>
{
    [MapperIgnoreTarget(nameof(Country.CreationTime))]
    [MapperIgnoreTarget(nameof(Country.StateProvinces))]
    [MapperIgnoreTarget(nameof(Country.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(Country.ExtraProperties))]
    public override partial Country Map(CountryInputDto source);

    [MapperIgnoreTarget(nameof(Country.CreationTime))]
    [MapperIgnoreTarget(nameof(Country.StateProvinces))]
    [MapperIgnoreTarget(nameof(Country.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(Country.ExtraProperties))]
    public override partial void Map(CountryInputDto source, Country destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CountryToCountryOutputDtoMapper : MapperBase<Country, CountryOutputDto>
{
    public override partial CountryOutputDto Map(Country source);

    public override partial void Map(Country source, CountryOutputDto destination);
}

// The source derives from the target: deep cloning makes Mapperly build a new
// target (as AutoMapper did) instead of returning the source instance itself.
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target, UseDeepCloning = true)]
public partial class CountryOutputDtoToCountryInputDtoMapper : MapperBase<CountryOutputDto, CountryInputDto>
{
    public override partial CountryInputDto Map(CountryOutputDto source);

    public override partial void Map(CountryOutputDto source, CountryInputDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class StateProvinceInputDtoToStateProvinceMapper : MapperBase<StateProvinceInputDto, StateProvince>
{
    [MapperIgnoreTarget(nameof(StateProvince.CreationTime))]
    [MapperIgnoreTarget(nameof(StateProvince.CountryId))]
    public override partial StateProvince Map(StateProvinceInputDto source);

    [MapperIgnoreTarget(nameof(StateProvince.CreationTime))]
    [MapperIgnoreTarget(nameof(StateProvince.CountryId))]
    public override partial void Map(StateProvinceInputDto source, StateProvince destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class StateProvinceToStateProvinceOutputDtoMapper : MapperBase<StateProvince, StateProvinceOutputDto>
{
    public override partial StateProvinceOutputDto Map(StateProvince source);

    public override partial void Map(StateProvince source, StateProvinceOutputDto destination);
}

// The source derives from the target: see CountryOutputDtoToCountryInputDtoMapper.
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target, UseDeepCloning = true)]
public partial class StateProvinceOutputDtoToStateProvinceInputDtoMapper : MapperBase<StateProvinceOutputDto, StateProvinceInputDto>
{
    public override partial StateProvinceInputDto Map(StateProvinceOutputDto source);

    public override partial void Map(StateProvinceOutputDto source, StateProvinceInputDto destination);
}
