using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RentaCar.Data.Requests.Booking;
using RentACar.WebAPI.Database;
using RentACar.WebAPI.Service;
using System;
using System.Collections.Generic;
using Xunit;

namespace RentCar.Tests
{
    public class BookingTests
    {
        public BookingService _bookingService;
        public RentaCarContext _context = new RentaCarContext();
        public IMapper _mapper;

        public BookingTests()
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
            _bookingService = new BookingService(_context, _mapper);
        }

        [Fact]
        public void ShouldFilterEmpty_ReturnList()
        {
            //Arrange
            _context.Booking.Add(new Booking
            {
                BookingId = 4,
                StartDate = DateTime.UtcNow.AddDays(-3),
                EndDate = DateTime.UtcNow,
                CustomerId = 1,
                VehicleId = 1,
                RatingStatus = false,
                CommentStatus = false
            });
            _context.Booking.Add(new Booking
            {
                BookingId = 5,
                StartDate = DateTime.UtcNow.AddDays(-2),
                EndDate = DateTime.UtcNow,
                CustomerId = 2,
                VehicleId = 2,
                RatingStatus = false,
                CommentStatus = false
            });
            _context.SaveChanges();
            //Act

            var list = _bookingService.Get(new BookingSearchRequest());

            //Assert
            Assert.NotNull(list);
            Assert.IsType<List<Data.Model.Booking>>(list);
        }

        [Fact]
        public void ShouldFilterByDate_ReturnList()
        {
            //Arrange
            BookingSearchRequest request = new BookingSearchRequest
            {
                StartDate = DateTime.UtcNow.AddDays(-3),
                EndDate = DateTime.UtcNow
            };

            //Act
            var item = _bookingService.Get(request);

            //Assert
            Assert.NotNull(item);
            Assert.IsType<List<Data.Model.Booking>>(item);
        }

        [Fact]
        public void ShouldFilterByVehicleId_ReturnList()
        {
            //Arrange
            BookingSearchRequest request = new BookingSearchRequest
            {
                VehicleID = 8
            };

            //Act
            var item = _bookingService.Get(request);

            //Assert
            Assert.Empty(item);
        }

        [Fact]
        public void ShouldInsert_ReturnNotEmpty()
        {
            //Arrange
            BookingUpsert request = new BookingUpsert
            {
                CustomerId = 6,
                VehicleId = 4,
                StartDate = DateTime.UtcNow.AddDays(-2),
                EndDate = DateTime.UtcNow,
                RatingStatus = false,
                CommentStatus = false
            };

            //Act
            var result = _bookingService.Insert(request);

            var list = _bookingService.Get(new BookingSearchRequest());

            //Assert
            Assert.IsType<List<Data.Model.Booking>>(list);
        }

        [Fact]
        public void ShouldUpdateStatus_ReturnTrueStatuses()
        {
            //Arrange
            BookingUpsert request = new BookingUpsert
            {
                RatingStatus = true,
                CommentStatus = true
            };

            //Act
            var item = _bookingService.GetByID(4);

            var result = _bookingService.Update(item.BookingId, request);

            //Assert
            Assert.Equal(true, result.RatingStatus);
            Assert.Equal(true, result.CommentStatus);
        }

        [Fact]
        public void ShouldDeleteBooking_ReturnOk()
        {
            _context.Booking.Add(new Booking
            {
                BookingId = 7,
                StartDate = DateTime.UtcNow.AddDays(-2),
                EndDate = DateTime.UtcNow,
                CustomerId = 2,
                VehicleId = 2,
                RatingStatus = false,
                CommentStatus = false
            });
            _context.SaveChanges();

            //Arrange
            _bookingService.Delete(7);
            _context.SaveChanges();

            //Act
            var list = _bookingService.Get(new BookingSearchRequest());

            //Assert
            Assert.NotNull(list);
            Assert.Equal(3, list.Count);
        }
    }
}
