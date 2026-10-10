using System;
using System.Collections.Generic;
using PolpAbp.MultiTenancy.Domain.Entities;
using PolpAbp.MultiTenancy.Dtos;
using PolpAbp.MultiTenancy.Shared;
using Shouldly;
using Volo.Abp.Data;
using Volo.Abp.ObjectMapping;
using Xunit;

namespace PolpAbp.MultiTenancy.Mapping
{
    /// <summary>
    /// Mapping-parity tests (T026): one fact per pair of the former
    /// MultiTenancyApplicationAutoMapperProfile. Each fact encodes the AutoMapper
    /// semantics as explicit expected values: ignored members and members missing from
    /// the source stay default on a new object and keep their value on an in-place map.
    /// </summary>
    public class MultiTenancyMappingParityTests : MultiTenancyApplicationTestBase
    {
        private readonly IObjectMapper _objectMapper;

        public MultiTenancyMappingParityTests()
        {
            _objectMapper = GetRequiredService<IObjectMapper<MultiTenancyApplicationModule>>();
        }

        // CreateMap<TenantAddOnInputDto, TenantAddOn>().IgnoreSourceMissingProperties()
        [Fact]
        public void TenantAddOnInputDto_To_TenantAddOn()
        {
            var input = NewAddOnInput(out var orderId, out var planId);

            var addOn = _objectMapper.Map<TenantAddOnInputDto, TenantAddOn>(input);

            AssertAddOnFields(addOn, orderId, planId);
            // Not in the source: defaults of a new TenantAddOn
            addOn.Id.ShouldBe(Guid.Empty);
            addOn.TenantId.ShouldBeNull();
            addOn.ContactMaps.ShouldBeNull();
            addOn.AddressMaps.ShouldBeNull();
            addOn.PictureMaps.ShouldBeNull();
            addOn.CreationTime.ShouldBe(default);
            addOn.IsDeleted.ShouldBeFalse();
            addOn.ExtraProperties.ShouldBeEmpty();
        }

        // Same pair, in-place form (TenantAddOnAppService.CreateOrUpdateAsync).
        [Fact]
        public void TenantAddOnInputDto_To_Existing_TenantAddOn_Preserves_Unmapped()
        {
            var id = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var addressMaps = new List<TenantAddressMap> { new TenantAddressMap(Guid.NewGuid()) };
            var addOn = new TenantAddOn(id)
            {
                TenantId = tenantId,
                DisplayName = "Old",
                AddressMaps = addressMaps,
                ConcurrencyStamp = "stamp-1"
            };
            addOn.SetProperty("Keep", "kept");
            var input = NewAddOnInput(out var orderId, out var planId);

            _objectMapper.Map(input, addOn);

            AssertAddOnFields(addOn, orderId, planId);
            addOn.Id.ShouldBe(id);
            addOn.TenantId.ShouldBe(tenantId);
            addOn.AddressMaps.ShouldBeSameAs(addressMaps);
            addOn.ConcurrencyStamp.ShouldBe("stamp-1");
            addOn.ExtraProperties.Count.ShouldBe(1);
            addOn.ExtraProperties["Keep"].ShouldBe("kept");
        }

