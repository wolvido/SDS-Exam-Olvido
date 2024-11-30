using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using AutoFixture;
using EntityFrameworkMock;
using Moq;
using SdsExamOlvido.DbContexts;
using SdsExamOlvido.Models;
using SdsExamOlvido.ServiceContracts;
using SdsExamOlvido.Services;
using Xunit;

namespace SdsExamOlvido.Tests
{
    public class RecyclableTypeServiceTests
    {
        private readonly IRecyclableTypeService _recyclableTypeService;

        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<DbSet<RecyclableType>> _mockDbSet;

        private readonly IFixture _fixture;

        public RecyclableTypeServiceTests()
        {
            _fixture = new Fixture();

            _fixture.Customize<RecyclableType>(c => c
                .With(rt => rt.MaxKg, 50.5m)
                .With(rt => rt.MinKg, 10.43M)
                .With(rt => rt.Rate, 1.23M)
                .With(rt => rt.Type, "Type1")
                );

            // Set up the mock DbSet with data
            var data = new List<RecyclableType>
            {
                new RecyclableType {Id = 10, Type = "Type1", Rate = 1.23M, MinKg = 1.23M, MaxKg = 1.23M},
                new RecyclableType {Id = 11, Type = "Type2", Rate = 1.34M, MinKg = 1.73M, MaxKg = 1.79M},
                new RecyclableType {Id = 12, Type = "Type3", Rate = 1.76M, MinKg = 1.83M, MaxKg = 1.26M}
            };

            var dataList = data.ToList();

            _mockDbSet = new Mock<DbSet<RecyclableType>>();
            //mock dbset commands
            _mockDbSet.As<IDbAsyncEnumerable<RecyclableType>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new DbAsyncEnumerator<RecyclableType>(dataList.AsQueryable().GetEnumerator()));
            _mockDbSet.Setup(m => m.Add(It.IsAny<RecyclableType>()))
                .Callback<RecyclableType>(dataList.Add);
            _mockDbSet.Setup(m => m.Remove(It.IsAny<RecyclableType>()))
                .Callback<RecyclableType>(entity => dataList.Remove(entity));
            _mockDbSet.Setup(m => m.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((object[] ids) => dataList.FirstOrDefault(d => d.Id == (int)ids[0]));

            _mockContext = new Mock<ApplicationDbContext>();
            //mock dbcontext commands
            _mockContext.Setup(c => c.Set<RecyclableType>()).Returns(_mockDbSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync()).Returns(() => Task.Run(() => { return 1; })).Verifiable();

            // Set up the service to use the mock DbContext
            _recyclableTypeService = new RecyclableTypeService(_mockContext.Object);

        }
        [Fact]
        public async Task CreateRecyclableType_WithValidRecyclableType_ReturnsTrue()
        {
            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(123);

            // Arrange
            var recyclableType = new RecyclableType {Id = 34, Type = "wwew", Rate = 1.4M, MinKg = 3M, MaxKg = 6M};
            // Act
            var result = await _recyclableTypeService.CreateRecyclableType(recyclableType);
            // Assert
            Assert.True(result);

            //verify that SaveChangesAsync was called
            _mockContext.Verify(x=>x.SaveChangesAsync());
        }

        [Fact]
        public async Task CreateRecyclableType_WithInvalidRecyclableType_ReturnsFalse()
        {
            // Arrange
            var invalidRecycleType = _fixture.Build<RecyclableType>()
                .With(x => x.Type, "exceedsTenCharacters")
                .With(x => x.Rate, 1.23244M)
                .With(x => x.MinKg, 1.23524M)
                .With(x => x.MaxKg, 1.23324M)
                .Create();
            // Act
            var result = await _recyclableTypeService.CreateRecyclableType(invalidRecycleType);
            // Assert
            Assert.False(result);
        }
        [Fact]
        public async Task CreateRecyclableType_WithNullRecyclableType_ReturnsFalse()
        {
            // Arrange
            // Act
            var result = await _recyclableTypeService.CreateRecyclableType(null);
            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteRecyclableType_TypeExists_ReturnsTrue()
        {
            // Arrange
            var recycleType = _fixture.Create<RecyclableType>();
            _mockDbSet.Object.Add(recycleType);
            // Act
            var result = await _recyclableTypeService.DeleteRecyclableType(recycleType.Id);
            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteRecyclableType_TypeNonExistent_ReturnsFalse()
        {
            // Arrange
            // Act
            var result = await _recyclableTypeService.DeleteRecyclableType(-1);
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public async Task UpdateRecyclableType_TypeExistsAndUpdated_ReturnsTrue()
        {
            // Arrange
            var recycleType = _fixture.Build<RecyclableType>()
                .With(x => x.Id, 1)
                .With(x => x.Type, "old")
                .With(x => x.Rate, 1)
                .With(x => x.MinKg, 1)
                .With(x => x.MaxKg, 1)
                .Create();

            _mockDbSet.Object.Add(recycleType);

            var updatedRecycleType = _fixture.Build<RecyclableType>()
                .With(x => x.Id, 1)
                .With(x => x.Type, "Updated")
                .With(x => x.Rate, 2)
                .With(x => x.MinKg, 2)
                .With(x => x.MaxKg, 2)
                .Create();
            // Act
            var result = await _recyclableTypeService.UpdateRecyclableType(updatedRecycleType);

            // Assert
            //grab the updated recycleType
            var grabUpdated = await _mockDbSet.Object.FindAsync(updatedRecycleType.Id);


            Assert.True(updatedRecycleType.MaxKg == grabUpdated.MaxKg);
        }
        
        [Fact]
        public async Task UpdateRecyclableType_TypeNonExistent_ReturnsFalse()
        {
            // Arrange
            var recycleType = _fixture.Create<RecyclableType>();
            // Act
            var result = await _recyclableTypeService.UpdateRecyclableType(recycleType);
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public async Task GetAllRecyclableTypes_ReturnsRecyclableTypes()
        {
            // Arrange
            // Act
            var result = await _recyclableTypeService.GetAllRecyclableTypes();
            // Assert
            Assert.NotNull(result);
        }
    }
}
