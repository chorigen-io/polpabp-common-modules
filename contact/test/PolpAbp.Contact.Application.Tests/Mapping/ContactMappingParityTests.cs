using System;
using PolpAbp.Contact.Domain.Entities;
using PolpAbp.Contact.Dtos;
using Shouldly;
using Volo.Abp.Data;
using Volo.Abp.ObjectMapping;
using Xunit;

namespace PolpAbp.Contact.Mapping
{
    /// <summary>
    /// Mapping-parity tests (T026): one fact per pair of the former
    /// ContactApplicationAutoMapperProfile. Each fact encodes the AutoMapper
    /// semantics as explicit expected values: IgnoreSourceMissingProperties leaves
    /// every target member without a same-named source member untouched (default on
    /// a new object, existing value on an in-place map).
    /// </summary>
    public class ContactMappingParityTests : ContactApplicationTestBase
    {
        private readonly IObjectMapper _objectMapper;

        public ContactMappingParityTests()
        {
            _objectMapper = GetRequiredService<IObjectMapper<ContactApplicationModule>>();
        }

        // CreateMap<AddressInputDto, Address>().IgnoreSourceMissingProperties()
        [Fact]
        public void AddressInputDto_To_Address()
        {
            var input = NewAddressInput(out var countryId, out var stateId);

            var address = _objectMapper.Map<AddressInputDto, Address>(input);

            AssertAddressFields(address, countryId, stateId);
            // Not in the source: defaults of a new Address
            address.Id.ShouldBe(Guid.Empty);
            address.TenantId.ShouldBeNull();
            address.OwnerId.ShouldBeNull();
            address.RoleId.ShouldBe(0);
            address.ExtraProperties.ShouldBeEmpty();
        }

        // Same pair, in-place form (AddressAppService update): members missing from the
        // source keep their existing values.
        [Fact]
        public void AddressInputDto_To_Existing_Address_Preserves_Unmapped()
        {
            var id = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var creationTime = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            var address = new Address(id)
            {
                TenantId = tenantId,
                OwnerId = ownerId,
                RoleId = 4,
                City = "old-city",
                ConcurrencyStamp = "stamp-1"
            };
            SetProperty(address, nameof(Address.CreationTime), creationTime);
            address.SetProperty("Keep", "kept");
            var input = NewAddressInput(out var countryId, out var stateId);

            _objectMapper.Map(input, address);

            AssertAddressFields(address, countryId, stateId);
            address.Id.ShouldBe(id);
            address.TenantId.ShouldBe(tenantId);
            address.OwnerId.ShouldBe(ownerId);
            address.RoleId.ShouldBe(4);
            address.CreationTime.ShouldBe(creationTime);
            address.ConcurrencyStamp.ShouldBe("stamp-1");
            address.ExtraProperties.Count.ShouldBe(1);
            address.ExtraProperties["Keep"].ShouldBe("kept");
        }

        // CreateMap<Address, AddressOutputDto>() -- by-name copy.
        [Fact]
        public void Address_To_AddressOutputDto()
        {
            var id = Guid.NewGuid();
            var countryId = Guid.NewGuid();
            var stateId = Guid.NewGuid();
            var creationTime = new DateTime(2026, 2, 3, 4, 5, 6, DateTimeKind.Utc);
            var address = new Address(id)
            {
                CountryId = countryId,
                StateProvinceId = stateId,
                StateProvinceName = "State",
                City = "City",
                County = "County",
                StreeNo = 12,
                Address1 = "Line 1",
                Address2 = "Line 2",
                ZipCode = "12345",
                TenantId = Guid.NewGuid(),
                OwnerId = Guid.NewGuid(),
                RoleId = 3
            };
            SetProperty(address, nameof(Address.CreationTime), creationTime);

            var dto = _objectMapper.Map<Address, AddressOutputDto>(address);

            dto.Id.ShouldBe(id);
            dto.CreationTime.ShouldBe(creationTime);
            dto.CountryId.ShouldBe(countryId);
            dto.StateProvinceId.ShouldBe(stateId);
            dto.StateProvinceName.ShouldBe("State");
            dto.City.ShouldBe("City");
            dto.County.ShouldBe("County");
            dto.StreeNo.ShouldBe(12);
            dto.Address1.ShouldBe("Line 1");
            dto.Address2.ShouldBe("Line 2");
            dto.ZipCode.ShouldBe("12345");
        }

