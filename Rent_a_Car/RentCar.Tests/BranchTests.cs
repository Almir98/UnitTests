using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RentaCar.Data.Requests.Branch;
using RentACar.WebAPI.Database;
using RentACar.WebAPI.Service;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace RentCar.Tests
{
    public class BranchTests
    {
        public BranchService _branchService;
        public RentaCarContext _context = new RentaCarContext();
        public IMapper _mapper;

        public BranchTests()
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
            _branchService = new BranchService(_context, _mapper);
        }

        [Fact]
        public void Get_FilterEmpty_ReturnOk()
        {
            //Arrange
            _context.Branch.Add(new Branch
            {
                BranchId = 1,
                BranchName = "Kramar",
                PhoneNumber = "2225111",
                Adress = "Test adresa1",
                OpenTime = "08:00",
                CloseTime = "17:00",
                Description = "Opis1",
                CityId = 1
            });
            _context.Branch.Add(new Branch
            {
                BranchId = 2,
                BranchName = "GumaM",
                PhoneNumber = "123456",
                Adress = "Test adresa2",
                OpenTime = "08:00",
                CloseTime = "17:00",
                Description = "Opis2",
                CityId = 2
            });
            _context.Branch.Add(new Branch
            {
                BranchId = 3,
                BranchName = "AutoAdo",
                PhoneNumber = "225366",
                Adress = "Test adresa 3",
                OpenTime = "08:00",
                CloseTime = "17:00",
                Description = "Opis3",
                CityId = 3
            });
            _context.SaveChanges();

            BranchSearchRequest request = new BranchSearchRequest();

            //Act
            var list = _branchService.Get(request);

            //Assert
            Assert.NotEmpty(list);
            Assert.Equal(3, list.Count);
            Assert.IsType<List<Data.Model.Branch>>(list);
        }


        [Fact]
        public void Insert_NewBranch_ReturnOk()
        {
            //Arrange
            BranchUpsert request = new BranchUpsert
            {
                BranchName = "test name",
                PhoneNumber = "123456",
                Adress = "Test ulica",
                OpenTime = "08:00",
                CloseTime = "17:00",
                Description = "description",
                CityId = 4
            };

            //Act
            var result = _branchService.Insert(request);

            var list = _branchService.Get(new BranchSearchRequest());

            //Assert
            Assert.Equal(4, list.Count);
        }


        [Fact]
        public void GetById_ShouldGetBranch_ReturnOk()
        {
            //Arrange
            _context.Branch.Add(new Branch
            {
                BranchId = 5,
                BranchName = "Buba centar",
                PhoneNumber = "225366",
                Adress = "Test adresa 3",
                OpenTime = "08:00",
                CloseTime = "17:00",
                Description = "Opis4",
                CityId = 4
            });
            _context.SaveChanges();

            //Act
            var item = _branchService.GetByID(5);

            //Assert
            Assert.IsType<Data.Model.Branch>(item);
        }

        [Fact]
        public void Delete_ShouldRemove_ReturnOk()
        {
            //Arrange
            _context.Branch.Add(new Branch
            {
                BranchId = 6,
                BranchName = "Skoda centar",
                PhoneNumber = "2253243",
                Adress = "Test adresa 4",
                OpenTime = "08:00",
                CloseTime = "17:00",
                Description = "Opis2",
                CityId = 5
            });
            _context.SaveChanges();

            //Act
            _branchService.Delete(6);
            var list = _branchService.Get(new BranchSearchRequest());

            //Assert
            Assert.IsType<List<Data.Model.Branch>>(list);
        }
    }

}
