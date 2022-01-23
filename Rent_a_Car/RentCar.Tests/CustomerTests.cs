using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Rent_a_Car.WebAPI.Exceptions;
using RentaCar.Data.Requests.Customer;
using RentACar.WebAPI.Database;
using RentACar.WebAPI.Service;
using System.Collections.Generic;
using Xunit;

namespace RentCar.Tests
{
    public class CustomerTests
    {
        public CustomerService _customerService;
        public RentaCarContext _context = new RentaCarContext();
        public IMapper _mapper;

        public CustomerTests()
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
            _customerService = new CustomerService(_context, _mapper);
        }

        [Fact]
        public void ShouldFilterEmpty_ReturnList()
        {
            //Arrange
            _context.Customer.Add(new Customer
            {
                CustomerId = 1,
                FirstName = "Ime1",
                LastName = "Prezime1",
                Phone = "030-111-111",
                Email = "test1@outlook.com",
                Username = "testUsername1",
                PasswordHash = "",
                PasswordSalt = "",
                CityId = 1,
                CustomerTypeId = 1
            });
            _context.Customer.Add(new Customer
            {
                CustomerId = 2,
                FirstName = "Ime2",
                LastName = "Prezime2",
                Phone = "030-222-222",
                Email = "test2@outlook.com",
                Username = "testUsername2",
                PasswordHash = "",
                PasswordSalt = "",
                CityId = 2,
                CustomerTypeId = 2
            });
            _context.Customer.Add(new Customer
            {
                CustomerId = 3,
                FirstName = "Ime3",
                LastName = "Prezime3",
                Phone = "030-333-333",
                Email = "test3@outlook.com",
                Username = "testUsername3",
                PasswordHash = "",
                PasswordSalt = "",
                CityId = 3,
                CustomerTypeId = 3
            });
            _context.SaveChanges();

            //Act

            var list = _customerService.Get(new CustomerSearchRequest());

            //Assert

            Assert.Equal(list.Count, _context.Customer.Local.Count);
            Assert.IsType<List<Data.Model.Customer>>(list);
            Assert.NotEmpty(list);
        }

        [Fact]
        public void ShouldFilterByFirstName_ReturnObject()
        {
            //Arrange
            _context.Customer.Add(new Customer
            {
                CustomerId = 4,
                FirstName = "Ime4",
                LastName = "Prezime4",
                Phone = "030-444-444",
                Email = "test4@outlook.com",
                Username = "testUsername4",
                PasswordHash = "",
                PasswordSalt = "",
                CityId = 4,
                CustomerTypeId = 4
            });
            _context.SaveChanges();

            //Act
            var request = new CustomerSearchRequest
            {
                FirstName = "Ime4"
            };

            var item = _customerService.Get(request);

            //Assert
            Assert.NotEmpty(item);
            Assert.Equal("Ime4", item[0].FirstName);
        }

        [Fact]
        public void ShouldFilterByLastName_ReturnObject()
        {
            //Arrange
            _context.Customer.Add(new Customer
            {
                CustomerId = 5,
                FirstName = "Ime5",
                LastName = "Prezime5",
                Phone = "030-555-555",
                Email = "test5@outlook.com",
                Username = "testUsername5",
                PasswordHash = "",
                PasswordSalt = "",
                CityId = 5,
                CustomerTypeId = 5
            });
            _context.SaveChanges();

            //Act
            var request = new CustomerSearchRequest
            {
                LastName = "Prezime5"
            };

            var item = _customerService.Get(request);

            //Assert
            Assert.NotEmpty(item);
            Assert.IsType<List<Data.Model.Customer>>(item);
            Assert.Equal("Prezime4", item[0].LastName);
        }

        [Fact]
        public void ShouldCustomerRequest_ReturnEmptyList()
        {
            //Act
            var request = new CustomerSearchRequest
            {
                FirstName = "Random",
                LastName = "Random"
            };

            var list = _customerService.Get(request);

            //Assert

            Assert.IsType<List<Data.Model.Customer>>(list);
            Assert.Empty(list);
        }