        // CreateMap<AddressOutputDto, AddressInputDto>().IgnoreSourceMissingProperties()
        [Fact]
        public void AddressOutputDto_To_AddressInputDto()
        {
            var countryId = Guid.NewGuid();
            var stateId = Guid.NewGuid();
            var output = new AddressOutputDto
            {
                Id = Guid.NewGuid(),
                CreationTime = DateTime.UtcNow,
                CountryId = countryId,
                StateProvinceId = stateId,
                StateProvinceName = "State",
                City = "City",
                County = "County",
                StreeNo = 7,
                Address1 = "Line 1",
                Address2 = "Line 2",
                ZipCode = "54321"
            };

            var input = _objectMapper.Map<AddressOutputDto, AddressInputDto>(output);

            input.ShouldNotBeOfType<AddressOutputDto>();
            input.CountryId.ShouldBe(countryId);
            input.StateProvinceId.ShouldBe(stateId);
            input.StateProvinceName.ShouldBe("State");
            input.City.ShouldBe("City");
            input.County.ShouldBe("County");
            input.StreeNo.ShouldBe(7);
            input.Address1.ShouldBe("Line 1");
            input.Address2.ShouldBe("Line 2");
            input.ZipCode.ShouldBe("54321");
        }

        // CreateMap<ContactCardInputDto, ContactCard>().IgnoreSourceMissingProperties()
        [Fact]
        public void ContactCardInputDto_To_ContactCard()
        {
            var input = NewContactCardInput();

            var card = _objectMapper.Map<ContactCardInputDto, ContactCard>(input);

            AssertContactCardFields(card);
            card.Id.ShouldBe(Guid.Empty);
            card.TenantId.ShouldBeNull();
            card.OwnerId.ShouldBeNull();
            card.RoleId.ShouldBe(0);
            card.ExtraProperties.ShouldBeEmpty();
        }

        // Same pair, in-place form (ContactCardAppService update).
        [Fact]
        public void ContactCardInputDto_To_Existing_ContactCard_Preserves_Unmapped()
        {
            var id = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var creationTime = new DateTime(2026, 3, 4, 5, 6, 7, DateTimeKind.Utc);
            var card = new ContactCard(id)
            {
                TenantId = tenantId,
                OwnerId = ownerId,
                RoleId = 9,
                FirstName = "Old",
                ConcurrencyStamp = "stamp-2"
            };
            SetProperty(card, nameof(ContactCard.CreationTime), creationTime);
            card.SetProperty("Keep", "kept");

            _objectMapper.Map(NewContactCardInput(), card);

            AssertContactCardFields(card);
            card.Id.ShouldBe(id);
            card.TenantId.ShouldBe(tenantId);
            card.OwnerId.ShouldBe(ownerId);
            card.RoleId.ShouldBe(9);
            card.CreationTime.ShouldBe(creationTime);
            card.ConcurrencyStamp.ShouldBe("stamp-2");
            card.ExtraProperties.Count.ShouldBe(1);
            card.ExtraProperties["Keep"].ShouldBe("kept");
        }

