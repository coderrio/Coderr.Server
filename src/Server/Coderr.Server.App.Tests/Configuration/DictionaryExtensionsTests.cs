using System;
using System.Collections.Generic;
using codeRR.Server.Infrastructure.Configuration;
using FluentAssertions;
using Xunit;

namespace codeRR.Server.App.Tests.Configuration
{
    public class DictionaryExtensionsTests
    {
        #region strings

        [Fact]
        public void Should_return_value_if_given_key_exists()
        {
            var dict = new Dictionary<string, string> {{"Name", "Vlue"}};

            var actual = dict.GetString("Name");

            actual.Should().Be("Vlue");
        }

        [Fact]
        public void Should_return_default_Value_if_given_key_do_not_exist()
        {
            var dict = new Dictionary<string, string> {{"Name", "Vlue"}};

            var actual = dict.GetString("Supplier", "Santa");

            actual.Should().Be("Santa");
        }

        #endregion

        #region boolean

        [Fact]
        public void Should_include_key_name_if_item_do_not_exist()
        {
            var dict = new Dictionary<string, string> {{"Usable", "Vlue"}};

            Action actual = () => dict.GetBoolean("hey!", null);

            actual.Should().Throw<ArgumentException>().Which.Message.Contains("hey!");
        }

        [Fact]
        public void Should_convert_to_bolean_if_given_item_is_found()
        {
            var dict = new Dictionary<string, string> {{"Usable", "True"}};

            var actual = dict.GetBoolean("Usable");

            actual.Should().BeTrue();
        }


        [Fact]
        public void Should_include_value_if_item_is_not_convertable_to_boolean()
        {
            var dict = new Dictionary<string, string> {{"Usable", "Vlue"}};

            Action actual = () => dict.GetBoolean("Usable");

            actual.Should().Throw<FormatException>().Which.Message.Contains("Vlue");
        }

        #endregion

        #region integer

        [Fact]
        public void Should_include_key_name_if_int_item_do_not_exist()
        {
            var dict = new Dictionary<string, string> {{"Length", "Vlue"}};

            Action actual = () => dict.GetInteger("hey!", null);

            actual.Should().Throw<ArgumentException>().Which.Message.Contains("hey!");
        }

        [Fact]
        public void Should_convert_to_integer_if_given_item_is_found()
        {
            var dict = new Dictionary<string, string> {{"Length", "1100"}};

            var actual = dict.GetInteger("Length");

            actual.Should().Be(1100);
        }


        [Fact]
        public void Should_include_value_if_item_is_not_convertable_to_integer()
        {
            var dict = new Dictionary<string, string> {{"Usable", "Vlue"}};

            Action actual = () => dict.GetInteger("Usable");

            actual.Should().Throw<FormatException>().Which.Message.Contains("Vlue");
        }

        #endregion
    }
}