        [Fact]
        public void ShouldGetByEmail_ReturnObject()
        {
            //Arrange
            _context.Customer.Add(new Customer
            {
                CustomerId = 6,
                FirstName = "Ime6",
                LastName = "Prezime6",
                Phone = "030-666-666",
                Email = "test6@outlook.com",
                Username = "testUsername6",
                PasswordHash = "",
                PasswordSalt = "",
                CityId = 6,
                CustomerTypeId = 6
            });
            _context.SaveChanges();

            var request = new CustomerSearchRequest
            {
                Email = "test6@outlook.com"
            };

            //Act
            var list = _customerService.Get(request);

            //Assert

            Assert.Equal("test6@outlook.com", list[0].Email);
            Assert.NotEmpty(list[0].Email);
            Assert.IsType<List<Data.Model.Customer>>(list);
        }

        [Fact]
        public void ShouldValidFormatPhone_ReturnObject()
        {
            //Arrange
            _context.Customer.Add(new Customer
            {
                CustomerId = 7,
                FirstName = "Ime7",
                LastName = "Prezime7",
                Phone = "030-777-777",
                Email = "test7@outlook.com",
                Username = "testUsername7",
                PasswordHash = "",
                PasswordSalt = "",
                CityId = 7,
                CustomerTypeId = 7
            });
            _context.SaveChanges();

            //Act
            var item = _customerService.GetById(7);

            //Assert
            Assert.Contains("-", item.Phone);
            Assert.IsType<Data.Model.Customer>(item);
            Assert.NotEmpty(item.Phone);
        }

        [Fact]
        public void ShouldValidFormatEmail_ReturnObject()
        {
            //Arrange
            _context.Customer.Add(new Customer
            {
                CustomerId = 8,
                FirstName = "Ime8",
                LastName = "Prezime8",
                Phone = "030-888-888",
                Email = "test8@outlook.com",
                Username = "testUsername8",
                PasswordHash = "",
                PasswordSalt = "",
                CityId = 8,
                CustomerTypeId = 8
            });
            _context.SaveChanges();

            //Act
            var item = _customerService.GetById(7);

            //Assert
            Assert.Contains("-", item.Email);
            Assert.IsType<Data.Model.Customer>(item);
            Assert.NotEmpty(item.Email);
        }


        [Fact]
        public void ShouldInsertPasswordsNotMatch_ReturnNotMatched()
        {
            //Arrange
            var request = new CustomerUpsert
            {
                FirstName = "Ime8",
                LastName = "Prezime8",
                Phone = "030-888-888",
                Email = "test8@outlook.com",
                Username = "testUsername8",
                Password = "password1",
                PasswordConfirm = "password2",
                CityId = 8,
                CustomerTypeId = 8
            };
            _context.SaveChanges();

            //Assert
            Assert.Throws<UserException>(() => _customerService.Insert(request));
        }

        [Fact]
        public void ShouldInsertCustomer_ReturnObject()
        {
            //Arrange
            var request = new CustomerUpsert
            {
                FirstName = "Ime9",
                LastName = "Prezime9",
                Phone = "030-999-999",
                Email = "test9@outlook.com",
                Username = "testUsername9",
                Password = "password9",
                PasswordConfirm = "password9",
                CityId = 9,
                CustomerTypeId = 9
            };
            _context.SaveChanges();

            //Act
            var item = _customerService.Insert(request);

            //Assert
            Assert.NotNull(item);
        }

        //[Fact]
        //public void ShouldInsertCustomer_ReturnObject()
        //{
        //    //Arrange


        //    //Act


        //    //Assert
        //}

        //[Fact]
        //public void ShouldInsertCustomer_ReturnObject()
        //{
        //    //Arrange


        //    //Act


        //    //Assert
        //}

        //[Fact]
        //public void ShouldInsertCustomer_ReturnObject()
        //{
        //    //Arrange


        //    //Act


        //    //Assert
        //}
    }
}