        // CreateMap<TenantAddOn, TenantAddOnOutputDto>()
        //   Ignore: AddresseMaps, ContactMaps, PictureMaps
        [Fact]
        public void TenantAddOn_To_TenantAddOnOutputDto()
        {
            var id = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var modificationTime = new DateTime(2026, 2, 3, 4, 5, 6, DateTimeKind.Utc);
            var addOn = new TenantAddOn(id)
            {
                TenantId = Guid.NewGuid(),
                IsAutoProvisioningEnabled = true,
                DisplayName = "Acme",
                Description = "Acme Inc.",
                CompanyTypeId = 2,
                CompanySize = 50,
                IndustryCodeId = 7,
                OrderId = orderId,
                PlanId = planId,
                LastModificationTime = modificationTime,
                AddressMaps = new List<TenantAddressMap> { new TenantAddressMap(Guid.NewGuid()) },
                ContactMaps = new List<TenantContactMap> { new TenantContactMap(Guid.NewGuid()) },
                PictureMaps = new List<TenantPictureMap> { new TenantPictureMap(Guid.NewGuid()) }
            };
            var creationTime = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            SetProperty(addOn, nameof(TenantAddOn.CreationTime), creationTime);

            var dto = _objectMapper.Map<TenantAddOn, TenantAddOnOutputDto>(addOn);

            dto.Id.ShouldBe(id);
            dto.CreationTime.ShouldBe(creationTime);
            dto.LastModificationTime.ShouldBe(modificationTime);
            dto.IsAutoProvisioningEnabled.ShouldBeTrue();
            dto.DisplayName.ShouldBe("Acme");
            dto.Description.ShouldBe("Acme Inc.");
            dto.CompanyTypeId.ShouldBe(2);
            dto.CompanySize.ShouldBe(50);
            dto.IndustryCodeId.ShouldBe(7);
            dto.OrderId.ShouldBe(orderId);
            dto.PlanId.ShouldBe(planId);
            // Ignored: the constructor's empty lists
            dto.AddresseMaps.ShouldBeEmpty();
            dto.ContactMaps.ShouldBeEmpty();
            dto.PictureMaps.ShouldBeEmpty();
        }

        // CreateMap<TenantAddOnOutputDto, TenantAddOnInputDto>() -- by-name copy.
        [Fact]
        public void TenantAddOnOutputDto_To_TenantAddOnInputDto()
        {
            var orderId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var output = new TenantAddOnOutputDto
            {
                Id = Guid.NewGuid(),
                CreationTime = DateTime.UtcNow,
                IsAutoProvisioningEnabled = true,
                DisplayName = "Out",
                Description = "Out desc",
                CompanyTypeId = 3,
                CompanySize = 9,
                IndustryCodeId = 4,
                OrderId = orderId,
                PlanId = planId
            };

            var input = _objectMapper.Map<TenantAddOnOutputDto, TenantAddOnInputDto>(output);

            input.ShouldNotBeOfType<TenantAddOnOutputDto>();
            input.IsAutoProvisioningEnabled.ShouldBeTrue();
            input.DisplayName.ShouldBe("Out");
            input.Description.ShouldBe("Out desc");
            input.CompanyTypeId.ShouldBe(3);
            input.CompanySize.ShouldBe(9);
            input.IndustryCodeId.ShouldBe(4);
            input.OrderId.ShouldBe(orderId);
            input.PlanId.ShouldBe(planId);
        }

        // CreateMap<TenantAddressMap, TenantAddressMapOutputDto>() -- by-name copy.
        [Fact]
        public void TenantAddressMap_To_TenantAddressMapOutputDto()
        {
            var id = Guid.NewGuid();
            var addressId = Guid.NewGuid();
            var creationTime = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc);
            var modificationTime = new DateTime(2026, 3, 2, 0, 0, 0, DateTimeKind.Utc);
            var map = new TenantAddressMap(id)
            {
                AddressId = addressId,
                IsPrimary = true,
                IsBillingAddress = true,
                DisplayOrder = 2,
                CreationTime = creationTime,
                LastModificationTime = modificationTime,
                TenantId = Guid.NewGuid()
            };

            var dto = _objectMapper.Map<TenantAddressMap, TenantAddressMapOutputDto>(map);

            dto.Id.ShouldBe(id);
            dto.AddressId.ShouldBe(addressId);
            dto.IsPrimary.ShouldBeTrue();
            dto.IsBillingAddress.ShouldBeTrue();
            dto.DisplayOrder.ShouldBe(2);
            dto.CreationTime.ShouldBe(creationTime);
            dto.LastModificationTime.ShouldBe(modificationTime);
        }

