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

            //valid fixture for RecyclableType
            _fixture.Customize<RecyclableType>(c => c
                .With(rt => rt.MaxKg, () => Math.Round((decimal)_fixture.Create<double>(), 2))
                .With(rt => rt.MinKg, () => Math.Round((decimal)_fixture.Create<double>(), 2))
                .With(rt => rt.Rate, () => Math.Round((decimal)_fixture.Create<double>(), 2))
                .With(rt => rt.Type, "Type1")
                );

            //mock DbSet default data
            var data = new List<RecyclableType>
            {
                new RecyclableType {Id = 10, Type = "Type1", Rate = 1.23M, MinKg = 1.23M, MaxKg = 1.23M},
                new RecyclableType {Id = 11, Type = "Type2", Rate = 1.34M, MinKg = 1.73M, MaxKg = 1.79M},
                new RecyclableType {Id = 12, Type = "Type3", Rate = 1.76M, MinKg = 1.83M, MaxKg = 1.26M}
            };
            var dataList = data;

            _mockDbSet = new Mock<DbSet<RecyclableType>>();

            //linq boilerplate
            _mockDbSet.As<IQueryable<RecyclableType>>()
                .Setup(m => m.Provider).Returns(dataList.AsQueryable().Provider);
            _mockDbSet.As<IQueryable<RecyclableType>>()
                .Setup(m => m.Expression).Returns(dataList.AsQueryable().Expression);
            _mockDbSet.As<IQueryable<RecyclableType>>()
                .Setup(m => m.ElementType).Returns(dataList.AsQueryable().ElementType);
            _mockDbSet.As<IQueryable<RecyclableType>>()
                .Setup(m => m.GetEnumerator()).Returns(dataList.AsQueryable().GetEnumerator());

            bool hasChanges = false;
            _mockDbSet.As<IDbAsyncEnumerable<RecyclableType>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new DbAsyncEnumerator<RecyclableType>(dataList.AsQueryable().GetEnumerator()));
            _mockDbSet.Setup(m => m.Add(It.IsAny<RecyclableType>()))
                //validation
                .Callback<RecyclableType>(entity =>
                {
                    if (entity == null || 
                    entity.Type.Length > 10 || 
                    entity.Rate % 1 != Math.Round(entity.Rate, 2) % 1 || 
                    entity.MinKg % 1 != Math.Round(entity.MinKg, 2) % 1 || 
                    entity.MaxKg % 1 != Math.Round(entity.MaxKg, 2) % 1) 
                    {
                        return;
                    }
                    dataList.Add(entity);
                    hasChanges = true;
                });
            _mockDbSet.Setup(m => m.Remove(It.IsAny<RecyclableType>()))
                .Callback<RecyclableType>(entity =>
                {
                    dataList.Remove(entity);
                    hasChanges = true;
                });

            _mockDbSet.Setup(m => m.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((object[] ids) => dataList.FirstOrDefault(d => d.Id == (int)ids[0]));

            _mockContext = new Mock<ApplicationDbContext>();
            _mockContext.Setup(c => c.Set<RecyclableType>()).Returns(_mockDbSet.Object);
            _mockContext.Setup(x => x.SaveChangesAsync()).ReturnsAsync(() =>
            {
                if (hasChanges)
                {
                    hasChanges = false;
                    return 1;
                }
                return 0;
            });

            _recyclableTypeService = new RecyclableTypeService(_mockContext.Object);

        }
        [Fact]
        public async Task CreateRecyclableType_WithValidRecyclableType_ReturnsTrue()
        {
            // Arrange
            var recyclableType = _fixture.Create<RecyclableType>();
            // Act
            var result = await _recyclableTypeService.CreateRecyclableType(recyclableType);
            // Assert
            var resultRecyclableType = await _mockDbSet.Object.FindAsync(recyclableType.Id);

            Assert.True(result);
            Assert.True(_mockDbSet.Object.Count() > 3);
            Assert.NotNull(resultRecyclableType);

            //verify that SaveChangesAsync was called
            _mockContext.Verify(x=>x.SaveChangesAsync());
        }

        [Fact]
        public async Task CreateRecyclableType_WithInvalidRecyclableType_ReturnsFalse()
        {
            // Arrange
            var exceedTenCharacters = _fixture.Build<RecyclableType>()
                .With(x => x.Type, "exceedsTenCharewewewacters")
                .With(x => x.Rate, 1.23M)
                .With(x => x.MinKg, 1.23M)
                .With(x => x.MaxKg, 1.23M)
                .Create();

            var exceedRateDecimal = _fixture.Build<RecyclableType>()
                .With(x => x.Rate, 1.23244M)
                .With(x => x.MinKg, 1.12M)
                .With(x => x.MaxKg, 1.23M)
                .With(x => x.Type, "Type1")
                .Create();

            var exceedMinKgDecimal = _fixture.Build<RecyclableType>()
                .With(x => x.MinKg, 1.23244M)
                .With(x => x.MaxKg, 1.12M)
                .With(x => x.Rate, 1.23M)
                .With(x => x.Type, "Type2")
                .Create();

            var exceedMaxKgDecimal = _fixture.Build<RecyclableType>()
                .With(x => x.MaxKg, 1.23244M)
                .With(x => x.MinKg, 1.12M)
                .With(x => x.Rate, 1.23M)
                .With(x => x.Type, "Type3")
                .Create();

            // Act
            var exceedTenCharactersResult = await _recyclableTypeService.CreateRecyclableType(exceedTenCharacters);
            var exceedRateDecimalResult = await _recyclableTypeService.CreateRecyclableType(exceedRateDecimal);
            var exceedMinKgDecimalResult = await _recyclableTypeService.CreateRecyclableType(exceedMinKgDecimal);
            var exceedMaxKgDecimalResult = await _recyclableTypeService.CreateRecyclableType(exceedMaxKgDecimal);
            // Assert
            Assert.False(exceedTenCharactersResult);
            Assert.False(exceedRateDecimalResult);
            Assert.False(exceedMinKgDecimalResult);
            Assert.False(exceedMaxKgDecimalResult);
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
            Assert.Null(await _mockDbSet.Object.FindAsync(recycleType.Id));
            Assert.True(_mockDbSet.Object.Count() <= 3);
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
            var grabUpdated = await _mockDbSet.Object.FindAsync(recycleType.Id);
            Assert.Equal(updatedRecycleType.MinKg, grabUpdated.MinKg);
            Assert.Equal(updatedRecycleType.MaxKg, grabUpdated.MaxKg);
            Assert.Equal(updatedRecycleType.Rate, grabUpdated.Rate);
            Assert.Equal(updatedRecycleType.Type, grabUpdated.Type);

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
            Assert.Null(await _mockDbSet.Object.FindAsync(recycleType.Id));
        }
        
        [Fact]
        public async Task GetAllRecyclableTypes_ReturnsRecyclableTypes()
        {
            // Arrange
            // Act
            var result = await _recyclableTypeService.GetAllRecyclableTypes();
            // Assert
            Assert.All(result, rt => Assert.IsType<RecyclableType>(rt));
        }

        [Fact]
        public async Task GetRecyclableTypeById_TypeExists_ReturnsRecyclableType()
        {
            // Arrange
            var recycleType = _fixture.Create<RecyclableType>();
            _mockDbSet.Object.Add(recycleType);
            // Act
            var result = await _recyclableTypeService.GetRecyclableTypeById(recycleType.Id);
            // Assert
            Assert.NotNull(result);
            Assert.IsType<RecyclableType>(result);
        }

        [Fact]
        public async Task GetRecyclableTypeById_TypeNonExistent_ReturnsNull()
        {
            // Arrange
            // Act
            var result = await _recyclableTypeService.GetRecyclableTypeById(-1);
            // Assert
            Assert.Null(result);
        }
    }
}
