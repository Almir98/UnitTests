using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RentaCar.Data.Requests.Manufacturer;
using RentACar.WebAPI.Database;
using RentACar.WebAPI.Service;
using System.Collections.Generic;
using Xunit;

namespace RentCar.Tests
{
    public class ManufacturerTests
    {
        public ManufacturerService _manufacturerService;
        public RentaCarContext _context = new RentaCarContext();
        public IMapper _mapper;

        public ManufacturerTests()
        {
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new Rent_a_Car.WebAPI.Mappers.Mapper());
                });
                IMapper mapper = mappingConfig.CreateMapper();
                _mapper = mapper;
            }

            var options = new DbContextOptionsBuilder<RentaCarContext>()
                .UseInMemoryDatabase(databaseName: "RentCar").Options;

            _context = new RentaCarContext(options);
            _manufacturerService = new ManufacturerService(_context, _mapper);
        }

        [Fact]
        public void ShouldFilterEmpty_ReturnList()
        {
            //Arrange
            _context.Manufacturer.Add(new Manufacturer
            {
                ManufacturerId = 1,
                ManufacturerName = "Audi"
            });
            _context.Manufacturer.Add(new Manufacturer
            {
                ManufacturerId = 2,
                ManufacturerName = "BMW"
            });
            _context.Manufacturer.Add(new Manufacturer
            {
                ManufacturerId = 3,
                ManufacturerName = "Mercedes-Benz"
            });
            _context.Manufacturer.Add(new Manufacturer
            {
                ManufacturerId = 4,
                ManufacturerName = "Opel"
            });
            _context.Manufacturer.Add(new Manufacturer
            {
                ManufacturerId = 5,
                ManufacturerName = "Rolls-Royce"
            });
            _context.SaveChanges();

            //Act
            var list = _manufacturerService.Get(new ManufacturerSearchRequest());

            //Assert
            Assert.Equal(6, list.Count);
        }

        [Fact]
        public void ShouldFilterName_ReturnExistingName()
        {
            //Arrange
            ManufacturerSearchRequest request = new ManufacturerSearchRequest
            {
                ManufacturerName = "Audi"
            };

            //Act
            var result = _manufacturerService.Get(request);

            //Assert

            Assert.Equal("Audi", request.ManufacturerName);
            Assert.NotEmpty(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldGetById_ReturnObject()
        {
            //Arrange
            _context.Manufacturer.Add(new Manufacturer
            {
                ManufacturerId = 7,
                ManufacturerName = "Renault"
            });
            _context.SaveChanges();

            //Act
            var item = _manufacturerService.GetByID(7);

            //Assert
            Assert.Equal("Renault", item.ManufacturerName);
        }

        [Fact]
        public void ShouldInsert_ReturnObject()
        {
            //Arrange
            var request = new ManufacturerUpsert
            {
                ManufacturerName = "Tesla"
            };
            _context.SaveChanges();

            //Act
            var result = _manufacturerService.Insert(request);

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldUpdate_ReturnObject()
        {
            _context.Manufacturer.Add(new Manufacturer
            {
                ManufacturerId = 8,
                ManufacturerName = "Ford"
            });
            _context.SaveChanges();

            //Arrange
            var request = new ManufacturerUpsert
            {
                ManufacturerName = "Porsche"
            };
            var item = _manufacturerService.GetByID(8);

            //Act
            var result = _manufacturerService.Update(item.ManufacturerId, request);

            //Assert
            Assert.Equal("Porsche", result.ManufacturerName);
            Assert.NotNull(result);
        }


        [Fact]
        public void ShouldDelete_ReturnList()
        {
            //Arrange
            _context.Manufacturer.Add(new Manufacturer
            {
                ManufacturerId = 9,
                ManufacturerName = "Mazda"
            });
            _context.SaveChanges();

            //Act
            _manufacturerService.Delete(9);

            var list = _manufacturerService.Get(new ManufacturerSearchRequest());

            //Assert
            Assert.IsType<List<Data.Model.Manufacturer>>(list);
        }
    }
}
