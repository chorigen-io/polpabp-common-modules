using System;
using PolpAbp.InlineMedia.Domain.Entities;
using PolpAbp.InlineMedia.Dtos;
using Shouldly;
using Volo.Abp.Data;
using Volo.Abp.ObjectMapping;
using Xunit;

namespace PolpAbp.InlineMedia.Mapping
{
    /// <summary>
    /// Mapping-parity tests (T026): one fact per pair of the former
    /// InlineMediaApplicationAutoMapperProfile. Each fact encodes the AutoMapper
    /// semantics as explicit expected values: IgnoreSourceMissingProperties leaves
    /// every target member without a same-named source member untouched (default on
    /// a new object, existing value on an in-place map).
    /// </summary>
    public class InlineMediaMappingParityTests : InlineMediaApplicationTestBase
    {
        private readonly IObjectMapper _objectMapper;

        public InlineMediaMappingParityTests()
        {
            _objectMapper = GetRequiredService<IObjectMapper<InlineMediaApplicationModule>>();
        }

        // CreateMap<PictureInputDto, Picture>().IgnoreSourceMissingProperties()
        [Fact]
        public void PictureInputDto_To_Picture()
        {
            var picture = _objectMapper.Map<PictureInputDto, Picture>(NewPictureInput());

            AssertPictureFields(picture);
            // Not in the source: defaults of a new Picture
            picture.Id.ShouldBe(Guid.Empty);
            picture.TenantId.ShouldBeNull();
            picture.CreationTime.ShouldBe(default);
            picture.LastModificationTime.ShouldBeNull();
            picture.IsDeleted.ShouldBeFalse();
            picture.ExtraProperties.ShouldBeEmpty();
        }

        // Same pair, in-place form (PictureStoreAppService.UpdateAsync): members missing
        // from the source keep their existing values.
        [Fact]
        public void PictureInputDto_To_Existing_Picture_Preserves_Unmapped()
        {
            var id = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var creationTime = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            var modificationTime = new DateTime(2026, 2, 3, 4, 5, 6, DateTimeKind.Utc);
            var picture = new Picture(id)
            {
                TenantId = tenantId,
                PictureBinary = new byte[] { 9, 9 },
                MimeType = "image/gif",
                LastModificationTime = modificationTime,
                ConcurrencyStamp = "stamp-1"
            };
            SetProperty(picture, nameof(Picture.CreationTime), creationTime);
            picture.SetProperty("Keep", "kept");

            _objectMapper.Map(NewPictureInput(), picture);

            AssertPictureFields(picture);
            picture.Id.ShouldBe(id);
            picture.TenantId.ShouldBe(tenantId);
            picture.CreationTime.ShouldBe(creationTime);
            picture.LastModificationTime.ShouldBe(modificationTime);
            picture.ConcurrencyStamp.ShouldBe("stamp-1");
            picture.ExtraProperties.Count.ShouldBe(1);
            picture.ExtraProperties["Keep"].ShouldBe("kept");
        }

        // CreateMap<Picture, PictureOutputDto>() -- by-name copy.
        [Fact]
        public void Picture_To_PictureOutputDto()
        {
            var id = Guid.NewGuid();
            var creationTime = new DateTime(2026, 3, 4, 5, 6, 7, DateTimeKind.Utc);
            var modificationTime = new DateTime(2026, 4, 5, 6, 7, 8, DateTimeKind.Utc);
            var picture = new Picture(id)
            {
                TenantId = Guid.NewGuid(),
                PictureBinary = new byte[] { 1, 2, 3, 4 },
                MimeType = "image/png",
                SeoFilename = "seo-name",
                AltAttribute = "alt",
                TitleAttribute = "title",
                IsNew = true,
                LastModificationTime = modificationTime
            };
            SetProperty(picture, nameof(Picture.CreationTime), creationTime);

            var dto = _objectMapper.Map<Picture, PictureOutputDto>(picture);

            dto.Id.ShouldBe(id);
            dto.CreationTime.ShouldBe(creationTime);
            dto.LastModificationTime.ShouldBe(modificationTime);
            dto.PictureBinary.ShouldBe(new byte[] { 1, 2, 3, 4 });
            dto.MimeType.ShouldBe("image/png");
            dto.SeoFilename.ShouldBe("seo-name");
            dto.AltAttribute.ShouldBe("alt");
            dto.TitleAttribute.ShouldBe("title");
            dto.IsNew.ShouldBeTrue();
        }

        private static PictureInputDto NewPictureInput()
        {
            return new PictureInputDto
            {
                PictureBinary = new byte[] { 10, 20, 30 },
                MimeType = "image/jpeg",
                SeoFilename = "input-seo",
                AltAttribute = "input-alt",
                TitleAttribute = "input-title",
                IsNew = true
            };
        }

        private static void AssertPictureFields(Picture picture)
        {
            picture.PictureBinary.ShouldBe(new byte[] { 10, 20, 30 });
            picture.MimeType.ShouldBe("image/jpeg");
            picture.SeoFilename.ShouldBe("input-seo");
            picture.AltAttribute.ShouldBe("input-alt");
            picture.TitleAttribute.ShouldBe("input-title");
            picture.IsNew.ShouldBeTrue();
        }

        private static void SetProperty(object target, string name, object value)
        {
            var type = target.GetType();
            while (type != null)
            {
                var property = type.GetProperty(name,
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.DeclaredOnly);
                if (property != null && property.GetSetMethod(true) != null)
                {
                    property.SetValue(target, value);
                    return;
                }
                type = type.BaseType;
            }
            throw new InvalidOperationException($"No settable property {name} on {target.GetType()}");
        }
    }
}
