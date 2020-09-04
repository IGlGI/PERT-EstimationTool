using Unity;
using Xunit;
using System;
using AutoFixture;
using FluentAssertions;
using System.Collections.Generic;
using PertEstimationTool.Tests.Helpers;
using PertEstimationTool.Services.Interfaces;

namespace PertEstimationTool.Tests.Services
{
    public class ControlServiceTests
    {
        private IUnityContainer _container;

        private Fixture _fixture;

        private IControlService _controlService;

        private ICacheService _cacheService;

        public ControlServiceTests()
        {
            _container = new TestsHelper().GetContainer();
            _fixture = _container.Resolve<Fixture>();
            _controlService = _container.Resolve<IControlService>();
            _cacheService = _container.Resolve<ICacheService>();
        }

        [Fact]
        public async void ControlServiceGetShouldGetObjectFromContainerByKeyAndAddToMemoryCache()
        {
            //Arrange
            var testObjectKey = _fixture.Create<string>();
            var testObject = _fixture.Create<Guid>();

            //Act
            _container.RegisterInstance<Guid>(testObjectKey, testObject);
            var result = await _controlService.Get<Guid>(testObjectKey);
            var memoryResult = (Guid)await _cacheService.Get(testObjectKey);

            //Assert
            result.Should().Be(testObject);
            memoryResult.Should().Be(testObject);
        }

        [Fact]
        public async void ControlServiceGetShouldGetObjectFromMemoryCache()
        {
            //Arrange
            var testObjectKey = _fixture.Create<string>();
            var testObject = _fixture.Create<Guid>();

            //Act
            await _cacheService.Add<Guid>(testObjectKey, testObject);
            var result = await _controlService.Get<Guid>(testObjectKey);

            //Assert
            result.Should().Be(testObject);
        }

        [Fact]
        public async void ControlServiceGetShouldThrowExceptionIfKeyIsEmpty()
        {
            //Arrange
            var testObjectKey = string.Empty;

            //Act
            var result = await Assert.ThrowsAsync<Exception>(() => _controlService.Get<object>(testObjectKey));

            //Assert
            result.Message.Should().Be("The key cannot be null or empty");
        }

        [Fact]
        public async void ControlServiceGetShouldThrowExceptionIfObjectNotRegisteredAndAbsentInMemoryCache()
        {
            //Arrange
            var testObjectKey = _fixture.Create<string>();

            //Act
            var result = await Assert.ThrowsAsync<Exception>(() => _controlService.Get<Guid>(testObjectKey));

            //Assert
            result.Message.Should().Be("The requested data wasn't found.");
        }

        [Fact]
        public async void ControlServiceResetShouldRemoveObjectFromMemoryCacheAndRegisterToContainerByKey()
        {
            //Arrange
            var testObjectKey = _fixture.Create<string>();
            var testObject = _fixture.Create<List<string>>();
            testObject.Add(_fixture.Create<string>());

            //Act
            await _cacheService.Add<List<string>>(testObjectKey, testObject);
            await _controlService.Reset<List<string>>(testObjectKey);
            var result = _container.Resolve<List<string>>(testObjectKey);

            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(0);
        }

        [Fact]
        public async void ControlServiceResetShouldThrowExceptionIfKeyIsEmpty()
        {
            //Arrange
            var testObjectKey = string.Empty;

            //Act
            var result = await Assert.ThrowsAsync<Exception>(() => _controlService.Reset<object>(testObjectKey));

            //Assert
            result.Message.Should().Be("The key cannot be null or empty");
        }
    }
}