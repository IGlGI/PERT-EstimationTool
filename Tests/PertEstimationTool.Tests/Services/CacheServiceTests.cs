using AutoFixture;
using FluentAssertions;
using PertEstimationTool.Services.Interfaces;
using PertEstimationTool.Tests.Helpers;
using System;
using System.Runtime.Caching;
using Unity;
using Xunit;

namespace PertEstimationTool.Tests.Services
{
    public class CacheServiceTests
    {
        private IUnityContainer _container;

        private Fixture _fixture;

        private ICacheService _cacheService;

        private MemoryCache _memoryCache;

        private CacheItemPolicy _cacheItemPolicy;

        public CacheServiceTests()
        {
            _container = new ContainerHelper().GetContainer();
            _fixture = _container.Resolve<Fixture>();
            _cacheService = _container.Resolve<ICacheService>();
            _memoryCache = _container.Resolve<MemoryCache>();
            _cacheItemPolicy = _container.Resolve<CacheItemPolicy>();
        }

        [Fact]
        public async void CacheSecviceAddShouldToAddObjectIntoMemoryCache()
        {
            //Arrange
            var testObjectKey = _fixture.Create<string>();
            var testObject = _fixture.Create<Guid>();

            //Act
            await _cacheService.Add<Guid>(testObjectKey, testObject);
            var result = (Guid)_memoryCache.Get(testObjectKey);

            //Assert
            result.Should().Be(testObject);
        }

        [Fact]
        public async void CacheSecviceGetShouldToGetObjectFromMemoryCache()
        {
            //Arrange
            var testObjectKey = _fixture.Create<string>();
            var testObject = _fixture.Create<Guid>();

            //Act
            _memoryCache.Add(testObjectKey, testObject, _cacheItemPolicy);
            var result = (Guid)await _cacheService.Get(testObjectKey);

            //Assert
            result.Should().Be(testObject);
        }

        [Fact]
        public async void CacheSecviceRemoveShouldRemoveObjectFromMemoryCache()
        {
            //Arrange
            var testObjectKey = _fixture.Create<string>();
            var testObject = _fixture.Create<Guid>();

            //Act
            await _cacheService.Add<Guid>(testObjectKey, testObject);
            await _cacheService.Remove(testObjectKey);
            var result = (Guid?)_memoryCache.Get(testObjectKey);

            //Assert
            result.Should().Be(null);
        }

        [Fact]
        public async void CacheSecviceUpadateShouldUpadateObjectInMemoryCache()
        {
            //Arrange
            var testObjectKey = _fixture.Create<string>();
            var testObject = _fixture.Create<Guid>();

            //Act
            _memoryCache.Add(testObjectKey, testObject, _cacheItemPolicy);
            testObject = _fixture.Create<Guid>();
            await _cacheService.Upadate<Guid>(testObjectKey, testObject);
            var result = (Guid)_memoryCache.Get(testObjectKey);

            //Assert
            result.Should().Be(testObject);
        }

        [Fact]
        public async void CacheSecviceClearShouldClearMemoryCache()
        {
            //Arrange
            var testObjectKey = _fixture.Create<string>();
            var testObject = _fixture.Create<Guid>();

            //Act
            _memoryCache.Add(testObjectKey, testObject, _cacheItemPolicy);
            await _cacheService.Clear();

            //Assert
            _memoryCache.GetCount().Should().Be(0);
        }
    }
}
