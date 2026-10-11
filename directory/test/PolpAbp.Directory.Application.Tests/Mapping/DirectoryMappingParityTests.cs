using System;
using System.Collections.Generic;
using PolpAbp.Directory.Domain.Entities;
using PolpAbp.Directory.Dtos;
using Shouldly;
using Volo.Abp.Data;
using Volo.Abp.ObjectMapping;
using Xunit;

namespace PolpAbp.Directory.Mapping
{
    /// <summary>
    /// Mapping-parity tests (T026): one fact per pair of the former
    /// DirectoryApplicationAutoMapperProfile. Each fact encodes the AutoMapper
    /// semantics as explicit expected values: IgnoreSourceMissingProperties leaves
    /// every target member without a same-named source member untouched (default on
    /// a new object, existing value on an in-place map).
    /// </summary>
    public class DirectoryMappingParityTests : DirectoryApplicationTestBase
    {
        private readonly IObjectMapper _objectMapper;

        public DirectoryMappingParityTests()
        {
            _objectMapper = GetRequiredService<IObjectMapper<DirectoryApplicationModule>>();
        }

        // CreateMap<CountryInputDto, Country>().IgnoreSourceMissingProperties()
        [Fact]
        public void CountryInputDto_To_Country()
        {
            var country = _objectMapper.Map<CountryInputDto, Country>(NewCountryInput());

            AssertCountryFields(country);
            // Not in the source: defaults of a new Country (parameterless constructor)
            country.Id.ShouldBe(Guid.Empty);
            country.CreationTime.ShouldBe(default);
            country.StateProvinces.ShouldNotBeNull();
            country.StateProvinces.ShouldBeEmpty();
            country.ExtraProperties.ShouldBeEmpty();
        }

        // Same pair, in-place form (CountryAppService create/update): members missing
        // from the source keep their existing values.
        [Fact]
        public void CountryInputDto_To_Existing_Country_Preserves_Unmapped()
        {
            var id = Guid.NewGuid();
            var creationTime = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            var province = new StateProvince(Guid.NewGuid()) { Name = "Kept" };
            var country = new Country(id)
            {
                CreationTime = creationTime,
                Name = "Old",
                ConcurrencyStamp = "stamp-1",
                StateProvinces = new List<StateProvince> { province }
            };
            country.SetProperty("Keep", "kept");

            _objectMapper.Map(NewCountryInput(), country);

            AssertCountryFields(country);
            country.Id.ShouldBe(id);
            country.CreationTime.ShouldBe(creationTime);
            country.ConcurrencyStamp.ShouldBe("stamp-1");
            country.StateProvinces.Count.ShouldBe(1);
            country.StateProvinces[0].ShouldBeSameAs(province);
            country.ExtraProperties.Count.ShouldBe(1);
            country.ExtraProperties["Keep"].ShouldBe("kept");
        }

        // CreateMap<Country, CountryOutputDto>() -- by-name copy.
        [Fact]
        public void Country_To_CountryOutputDto()
        {
            var id = Guid.NewGuid();
            var country = new Country(id)
            {
                Name = "Canada",
                TwoLetterIsoCode = "CA",
                ThreeLetterIsoCode = "CAN",
                NumbericIsoCode = 124
            };

            var dto = _objectMapper.Map<Country, CountryOutputDto>(country);

            dto.Id.ShouldBe(id);
            dto.Name.ShouldBe("Canada");
            dto.TwoLetterIsoCode.ShouldBe("CA");
            dto.ThreeLetterIsoCode.ShouldBe("CAN");
            dto.NumbericIsoCode.ShouldBe(124);
        }

