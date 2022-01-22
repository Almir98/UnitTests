using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RentaCar.Data.Requests.Comments;
using RentACar.WebAPI.Database;
using RentACar.WebAPI.Service;
using System;
using System.Collections.Generic;
using Xunit;

namespace RentCar.Tests
{
    public class CommentTests
    {
        public CommentService _commentService;
        public RentaCarContext _context = new RentaCarContext();
        public IMapper _mapper;

        public CommentTests()
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
            _commentService = new CommentService(_context, _mapper);
        }

        [Fact]
        public void ShouldFilterEmpty_ReturnList()
        {
            //Arrange
            _context.Comment.Add(new Comment
            {
                CommentId = 1,
                Description = "Komentar broj 1",
                DateOfComment = DateTime.UtcNow,
                CustomerId = 1,
                VehicleId = 1
            });
            _context.Comment.Add(new Comment
            {
                CommentId = 2,
                Description = "Komentar broj 2",
                DateOfComment = DateTime.UtcNow,
                CustomerId = 2,
                VehicleId = 2
            });
            _context.Comment.Add(new Comment
            {
                CommentId = 3,
                Description = "Komentar broj 3",
                DateOfComment = DateTime.UtcNow,
                CustomerId = 3,
                VehicleId = 3
            });
            _context.SaveChanges();

            //Act
            var list = _commentService.Get(new CommentSearchRequest());

            //Assert
            Assert.IsType<List<Data.Model.Comment>>(list);
            Assert.Equal(list.Count, _context.Comment.Local.Count);
        }

        [Fact]
        public void ShouldFilterByDateOfComment_ReturnList()
        {
            //Arrange
            _context.Comment.Add(new Comment
            {
                CommentId = 4,
                Description = "Komentar broj 4",
                DateOfComment = DateTime.UtcNow.AddDays(-3),
                CustomerId = 4,
                VehicleId = 4
            });
            _context.SaveChanges();

            //Act
            var request = new CommentSearchRequest
            {
                DateOfComment = DateTime.UtcNow.AddDays(-3)
            };

            var item = _commentService.Get(request);

            //Assert
            Assert.Single(item);
            Assert.IsType<List<Data.Model.Comment>>(item);
        }

        [Fact]
        public void ShouldFilterByCustomerId_ReturnList()
        {
            //Arrange
            _context.Comment.Add(new Comment
            {
                CommentId = 7,
                Description = "Komentar broj 7",
                DateOfComment = DateTime.UtcNow.AddDays(-5),
                CustomerId = 7,
                VehicleId = 7
            });
            _context.SaveChanges();

            //Act
            var request = new CommentSearchRequest
            {
                CustomerID = 7
            };
            var item = _commentService.Get(request);

            //Assert
            Assert.NotNull(item);
            Assert.IsType<List<Data.Model.Comment>>(item);
        }

        [Fact]
        public void ShouldAddComment_ReturnObject()
        {
            //Arrange
            CommentUpsert request = new CommentUpsert
            {
                Description = "Komentar broj 6",
                DateOfComment = DateTime.UtcNow,
                CustomerId = 6,
                VehicleId = 6,
                CommentStatus = false
            };

            //Act
            var result = _commentService.Insert(request);

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldUpdateComment_ReturnObject()
        {
            //Act
            var request = new CommentUpsert
            {
                Description = "Novi update opis"
            };

            var item = _commentService.GetByID(7);
            var result = _commentService.Update(item.CustomerId, request);

            //Assert
            Assert.Equal("Novi update opis", result.Description);
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldDeleteComment_ReturnObject()
        {
            //Arrange
            _context.Comment.Add(new Comment
            {
                CommentId = 6,
                Description = "Komentar broj 6",
                DateOfComment = DateTime.UtcNow,
                CustomerId = 6,
                VehicleId = 6
            });
            _context.SaveChanges();

            //Act
            _commentService.Delete(6);
            var result = _commentService.Get(new CommentSearchRequest());

            //Assert
            Assert.NotNull(result);
        }

        //[Fact]
        public void ShouldGetById_ReturnObject()
        {
            //Arrange
            _context.Comment.Add(new Comment
            {
                CommentId = 8,
                Description = "Komentar broj 8",
                DateOfComment = DateTime.UtcNow,
                CustomerId = 8,
                VehicleId = 8
            });
            _context.SaveChanges();

            //Act
            var result = _commentService.GetByID(8);


            //Assert

        }
    }
}
