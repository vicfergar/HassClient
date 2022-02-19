using Newtonsoft.Json;

namespace HassClient.Models
{
    /// <summary>
    /// Represents a color described by a known name.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "Color names are self-documented")]
    public class NameColor : Color
    {
        public static NameColor AliceBlue => new NameColor("aliceblue");

        public static NameColor AntiqueWhite => new NameColor("antiquewhite");

        public static NameColor Aqua => new NameColor("aqua");

        public static NameColor Aquamarine => new NameColor("aquamarine");

        public static NameColor Azure => new NameColor("azure");

        public static NameColor Beige => new NameColor("beige");

        public static NameColor Bisque => new NameColor("bisque");

        public static NameColor Black => new NameColor("black");

        public static NameColor BlanchedAlmond => new NameColor("blanchedalmond");

        public static NameColor Blue => new NameColor("blue");

        public static NameColor BlueViolet => new NameColor("blueviolet");

        public static NameColor Brown => new NameColor("brown");

        public static NameColor BurlyWood => new NameColor("burlywood");

        public static NameColor CadetBlue => new NameColor("cadetblue");

        public static NameColor Chartreuse => new NameColor("chartreuse");

        public static NameColor Chocolate => new NameColor("chocolate");

        public static NameColor Coral => new NameColor("coral");

        public static NameColor CornflowerBlue => new NameColor("cornflowerblue");

        public static NameColor Cornsilk => new NameColor("cornsilk");

        public static NameColor Crimson => new NameColor("crimson");

        public static NameColor Cyan => new NameColor("cyan");

        public static NameColor DarkBlue => new NameColor("darkblue");

        public static NameColor DarkCyan => new NameColor("darkcyan");

        public static NameColor DarkGoldenrod => new NameColor("darkgoldenrod");

        public static NameColor DarkGray => new NameColor("darkgray");

        public static NameColor DarkGreen => new NameColor("darkgreen");

        public static NameColor DarkGrey => new NameColor("darkgrey");

        public static NameColor DarkKhaki => new NameColor("darkkhaki");

        public static NameColor DarkmMagenta => new NameColor("darkmagenta");

        public static NameColor DarkOliveGreen => new NameColor("darkolivegreen");

        public static NameColor DarkOrange => new NameColor("darkorange");

        public static NameColor DarkOrchid => new NameColor("darkorchid");

        public static NameColor DarkRed => new NameColor("darkred");

        public static NameColor DarkSalmon => new NameColor("darksalmon");

        public static NameColor DarkSeaGreen => new NameColor("darkseagreen");

        public static NameColor DarkSlateBlue => new NameColor("darkslateblue");

        public static NameColor DarkSlateGray => new NameColor("darkslategray");

        public static NameColor DarkSlateGrey => new NameColor("darkslategrey");

        public static NameColor DarkTurquoise => new NameColor("darkturquoise");

        public static NameColor DarkViolet => new NameColor("darkviolet");

        public static NameColor DeepPink => new NameColor("deeppink");

        public static NameColor DeepSkyBlue => new NameColor("deepskyblue");

        public static NameColor DimGray => new NameColor("dimgray");

        public static NameColor DimGrey => new NameColor("dimgrey");

        public static NameColor DodgerBlue => new NameColor("dodgerblue");

        public static NameColor Firebrick => new NameColor("firebrick");

        public static NameColor FloralWhite => new NameColor("floralwhite");

        public static NameColor ForestGreen => new NameColor("forestgreen");

        public static NameColor Fuchsia => new NameColor("fuchsia");

        public static NameColor Gainsboro => new NameColor("gainsboro");

        public static NameColor GhostWhite => new NameColor("ghostwhite");

        public static NameColor Gold => new NameColor("gold");

        public static NameColor Goldenrod => new NameColor("goldenrod");

        public static NameColor Gray => new NameColor("gray");

        public static NameColor Green => new NameColor("green");

        public static NameColor GreenYellow => new NameColor("greenyellow");

        public static NameColor Grey => new NameColor("grey");

        public static NameColor Honeydew => new NameColor("honeydew");

        public static NameColor HotPink => new NameColor("hotpink");

        public static NameColor IndianRed => new NameColor("indianred");

        public static NameColor Indigo => new NameColor("indigo");

        public static NameColor Ivory => new NameColor("ivory");

        public static NameColor Khaki => new NameColor("khaki");

        public static NameColor Lavender => new NameColor("lavender");

        public static NameColor LavenderBlush => new NameColor("lavenderblush");

        public static NameColor LawnGreen => new NameColor("lawngreen");

        public static NameColor LemonChiffon => new NameColor("lemonchiffon");

        public static NameColor LightBlue => new NameColor("lightblue");

        public static NameColor LightCoral => new NameColor("lightcoral");

        public static NameColor LightCyan => new NameColor("lightcyan");

        public static NameColor LightGoldenrodYellow => new NameColor("lightgoldenrodyellow");

        public static NameColor LightGray => new NameColor("lightgray");

        public static NameColor LightGreen => new NameColor("lightgreen");

        public static NameColor LightGrey => new NameColor("lightgrey");

