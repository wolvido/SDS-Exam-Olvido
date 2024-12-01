using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class RecyclableItemServiceTests
    {
        private readonly IRecyclableItemService _recyclableItemService;

        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<DbSet<RecyclableItem>> _mockDbSet;

        private readonly IFixture _fixture;

        public RecyclableItemServiceTests()
        {
            _fixture = new Fixture();
            //valid fixture for RecyclableItem
            _fixture.Customize<RecyclableItem>(c => c
                .With(recyclableItem => recyclableItem.ItemDescription, "Item4")
                .With(recyclableItem => recyclableItem.Weight, () => Math.Round((decimal)_fixture.Create<double>(), 2))
                .With(recyclableItem => recyclableItem.ComputedRate,() => Math.Round((decimal)_fixture.Create<double>(), 2))
                .With(recyclableItem => recyclableItem.RecyclableTypeId, _fixture.Create<int>())
                .With(RecyclableItem => RecyclableItem.Id, _fixture.Create<int>())
                .OmitAutoProperties()
                );

            //mock DbSet default data
            var data = new List<RecyclableItem>
            {
                new RecyclableItem { Id = 1, RecyclableTypeId = 1, Weight = 1.23M, ComputedRate = 1.23M, ItemDescription = "Item1" },
                new RecyclableItem { Id = 2, RecyclableTypeId = 2, Weight = 2.23M, ComputedRate = 2.23M, ItemDescription = "Item2" },
                new RecyclableItem { Id = 3, RecyclableTypeId = 3, Weight = 3.23M, ComputedRate = 3.23M, ItemDescription = "Item3" }
            };
            var dataList = data;

            _mockDbSet = new Mock<DbSet<RecyclableItem>>();

            //linq boilerplate
            _mockDbSet.As<IQueryable<RecyclableItem>>()
                .Setup(m => m.Provider).Returns(dataList.AsQueryable().Provider);
            _mockDbSet.As<IQueryable<RecyclableItem>>()
                .Setup(m => m.Expression).Returns(dataList.AsQueryable().Expression);
            _mockDbSet.As<IQueryable<RecyclableItem>>()
                .Setup(m => m.ElementType).Returns(dataList.AsQueryable().ElementType);
            _mockDbSet.As<IQueryable<RecyclableItem>>()
                .Setup(m => m.GetEnumerator()).Returns(dataList.AsQueryable().GetEnumerator());

            bool hasChanges = false;
            _mockDbSet.As<IDbAsyncEnumerable<RecyclableItem>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new DbAsyncEnumerator<RecyclableItem>(dataList.AsQueryable().GetEnumerator()));
            _mockDbSet.Setup(m => m.Add(It.IsAny<RecyclableItem>()))
                //validation
                .Callback<RecyclableItem>(entity =>
                {
                    if (entity == null ||
                    entity.ItemDescription.Length > 150 ||
                    entity.Weight % 1 != Math.Round(entity.Weight, 2) % 1 ||
                    entity.ComputedRate % 1 != Math.Round(entity.ComputedRate, 2) % 1)
                    {
                        return;
                    }
                    dataList.Add(entity);
                    hasChanges = true;
                });
            _mockDbSet.Setup(m => m.Remove(It.IsAny<RecyclableItem>()))
                .Callback<RecyclableItem>(entity =>
                {
                    dataList.Remove(entity);
                    hasChanges = true;
                });

            _mockDbSet.Setup(m => m.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((object[] ids) => dataList.FirstOrDefault(d => d.Id == (int)ids[0]));

            _mockContext = new Mock<ApplicationDbContext>();
            _mockContext.Setup(c => c.Set<RecyclableItem>()).Returns(_mockDbSet.Object);
            _mockContext.Setup(x => x.SaveChangesAsync()).ReturnsAsync(() =>
            {
                if (hasChanges)
                {
                    hasChanges = false;
                    return 1;
                }
                return 0;
            });

            _recyclableItemService = new RecyclableItemService(_mockContext.Object);
        }

        [Fact]
        public async Task CreateRecyclableItem_WithValidData_ReturnsTrue()
        {
            //Arrange
            var recyclableItem = _fixture.Create<RecyclableItem>();
            //Act
            var result = await _recyclableItemService.CreateRecyclableItem(recyclableItem);
            //Assert
            var resultItem = _mockDbSet.Object.FirstOrDefault(x => x.Id == recyclableItem.Id);

            Assert.True(result);
            Assert.True(_mockDbSet.Object.Count() > 3);
            Assert.NotNull(resultItem);
        }

        [Fact]
        public async Task CreateRecyclableItem_WithInvalidData_ReturnsFalse()
        {
            //Arrange
            var exceeedCharacters = _fixture.Build<RecyclableItem>()
                .With(x => x.ItemDescription, new string('a', 151))
                .With(recyclableItem => recyclableItem.Weight, () => Math.Round((decimal)_fixture.Create<double>(), 2))
                .With(recyclableItem => recyclableItem.ComputedRate,() => Math.Round((decimal)_fixture.Create<double>(), 2))
                .With(recyclableItem => recyclableItem.RecyclableTypeId, _fixture.Create<int>())
                .With(RecyclableItem => RecyclableItem.Id, _fixture.Create<int>())
                .OmitAutoProperties()
                .Create();

            var weightExceedDecimal = _fixture.Build<RecyclableItem>()
                .With(x => x.Weight, 1.234M)
                .With(recyclableItem => recyclableItem.ComputedRate, () => Math.Round((decimal)_fixture.Create<double>(), 2))
                .With(recyclableItem => recyclableItem.RecyclableTypeId, _fixture.Create<int>())
                .With(RecyclableItem => RecyclableItem.Id, _fixture.Create<int>())
                .With(recyclableItem => recyclableItem.ItemDescription, "Item4")
                .OmitAutoProperties()
                .Create();

            var computedRateExceedDecimal = _fixture.Build<RecyclableItem>()
                .With(x => x.Weight, () => Math.Round((decimal)_fixture.Create<double>(), 2))
                .With(recyclableItem => recyclableItem.ComputedRate, 1.234M)
                .With(recyclableItem => recyclableItem.RecyclableTypeId, _fixture.Create<int>())
                .With(RecyclableItem => RecyclableItem.Id, _fixture.Create<int>())
                .With(recyclableItem => recyclableItem.ItemDescription, "Item4")
                .OmitAutoProperties()
                .Create();

            //Act

            bool exceedCharactersResult = await _recyclableItemService.CreateRecyclableItem(computedRateExceedDecimal);
            bool weightExceedDecimalResult = await _recyclableItemService.CreateRecyclableItem(weightExceedDecimal);
            bool computedRateExceedDecimalResult = await _recyclableItemService.CreateRecyclableItem(computedRateExceedDecimal);

            //Assert
            Assert.False(exceedCharactersResult);
            Assert.False(weightExceedDecimalResult);
            Assert.False(computedRateExceedDecimalResult);

        }

        [Fact]
        public async Task CreateRecyclableItem_WithNullData_ReturnsFalse()
        {
            //Arrange
            //Act
            var result = await _recyclableItemService.CreateRecyclableItem(null);
            //Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteRecyclableItem_WithValidId_ReturnsTrue()
        {
            //Arrange
            var recycleItem = _fixture.Create<RecyclableItem>();
            _mockDbSet.Object.Add(recycleItem);
            //Act
            var result = await _recyclableItemService.DeleteRecyclableItem(recycleItem.Id);

            //Assert
            Assert.True(result);
            Assert.Null(await _mockDbSet.Object.FindAsync(recycleItem.Id));
            Assert.True(_mockDbSet.Object.Count() <= 3);

        }

        [Fact]
        public async Task DeleteRecyclableItem_WithInvalidId_ReturnsFalse()
        {
            //Arrange

            //Act
            var result = await _recyclableItemService.DeleteRecyclableItem(-1);
            //Assert
            Assert.False(result);

        }

        [Fact]
        public async Task UpdateRecyclableItem_WithValidData_ReturnsTrue()
        {
            //Arrange
            var recycleItem = _fixture.Build<RecyclableItem>()
                .With(x => x.ItemDescription, "Original")
                .With(x => x.Weight, () => Math.Round((decimal)_fixture.Create<double>(), 2))
                .With(x => x.ComputedRate, () => Math.Round((decimal)_fixture.Create<double>(), 2))
                .With(x => x.RecyclableTypeId, _fixture.Create<int>())
                .With(x => x.Id, 101)
                .OmitAutoProperties()
                .Create();

            _mockDbSet.Object.Add(recycleItem);

            var updatedItem = _fixture.Build<RecyclableItem>()
                .With(x => x.ItemDescription, "Updated")
                .With(x => x.Weight, () => Math.Round((decimal)_fixture.Create<double>(), 2))
                .With(x => x.ComputedRate, () => Math.Round((decimal)_fixture.Create<double>(), 2))
                .With(x => x.RecyclableTypeId, _fixture.Create<int>())
                .With(x => x.Id, 101)
                .OmitAutoProperties()
                .Create();

            //Act
            var result = await _recyclableItemService.UpdateRecyclableItem(updatedItem);

            //Assert
            var grabUpdated = await _mockDbSet.Object.FindAsync(recycleItem.Id);
            Assert.Equal(updatedItem.Weight, grabUpdated.Weight);
            Assert.Equal(updatedItem.ComputedRate, grabUpdated.ComputedRate);
            Assert.Equal(updatedItem.ItemDescription, grabUpdated.ItemDescription);
            Assert.Equal(updatedItem.RecyclableTypeId, grabUpdated.RecyclableTypeId);


        }

        [Fact]
        public async Task UpdateRecyclableItem_WithInVvalidData_ReturnsTrue()
        {
            //Arrange
            var recycleItem = _fixture.Create<RecyclableItem>();

            //Act
            var result = await _recyclableItemService.UpdateRecyclableItem(recycleItem);

            //Assert
            Assert.False(result);
            Assert.Null(await _mockDbSet.Object.FindAsync(recycleItem.Id));

        }

        [Fact]
        public async Task GetAllRecyclableItems_ReturnsAllItems()
        {
            //Arrange
            //Act
            var result = await _recyclableItemService.GetAllRecyclableItems();
            //Assert
            Assert.All(result, x => Assert.IsType<RecyclableItem>(x));
        }

        [Fact]
        public async Task GetRecyclableItemById_WithValidId_ReturnsItem()
        {
            //Arrange
            var recyclableItem = _fixture.Create<RecyclableItem>();
            _mockDbSet.Object.Add(recyclableItem);

            //Act
            var result = await _recyclableItemService.GetRecyclableItemById(recyclableItem.Id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(recyclableItem.Id, result.Id);

        }

        [Fact]
        public async Task GetRecyclableItemById_WithInvalid()
        {
            //Arrange
            //Act
            var result = await _recyclableItemService.GetRecyclableItemById(-1);
            //Assert
            Assert.Null(result);
        }

    }
}