        // CreateMap<TenantAddressMapInputDto, TenantAddressMap>().IgnoreSourceMissingProperties()
        // (used in place by TenantAddOnAppService.AddOrUpdateAddressMapAsync)
        [Fact]
        public void TenantAddressMapInputDto_To_TenantAddressMap()
        {
            var addressId = Guid.NewGuid();
            var input = new TenantAddressMapInputDto
            {
                AddressId = addressId,
                IsPrimary = true,
                IsBillingAddress = false,
                DisplayOrder = 5
            };

            var created = _objectMapper.Map<TenantAddressMapInputDto, TenantAddressMap>(input);

            created.AddressId.ShouldBe(addressId);
            created.IsPrimary.ShouldBeTrue();
            created.IsBillingAddress.ShouldBeFalse();
            created.DisplayOrder.ShouldBe(5);
            created.Id.ShouldBe(Guid.Empty);
            created.TenantId.ShouldBe(Guid.Empty);
            created.CreationTime.ShouldBe(default);
            created.LastModificationTime.ShouldBeNull();
            created.DeletionTime.ShouldBeNull();
            created.IsDeleted.ShouldBeFalse();

            var id = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var creationTime = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc);
            var existing = new TenantAddressMap(id)
            {
                TenantId = tenantId,
                CreationTime = creationTime,
                IsBillingAddress = true
            };

            _objectMapper.Map(input, existing);

