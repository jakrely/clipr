﻿using clipr.Core;
using clipr.Usage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace clipr.UnitTests
{
    class NoDescription
    {
        [NamedArgument("name")]
        public string Name { get; set; }
    }

    class Help: AutomaticHelpGenerator<NoDescription>
    {
        public Help()
        {
            ShortName = null;
            LongName = "help";
        }
    }

    [TestClass]
    public class HelpGeneratorUnitTest
    {
        [TestMethod]
        public void Help_WithNoDescription_NoNullPointer()
        {

            var args = "--help".Split();
            var opt = new NoDescription();
            var parser = new CliParser<NoDescription>(
                opt,
                ParserOptions.None,
                new Help());

            AssertEx.Throws<ParserExit>(() =>
            parser.Parse(args));
        }
        
        [StaticEnumeration]
        internal class MyEnum
        {
            [EnumerationDescription("Some enum one")]
            public static readonly MyEnum First = new MyEnum();

            [EnumerationDescription("Some enum two")]
            public static readonly MyEnum Second = new MyEnum();
        }

        internal class StaticEnumOptions
        {
            public MyEnum Value { get; set; }
        }

        public class OptionsWithLongDescription
        {
            [NamedArgument('n', "name", Description = @"Lorem ipsum dolor sit amet,
consectetur adipiscing elit. Donec eget nunc semper, cursus purus et, placerat
magna. Mauris porttitor ante sit amet erat consequat, in euismod velit
imperdiet. Cras placerat tempus nisl id lacinia. Nulla facilisi. Pellentesque
dignissim, eros pellentesque facilisis porta, ligula magna venenatis sem, sed
dignissim risus turpis sit amet augue. Ut tincidunt mi faucibus dictum posuere.
Nullam condimentum consectetur interdum.")]
            public string Name { get; set; }
        }

        [TestMethod]
        public void Help_With80CharacterDisplayWidth_PrintsUpTo80Characters()
        {
            const string expected = " -n, --name  Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec eget\r";

            var opt = new OptionsWithLongDescription();
            var parser = new CliParser<OptionsWithLongDescription>(opt);
            var gen = new AutomaticHelpGenerator<OptionsWithLongDescription>
            {
                DisplayWidth = 80
            };


            var help = gen.GetHelp(parser.Config).Split('\n');

            Assert.AreEqual(expected, help[3]);
        }

        [TestMethod]
        public void Help_With40CharacterDisplayWidth_PrintsUpTo40Characters()
        {
            const string expected = " -n, --name  Lorem ipsum dolor sit amet,\r";

            var opt = new OptionsWithLongDescription();
            var parser = new CliParser<OptionsWithLongDescription>(opt);
            var gen = new AutomaticHelpGenerator<OptionsWithLongDescription>
            {
                DisplayWidth = 40
            };


            var help = gen.GetHelp(parser.Config).Split('\n');

            Assert.AreEqual(expected, help[3]);
        }

        [ApplicationInfo(Description = "This is a set of options.")]
        public class RequiredNamedOptions
        {
            [NamedArgument('c', "confirm", Action = ParseAction.StoreTrue, Required = true,
                 Description = "Confirms that the action is intended.")]
            public bool Confirmed { get; set; }
        }

        [TestMethod]
        public void Help_WithRequiredNamedArgument_ShowsArgumentInRequiredSection()
        {
            const string expected = @"Usage: clipr [ -h|--help ] [ --version ] -c|--confirm

 This is a set of options.

Required Arguments:
 -c, --confirm  Confirms that the action is intended.

Optional Arguments:
 -h, --help     Display this help document.
 --version      Displays the version of the current executable.";

            var opt = new RequiredNamedOptions();
            var parser = new CliParser<RequiredNamedOptions>(opt);
            var gen = new AutomaticHelpGenerator<RequiredNamedOptions>();

            var help = gen.GetHelp(parser.Config);

            Assert.AreEqual(expected, help);
        }

        // TODO GenerateUsage_WithStaticEnum_ListsEnumValues()
    }
}
