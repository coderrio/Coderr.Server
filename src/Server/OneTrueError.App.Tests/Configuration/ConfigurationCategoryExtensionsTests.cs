using System.Globalization;
using System.Threading;
using FluentAssertions;
using OneTrueError.App.Configuration;
using OneTrueError.App.Tests.Configuration.TestEntitites;
using OneTrueError.Infrastructure.Configuration;
using Xunit;

namespace OneTrueError.App.Tests.Configuration
{
    public class ConfigurationCategoryExtensionsTests
    {
        [Fact]
        public void should_ignore_categoryName_when_generating_The_dictionary()
        {
            var cat = new WriteTestSection();

            var dict = cat.ToConfigDictionary();

            dict.Keys.Should().NotContain(x => x == "SectionName");
        }

        [Fact]
        public void should_format_values_with_invariant_culture()
        {
            var cat = new SoCultural();
            cat.Number = 43.32f;

            Thread.CurrentThread.CurrentCulture = new CultureInfo("sv-se");
            var dict = cat.ToConfigDictionary();

            dict["Number"].Should().Be("43.32");
            43.32f.ToString().Should().Be("43,32", "because we need to validate that the config is correct.");
        }

        [Fact]
        public void should_be_able_to_work_despite_local_culture_when_persisting_configuration()
        {
            var cat = new SoCultural();
            cat.Number = 43.32f;

            Thread.CurrentThread.CurrentCulture = new CultureInfo("sv-se");
            var dict = cat.ToConfigDictionary();

            dict["Number"].Should().Be("43.32");
            43.32f.ToString()
                .Should()
                .Be("43,32", "because we need to validate changes when another culture is active.");
        }

        [Fact]
        public void do_not_persist_category_name_as_its_just_metadata_and_part_of_the_key()
        {
            var cat = new SoCultural();
            cat.Number = 43.32f;

            Thread.CurrentThread.CurrentCulture = new CultureInfo("sv-se");
            var dict = cat.ToConfigDictionary();

            dict.Keys.Should().NotContain("SectionName");
        }
    }
}