            existing.AddressId.ShouldBe(addressId);
            existing.IsPrimary.ShouldBeTrue();
            existing.IsBillingAddress.ShouldBeFalse();
            existing.DisplayOrder.ShouldBe(5);
            existing.Id.ShouldBe(id);
            existing.TenantId.ShouldBe(tenantId);
            existing.CreationTime.ShouldBe(creationTime);
        }

        // CreateMap<TenantContactMap, TenantContactMapOutputDto>() -- by-name copy.
        [Fact]
        public void TenantContactMap_To_TenantContactMapOutputDto()
        {
            var id = Guid.NewGuid();
            var contactId = Guid.NewGuid();
            var creationTime = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);
            var map = new TenantContactMap(id)
            {
                ContactId = contactId,
                IsPrimary = true,
                DisplayOrder = 3,
                CreationTime = creationTime,
                TenantId = Guid.NewGuid()
            };

            var dto = _objectMapper.Map<TenantContactMap, TenantContactMapOutputDto>(map);

            dto.Id.ShouldBe(id);
            dto.ContactId.ShouldBe(contactId);
            dto.IsPrimary.ShouldBeTrue();
            dto.DisplayOrder.ShouldBe(3);
            dto.CreationTime.ShouldBe(creationTime);
            dto.LastModificationTime.ShouldBeNull();
        }

        // CreateMap<TenantContactMapInputDto, TenantContactMap>().IgnoreSourceMissingProperties()
        [Fact]
        public void TenantContactMapInputDto_To_TenantContactMap()
        {
            var contactId = Guid.NewGuid();
            var input = new TenantContactMapInputDto { ContactId = contactId, IsPrimary = true, DisplayOrder = 4 };

            var created = _objectMapper.Map<TenantContactMapInputDto, TenantContactMap>(input);

            created.ContactId.ShouldBe(contactId);
            created.IsPrimary.ShouldBeTrue();
            created.DisplayOrder.ShouldBe(4);
            created.Id.ShouldBe(Guid.Empty);
            created.TenantId.ShouldBe(Guid.Empty);
            created.CreationTime.ShouldBe(default);
            created.IsDeleted.ShouldBeFalse();

            var id = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var existing = new TenantContactMap(id) { TenantId = tenantId, DisplayOrder = 1 };

            _objectMapper.Map(input, existing);

            existing.ContactId.ShouldBe(contactId);
            existing.DisplayOrder.ShouldBe(4);
            existing.Id.ShouldBe(id);
            existing.TenantId.ShouldBe(tenantId);
        }

        // CreateMap<TenantPictureMapInputDto, TenantPictureMap>()
        //   Ignore: PictureRole; IgnoreSourceMissingProperties
        // (used in place by TenantAddOnAppService.AddOrUpdatePictureMapAsync)
        [Fact]
        public void TenantPictureMapInputDto_To_TenantPictureMap()
        {
            var pictureId = Guid.NewGuid();
            var input = new TenantPictureMapInputDto { PictureId = pictureId, RoleId = 2, DisplayOrder = 6 };

            var created = _objectMapper.Map<TenantPictureMapInputDto, TenantPictureMap>(input);

            created.PictureId.ShouldBe(pictureId);
            created.RoleId.ShouldBe(2);
            created.PictureRole.ShouldBe((TenantPictureRoleEnum)2);
            created.DisplayOrder.ShouldBe(6);
            created.Id.ShouldBe(Guid.Empty);
            created.TenantId.ShouldBe(Guid.Empty);
            created.CreationTime.ShouldBe(default);
            created.IsDeleted.ShouldBeFalse();

            var id = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var creationTime = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc);
            var existing = new TenantPictureMap(id) { TenantId = tenantId, CreationTime = creationTime, RoleId = 1 };

            _objectMapper.Map(input, existing);

            existing.PictureId.ShouldBe(pictureId);
            existing.RoleId.ShouldBe(2);
            existing.DisplayOrder.ShouldBe(6);
            existing.Id.ShouldBe(id);
            existing.TenantId.ShouldBe(tenantId);
            existing.CreationTime.ShouldBe(creationTime);
        }

        // CreateMap<TenantPictureMap, TenantPictureMapOutputDto>()
        //   Ignore: PictureRole (computed from RoleId on the DTO)
        [Fact]
        public void TenantPictureMap_To_TenantPictureMapOutputDto()
        {
            var id = Guid.NewGuid();
            var pictureId = Guid.NewGuid();
            var creationTime = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc);
            var modificationTime = new DateTime(2026, 7, 2, 0, 0, 0, DateTimeKind.Utc);
            var map = new TenantPictureMap(id)
            {
                PictureId = pictureId,
                RoleId = 3,
                DisplayOrder = 8,
                CreationTime = creationTime,
                LastModificationTime = modificationTime,
                TenantId = Guid.NewGuid()
            };

            var dto = _objectMapper.Map<TenantPictureMap, TenantPictureMapOutputDto>(map);

            dto.Id.ShouldBe(id);
            dto.PictureId.ShouldBe(pictureId);
            dto.RoleId.ShouldBe(3);
            dto.PictureRole.ShouldBe((TenantPictureRoleEnum)3);
            dto.DisplayOrder.ShouldBe(8);
            dto.CreationTime.ShouldBe(creationTime);
            dto.LastModificationTime.ShouldBe(modificationTime);
        }

        private static TenantAddOnInputDto NewAddOnInput(out Guid orderId, out Guid planId)
        {
            orderId = Guid.NewGuid();
            planId = Guid.NewGuid();
            return new TenantAddOnInputDto
            {
                IsAutoProvisioningEnabled = true,
                DisplayName = "Input",
                Description = "Input desc",
                CompanyTypeId = 1,
                CompanySize = 20,
                IndustryCodeId = 5,
                OrderId = orderId,
                PlanId = planId
            };
        }

        private static void AssertAddOnFields(TenantAddOn addOn, Guid orderId, Guid planId)
        {
            addOn.IsAutoProvisioningEnabled.ShouldBeTrue();
            addOn.DisplayName.ShouldBe("Input");
            addOn.Description.ShouldBe("Input desc");
            addOn.CompanyTypeId.ShouldBe(1);
            addOn.CompanySize.ShouldBe(20);
            addOn.IndustryCodeId.ShouldBe(5);
            addOn.OrderId.ShouldBe(orderId);
            addOn.PlanId.ShouldBe(planId);
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
