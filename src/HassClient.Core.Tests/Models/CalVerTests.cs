using HassClient.Models;
using NUnit.Framework;
using System;

namespace HassClient.Core.Tests
{
    [TestFixture(TestOf = typeof(CalVer))]
    public class CalVerTests
    {
        [Test]
        public void CreateWithNullThrows()
        {
            var version = new CalVer();
            Assert.Throws<ArgumentNullException>(() => CalVer.Create(null));
        }

        [Test]
        public void CreateWithInvalidYearThrows()
        {
            var version = new CalVer();
            Assert.Throws<ArgumentException>(() => CalVer.Create("invalid"));
        }

        [Test]
        public void CreateWithInvalidMonthThrows()
        {
            var version = new CalVer();
            Assert.Throws<ArgumentException>(() => CalVer.Create("2022.invalid"));
        }

        [Test]
        public void CreateWithInvalidMicroAndModifierThrows()
        {
            var version = new CalVer();
            Assert.Throws<ArgumentException>(() => CalVer.Create("2022.02.''"));
        }

        [Test]
        public void CreateWithYearAndMonth()
        {
            var version = CalVer.Create("2022.02");

            Assert.AreEqual(2022, version.Year);
            Assert.AreEqual(2, version.Month);
            Assert.AreEqual(0, version.Micro);
            Assert.AreEqual(string.Empty, version.Modifier);
        }

        [Test]
        public void CreateWithYearAndMonthAndMicro()
        {
            var version = CalVer.Create("2022.02.13");

            Assert.AreEqual(2022, version.Year);
            Assert.AreEqual(2, version.Month);
            Assert.AreEqual(13, version.Micro);
            Assert.AreEqual(string.Empty, version.Modifier);
        }

        [Test]
        public void CreateWithYearAndMonthAndModifier()
        {
            var version = CalVer.Create("2022.02.b3");

            Assert.AreEqual(2022, version.Year);
            Assert.AreEqual(2, version.Month);
            Assert.AreEqual(0, version.Micro);
            Assert.AreEqual("b3", version.Modifier);
        }

        [Test]
        public void CreateWithYearAndMonthMicroAndModifier()
        {
            var version = CalVer.Create("2022.02.4b3");

            Assert.AreEqual(2022, version.Year);
            Assert.AreEqual(2, version.Month);
            Assert.AreEqual(4, version.Micro);
            Assert.AreEqual("b3", version.Modifier);
        }

        [Test]
        public void CreateDateIsCorrect()
        {
            var version = CalVer.Create("2022.02.4b3");

            Assert.AreEqual(2022, version.ReleaseDate.Year);
            Assert.AreEqual(2, version.ReleaseDate.Month);
        }

        [Test]
        public void ToStringIsCorrect()
        {
            var expectedVersionString = "2022.2.4b3";
            var version = CalVer.Create(expectedVersionString);

            Assert.AreEqual(expectedVersionString, version.ToString());
        }
    }
}
