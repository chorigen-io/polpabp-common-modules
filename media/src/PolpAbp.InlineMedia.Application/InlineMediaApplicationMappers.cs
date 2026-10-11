using PolpAbp.InlineMedia.Domain.Entities;
using PolpAbp.InlineMedia.Dtos;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace PolpAbp.InlineMedia;

/* Mapperly mappers (formerly InlineMediaApplicationAutoMapperProfile).
 * One [Mapper] class per type pair; ABP registers each one conventionally
 * and IObjectMapper.Map<,> resolves it.
 * Former IgnoreSourceMissingProperties() = RequiredMappingStrategy.Target plus an
 * explicit [MapperIgnoreTarget] for every settable target member with no source. */

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class PictureInputDtoToPictureMapper : MapperBase<PictureInputDto, Picture>
{
    [MapperIgnoreTarget(nameof(Picture.TenantId))]
    [MapperIgnoreTarget(nameof(Picture.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(Picture.ExtraProperties))]
    [MapperIgnoreTarget(nameof(Picture.CreationTime))]
    [MapperIgnoreTarget(nameof(Picture.CreatorId))]
    [MapperIgnoreTarget(nameof(Picture.LastModificationTime))]
    [MapperIgnoreTarget(nameof(Picture.LastModifierId))]
    [MapperIgnoreTarget(nameof(Picture.IsDeleted))]
    [MapperIgnoreTarget(nameof(Picture.DeleterId))]
    [MapperIgnoreTarget(nameof(Picture.DeletionTime))]
    public override partial Picture Map(PictureInputDto source);

    [MapperIgnoreTarget(nameof(Picture.TenantId))]
    [MapperIgnoreTarget(nameof(Picture.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(Picture.ExtraProperties))]
    [MapperIgnoreTarget(nameof(Picture.CreationTime))]
    [MapperIgnoreTarget(nameof(Picture.CreatorId))]
    [MapperIgnoreTarget(nameof(Picture.LastModificationTime))]
    [MapperIgnoreTarget(nameof(Picture.LastModifierId))]
    [MapperIgnoreTarget(nameof(Picture.IsDeleted))]
    [MapperIgnoreTarget(nameof(Picture.DeleterId))]
    [MapperIgnoreTarget(nameof(Picture.DeletionTime))]
    public override partial void Map(PictureInputDto source, Picture destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class PictureToPictureOutputDtoMapper : MapperBase<Picture, PictureOutputDto>
{
    public override partial PictureOutputDto Map(Picture source);

    public override partial void Map(Picture source, PictureOutputDto destination);
}
