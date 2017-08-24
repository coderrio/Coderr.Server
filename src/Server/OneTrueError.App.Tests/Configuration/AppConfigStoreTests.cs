using System;
using System.Configuration;
using FluentAssertions;
using OneTrueError.App.Tests.Configuration.TestEntitites;
using OneTrueError.Infrastructure.Configuration.ConfigFile;
using Xunit;

namespace OneTrueError.App.Tests.Configuration
{
    public class AppConfigStoreTests
    {
        [Fact]
        public void should_be_able_to_read_an_existing_category()
        {
            var sut = new ConfigFileStore();
            var actual = sut.Load<MySection>();

            actual.Name.Should().Be("Yo!");
        }

        [Fact]
        public void should_return_null_if_a_category_is_missing()
        {
            var sut = new ConfigFileStore();
            var actual = sut.Load<NonExistantSection>();

            actual.Should().BeNull();
        }

        [Fact]
        public void should_store_settings_with_dot_notation()
        {
            var expected = "World" + Guid.NewGuid().ToString("N");
            var category = new WriteTestSection();
            category.Properties["Hello"] = expected;

            var sut = new ConfigFileStore();
            sut.Store(category);
            var actual = ConfigurationManager.AppSettings[category.SectionName + ".Hello"];

            actual.Should().Be(expected);
        }
    }
}