        public static NameColor LightPink => new NameColor("lightpink");

        public static NameColor LightSalmon => new NameColor("lightsalmon");

        public static NameColor LightSeaGreen => new NameColor("lightseagreen");

        public static NameColor LightSkyBlue => new NameColor("lightskyblue");

        public static NameColor LightSlateGray => new NameColor("lightslategray");

        public static NameColor LightSlateGrey => new NameColor("lightslategrey");

        public static NameColor LightSteelBlue => new NameColor("lightsteelblue");

        public static NameColor LightYellow => new NameColor("lightyellow");

        public static NameColor Lime => new NameColor("lime");

        public static NameColor LimeGreen => new NameColor("limegreen");

        public static NameColor Linen => new NameColor("linen");

        public static NameColor Magenta => new NameColor("magenta");

        public static NameColor Maroon => new NameColor("maroon");

        public static NameColor MediumAquamarine => new NameColor("mediumaquamarine");

        public static NameColor MediumBlue => new NameColor("mediumblue");

        public static NameColor MediumOrchid => new NameColor("mediumorchid");

        public static NameColor MediumPurple => new NameColor("mediumpurple");

        public static NameColor MediumSeaGreen => new NameColor("mediumseagreen");

        public static NameColor MediumSlateBlue => new NameColor("mediumslateblue");

        public static NameColor MediumSpringGreen => new NameColor("mediumspringgreen");

        public static NameColor MediumTurquoise => new NameColor("mediumturquoise");

        public static NameColor MediumVioletRed => new NameColor("mediumvioletred");

        public static NameColor MidnightBlue => new NameColor("midnightblue");

        public static NameColor MintCream => new NameColor("mintcream");

        public static NameColor MistyRose => new NameColor("mistyrose");

        public static NameColor Moccasin => new NameColor("moccasin");

        public static NameColor NavajoWhite => new NameColor("navajowhite");

        public static NameColor Navy => new NameColor("navy");

        public static NameColor OldLace => new NameColor("oldlace");

        public static NameColor Olive => new NameColor("olive");

        public static NameColor OliveDrab => new NameColor("olivedrab");

        public static NameColor Orange => new NameColor("orange");

        public static NameColor OrangeRed => new NameColor("orangered");

        public static NameColor Orchid => new NameColor("orchid");

        public static NameColor PaleGoldenrod => new NameColor("palegoldenrod");

        public static NameColor PaleGreen => new NameColor("palegreen");

        public static NameColor PaleTurquoise => new NameColor("paleturquoise");

        public static NameColor PaleVioletRed => new NameColor("palevioletred");

        public static NameColor PapayaWhip => new NameColor("papayawhip");

        public static NameColor PeachPuff => new NameColor("peachpuff");

        public static NameColor Peru => new NameColor("peru");

        public static NameColor Pink => new NameColor("pink");

        public static NameColor Plum => new NameColor("plum");

        public static NameColor PowderBlue => new NameColor("powderblue");

        public static NameColor Purple => new NameColor("purple");

        public static NameColor Red => new NameColor("red");

        public static NameColor RosyBrown => new NameColor("rosybrown");

        public static NameColor RoyalBlue => new NameColor("royalblue");

        public static NameColor SaddleBrown => new NameColor("saddlebrown");

        public static NameColor Salmon => new NameColor("salmon");

        public static NameColor SandyBrown => new NameColor("sandybrown");

        public static NameColor SeaGreen => new NameColor("seagreen");

        public static NameColor SeaShell => new NameColor("seashell");

        public static NameColor Sienna => new NameColor("sienna");

        public static NameColor Silver => new NameColor("silver");

        public static NameColor SkyBlue => new NameColor("skyblue");

        public static NameColor SlateBlue => new NameColor("slateblue");

        public static NameColor SlateGray => new NameColor("slategray");

        public static NameColor SlateGrey => new NameColor("slategrey");

        public static NameColor Snow => new NameColor("snow");

        public static NameColor SpringGreen => new NameColor("springgreen");

        public static NameColor SteelBlue => new NameColor("steelblue");

        public static NameColor Tan => new NameColor("tan");

        public static NameColor Teal => new NameColor("teal");

        public static NameColor Thistle => new NameColor("thistle");

        public static NameColor Tomato => new NameColor("tomato");

        public static NameColor Turquoise => new NameColor("turquoise");

        public static NameColor Violet => new NameColor("violet");

        public static NameColor Wheat => new NameColor("wheat");

        public static NameColor White => new NameColor("white");

        public static NameColor WhiteSmoke => new NameColor("whitesmoke");

        public static NameColor Yellow => new NameColor("yellow");

        public static NameColor YellowGreen => new NameColor("yellowgreen");

        public static NameColor HomeAssistant => new NameColor("homeassistant");

        /// <summary>
        /// Gets a the color name.
        /// </summary>
        [JsonProperty("color_name")]
        public string Name { get; internal set; }

        internal NameColor(string name)
            : base()
        {
            this.Name = name;
        }

        /// <inheritdoc />
        public override string ToString() => this.Name;
    }
}