        // CreateMap<CountryOutputDto, CountryInputDto>().IgnoreSourceMissingProperties()
        [Fact]
        public void CountryOutputDto_To_CountryInputDto()
        {
            var output = new CountryOutputDto
            {
                Id = Guid.NewGuid(),
                Name = "Japan",
                TwoLetterIsoCode = "JP",
                ThreeLetterIsoCode = "JPN",
                NumbericIsoCode = 392
            };

            var input = _objectMapper.Map<CountryOutputDto, CountryInputDto>(output);

            input.ShouldNotBeOfType<CountryOutputDto>();
            input.Name.ShouldBe("Japan");
            input.TwoLetterIsoCode.ShouldBe("JP");
            input.ThreeLetterIsoCode.ShouldBe("JPN");
            input.NumbericIsoCode.ShouldBe(392);
        }

        // CreateMap<StateProvinceInputDto, StateProvince>().IgnoreSourceMissingProperties()
        [Fact]
        public void StateProvinceInputDto_To_StateProvince()
        {
            var province = _objectMapper.Map<StateProvinceInputDto, StateProvince>(
                new StateProvinceInputDto { Name = "Ontario", Abbreviation = "ON" });

            province.Name.ShouldBe("Ontario");
            province.Abbreviation.ShouldBe("ON");
            province.Id.ShouldBe(Guid.Empty);
            province.CountryId.ShouldBe(Guid.Empty);
            province.CreationTime.ShouldBe(default);
        }

        // Same pair, in-place form (CountryAppService state/province update).
        [Fact]
        public void StateProvinceInputDto_To_Existing_StateProvince_Preserves_Unmapped()
        {
            var id = Guid.NewGuid();
            var countryId = Guid.NewGuid();
            var creationTime = new DateTime(2026, 2, 3, 4, 5, 6, DateTimeKind.Utc);
            var province = new StateProvince(id)
            {
                CountryId = countryId,
                CreationTime = creationTime,
                Name = "Old",
                Abbreviation = "OL"
            };

            _objectMapper.Map(new StateProvinceInputDto { Name = "Quebec", Abbreviation = "QC" }, province);

            province.Name.ShouldBe("Quebec");
            province.Abbreviation.ShouldBe("QC");
            province.Id.ShouldBe(id);
            province.CountryId.ShouldBe(countryId);
            province.CreationTime.ShouldBe(creationTime);
        }

        // CreateMap<StateProvince, StateProvinceOutputDto>() -- by-name copy.
        [Fact]
        public void StateProvince_To_StateProvinceOutputDto()
        {
            var id = Guid.NewGuid();
            var province = new StateProvince(id)
            {
                CountryId = Guid.NewGuid(),
                Name = "Alberta",
                Abbreviation = "AB"
            };

            var dto = _objectMapper.Map<StateProvince, StateProvinceOutputDto>(province);

            dto.Id.ShouldBe(id);
            dto.Name.ShouldBe("Alberta");
            dto.Abbreviation.ShouldBe("AB");
        }

        // CreateMap<StateProvinceOutputDto, StateProvinceInputDto>().IgnoreSourceMissingProperties()
        [Fact]
        public void StateProvinceOutputDto_To_StateProvinceInputDto()
        {
            var output = new StateProvinceOutputDto
            {
                Id = Guid.NewGuid(),
                Name = "Manitoba",
                Abbreviation = "MB"
            };

            var input = _objectMapper.Map<StateProvinceOutputDto, StateProvinceInputDto>(output);

            input.ShouldNotBeOfType<StateProvinceOutputDto>();
            input.Name.ShouldBe("Manitoba");
            input.Abbreviation.ShouldBe("MB");
        }

        private static CountryInputDto NewCountryInput()
        {
            return new CountryInputDto
            {
                Name = "Input Country",
                TwoLetterIsoCode = "IC",
                ThreeLetterIsoCode = "INC",
                NumbericIsoCode = 999
            };
        }

        private static void AssertCountryFields(Country country)
        {
            country.Name.ShouldBe("Input Country");
            country.TwoLetterIsoCode.ShouldBe("IC");
            country.ThreeLetterIsoCode.ShouldBe("INC");
            country.NumbericIsoCode.ShouldBe(999);
        }
    }
}
