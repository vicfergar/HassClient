using System;
using System.Text.RegularExpressions;

namespace HassClient.Models
{
    /// <summary>
    /// Calendar versioning representation used by Home Assitant.
    /// </summary>
    public class CalVer
    {
        /// <summary>
        /// Gets or sets the year in which this version was released.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Gets or sets the month in which this version was released.
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Gets or sets the third and usually final number in the version. Sometimes referred to as the "patch" segment.
        /// </summary>
        public int Micro { get; set; }

        /// <summary>
        /// Gets or sets an optional text tag, such as "dev", "alpha", "beta", "rc1", and so on.
        /// </summary>
        public string Modifier { get; set; }

        /// <summary>
        /// Gets the release date extracted from <see cref="Year"/> and <see cref="Month"/>.
        /// </summary>
        public DateTime ReleaseDate => new DateTime(this.Year, this.Month, 1);

        /// <inheritdoc />
        public override string ToString() => $"{this.Year}.{this.Month}.{this.Micro}{this.Modifier}";

        /// <summary>
        /// Converts the string representation of a version number to an equivalent <see cref="CalVer"/> object.
        /// </summary>
        /// <param name="input">An string representing a calendar version (eg: 2021.12.0b3).</param>
        /// <exception cref="ArgumentNullException"><paramref name="input"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="input"/> has fewer than two or more than three version components.</exception>
        /// <returns>An object that is equivalent to the version specified in the <paramref name="input"/> parameter.</returns>
        public static CalVer Parse(string input)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var parts = input.Split('.');
            if (parts.Length < 2 || parts.Length > 3)
            {
                throw new ArgumentException("Unexpected version format", nameof(input));
            }

            var yearPart = parts[0];
            if (!int.TryParse(yearPart, out var year))
            {
                throw new ArgumentException($"Unexpected version format. {nameof(Year)} cannot be parsed from '{yearPart}'", nameof(input));
            }

            var monthPart = parts[1];
            if (!int.TryParse(monthPart, out var month))
            {
                throw new ArgumentException($"Unexpected version format. {nameof(Month)} cannot be parsed from '{monthPart}'", nameof(input));
            }

            var micro = 0;
            var modifier = string.Empty;
            if (parts.Length > 2)
            {
                var microModifierPart = parts[2];
                var match = Regex.Match(microModifierPart, @"(?<micro>^\d*)(?<modifier>[\w\d]*)");
                var microStr = match.Groups["micro"].Value;
                var modifierStr = match.Groups["modifier"].Value;

                if (string.IsNullOrEmpty(microStr) && string.IsNullOrEmpty(modifierStr))
                {
                    throw new ArgumentException($"Unexpected version format. {nameof(Micro)} and {nameof(Modifier)} cannot be parsed from '{microModifierPart}'", nameof(input));
                }

                micro = !string.IsNullOrEmpty(microStr) ? int.Parse(microStr) : 0;
                modifier = modifierStr;
            }

            return new CalVer
            {
                Year = year,
                Month = month,
                Micro = micro,
                Modifier = modifier,
            };
        }
    }
}
