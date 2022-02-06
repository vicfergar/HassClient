using HassClient.Models;
using NUnit.Framework;
using System;

namespace HassClient.Core.Tests
{
    [TestFixture(TestOf = typeof(CalVer))]
    public class CalVerTests
    {
        [Test]
        public void ParseWithNullThrows()
        {
            var version = new CalVer();
            Assert.Throws<ArgumentNullException>(() => CalVer.Parse(null));
        }

        [Test]
        public void ParseWithInvalidYearThrows()
        {
            var version = new CalVer();
            Assert.Throws<ArgumentException>(() => CalVer.Parse("invalid"));
        }

        [Test]
        public void ParseWithInvalidMonthThrows()
        {
            var version = new CalVer();
            Assert.Throws<ArgumentException>(() => CalVer.Parse("2022.invalid"));
        }

        [Test]
        public void ParseWithInvalidMicroAndModifierThrows()
        {
            var version = new CalVer();
            Assert.Throws<ArgumentException>(() => CalVer.Parse("2022.02.''"));
        }

        [Test]
        public void ParseWithYearAndMonth()
        {
            var version = CalVer.Parse("2022.02");

            Assert.AreEqual(2022, version.Year);
            Assert.AreEqual(2, version.Month);
            Assert.AreEqual(0, version.Micro);
            Assert.AreEqual(string.Empty, version.Modifier);
        }

        [Test]
        public void ParseWithYearAndMonthAndMicro()
        {
            var version = CalVer.Parse("2022.02.13");

            Assert.AreEqual(2022, version.Year);
            Assert.AreEqual(2, version.Month);
            Assert.AreEqual(13, version.Micro);
            Assert.AreEqual(string.Empty, version.Modifier);
        }

        [Test]
        public void ParseWithYearAndMonthAndModifier()
        {
            var version = CalVer.Parse("2022.02.b3");

            Assert.AreEqual(2022, version.Year);
            Assert.AreEqual(2, version.Month);
            Assert.AreEqual(0, version.Micro);
            Assert.AreEqual("b3", version.Modifier);
        }

        [Test]
        public void ParseWithYearAndMonthMicroAndModifier()
        {
            var version = CalVer.Parse("2022.02.4b3");

            Assert.AreEqual(2022, version.Year);
            Assert.AreEqual(2, version.Month);
            Assert.AreEqual(4, version.Micro);
            Assert.AreEqual("b3", version.Modifier);
        }

        [Test]
        public void ReleaseDateIsCorrect()
        {
            var version = CalVer.Parse("2022.02.4b3");

            Assert.AreEqual(2022, version.ReleaseDate.Year);
            Assert.AreEqual(2, version.ReleaseDate.Month);
        }

        [Test]
        public void ToStringIsCorrect()
        {
            var expectedVersionString = "2022.2.4b3";
            var version = CalVer.Parse(expectedVersionString);

            Assert.AreEqual(expectedVersionString, version.ToString());
        }
    }
}