        // CreateMap<ContactCard, ContactCardOutputDto>() -- by-name copy.
        [Fact]
        public void ContactCard_To_ContactCardOutputDto()
        {
            var id = Guid.NewGuid();
            var creationTime = new DateTime(2026, 4, 5, 6, 7, 8, DateTimeKind.Utc);
            var card = new ContactCard(id)
            {
                FirstName = "Ada",
                LastName = "Lovelace",
                Email = "ada@example.com",
                PhoneCountryCode = "+44",
                PhoneNumber = "5550100",
                EmailConfirmed = true,
                PhoneConfirmed = true,
                TenantId = Guid.NewGuid(),
                OwnerId = Guid.NewGuid(),
                RoleId = 2
            };
            SetProperty(card, nameof(ContactCard.CreationTime), creationTime);

            var dto = _objectMapper.Map<ContactCard, ContactCardOutputDto>(card);

            dto.Id.ShouldBe(id);
            dto.CreationTime.ShouldBe(creationTime);
            dto.FirstName.ShouldBe("Ada");
            dto.LastName.ShouldBe("Lovelace");
            dto.Email.ShouldBe("ada@example.com");
            dto.PhoneCountryCode.ShouldBe("+44");
            dto.PhoneNumber.ShouldBe("5550100");
            dto.EmailConfirmed.ShouldBeTrue();
            dto.PhoneConfirmed.ShouldBeTrue();
        }

        // CreateMap<ContactCardOutputDto, ContactCardInputDto>().IgnoreSourceMissingProperties()
        [Fact]
        public void ContactCardOutputDto_To_ContactCardInputDto()
        {
            var output = new ContactCardOutputDto
            {
                Id = Guid.NewGuid(),
                CreationTime = DateTime.UtcNow,
                FirstName = "Grace",
                LastName = "Hopper",
                Email = "grace@example.com",
                PhoneCountryCode = "+1",
                PhoneNumber = "5550199",
                EmailConfirmed = true,
                PhoneConfirmed = false
            };

            var input = _objectMapper.Map<ContactCardOutputDto, ContactCardInputDto>(output);

            input.ShouldNotBeOfType<ContactCardOutputDto>();
            input.FirstName.ShouldBe("Grace");
            input.LastName.ShouldBe("Hopper");
            input.Email.ShouldBe("grace@example.com");
            input.PhoneCountryCode.ShouldBe("+1");
            input.PhoneNumber.ShouldBe("5550199");
            input.EmailConfirmed.ShouldBeTrue();
            input.PhoneConfirmed.ShouldBeFalse();
        }

        private static AddressInputDto NewAddressInput(out Guid countryId, out Guid stateId)
        {
            countryId = Guid.NewGuid();
            stateId = Guid.NewGuid();
            return new AddressInputDto
            {
                CountryId = countryId,
                StateProvinceId = stateId,
                StateProvinceName = "Input State",
                City = "Input City",
                County = "Input County",
                StreeNo = 42,
                Address1 = "Input 1",
                Address2 = "Input 2",
                ZipCode = "99999"
            };
        }

        private static void AssertAddressFields(Address address, Guid countryId, Guid stateId)
        {
            address.CountryId.ShouldBe(countryId);
            address.StateProvinceId.ShouldBe(stateId);
            address.StateProvinceName.ShouldBe("Input State");
            address.City.ShouldBe("Input City");
            address.County.ShouldBe("Input County");
            address.StreeNo.ShouldBe(42);
            address.Address1.ShouldBe("Input 1");
            address.Address2.ShouldBe("Input 2");
            address.ZipCode.ShouldBe("99999");
        }

        private static ContactCardInputDto NewContactCardInput()
        {
            return new ContactCardInputDto
            {
                FirstName = "First",
                LastName = "Last",
                Email = "first.last@example.com",
                PhoneCountryCode = "+33",
                PhoneNumber = "5550123",
                EmailConfirmed = true,
                PhoneConfirmed = false
            };
        }

        private static void AssertContactCardFields(ContactCard card)
        {
            card.FirstName.ShouldBe("First");
            card.LastName.ShouldBe("Last");
            card.Email.ShouldBe("first.last@example.com");
            card.PhoneCountryCode.ShouldBe("+33");
            card.PhoneNumber.ShouldBe("5550123");
            card.EmailConfirmed.ShouldBeTrue();
            card.PhoneConfirmed.ShouldBeFalse();
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
