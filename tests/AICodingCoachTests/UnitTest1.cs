using AICodingCoach.Services;
using Ara3D.Utils;

namespace AICodingCoachTests
{
    public class Tests
    {

        [Test]
        [TestCase("", 0)]
        [TestCase(" ", 1)]
        [TestCase("a", 1)]
        [TestCase("ab", 1)]
        [TestCase("a b", 1)]
        [TestCase("ab\n", 2)]
        [TestCase("\n", 1)]
        [TestCase("ab\ncd", 2)]
        [TestCase("ab\ncd\n", 3)]
        [TestCase("```", 1)]
        [TestCase("abc```", 2)]
        [TestCase("abc```def", 2)]
        [TestCase("abc```def```", 3)]
        [TestCase("abc```def```ghi", 3)]
        [TestCase("a\n```def", 3)]
        [TestCase("a\n```def```ghi", 4)]
        public void OutputSplitResult(string input, int count)
        {
            var output = ProjectService.SplitMessageText(input);
            Console.WriteLine("Input:");
            Console.WriteLine(input);
            Console.WriteLine("Output:");
            for (var i=0; i < output.Count; ++i)
            {
                Console.WriteLine($" {i} {output[i]}");
            }
            Assert.AreEqual(count, output.Count);
        }

        [Test, Explicit]
        public static void GenerateExamples()
        {
            var topics = new[]
            {
                /*
                "arrays",
                "two dimensional array",
                "classes",
                "lists",
                "collections",
                "iterators",
                "for loops",
                "while loops",
                "if statements",
                "strings",
                "binary numbers",
                "hexadecimal numbers",
                "trigonometric functions",
                "boolean operators",
                "integers",
                "stacks",
                "ASCII characters", 
                "floating point values",
                "variables",
                "pixels",
                "functions",
                "string interpolation",
                "string concatenation",
                "inheritance",
                "virtual functions and polymorphism",
                "dates",
                "time",
                "reading and writing to a file",
                "sorting",
                "recursion",
                "lambdas",
                "generators",
                */

                "bitmap files",
                "colors",
                "gradients",
                "vectors",
                "parametric functions",
                "polar functions",
                "linear interpolation",
                "random numbers",
                "sets",
                "dictionary",
                "prefix and postfix increment and decrement",
                "noise",
                "Brushes",
                "Trees",
                "fractals",
                "binary search",
                "computing average",
                "bit shifting",
                "truth tables",
                "static functions",
                "properties",
                "Size type",
                "Point type",
                "byte", 
                "Format functions",
                "Parse functions",
                "types",
                "constants",
                "GetHashCode function",
                "ToString function",
                "objects and polymorphism",
                "converting integers to bits", 
                "multiplication and division",
                "casting",
                "coercion",
                "structs",
            };

            var folder = SpecialFolders.Temp.RelativeFolder("ai-examples");
            folder.Create();
            var systemPrompt = new FilePath(@"C:\Users\cdigg\git\AICodingCoach\src\AICodingCoach\SystemPrompt.txt")
                .ReadAllText();
            foreach (var topic in topics)
            {
                Console.WriteLine($"Processing topic: {topic}");
                var prompt = $"Provide a code sample that explains and demonstrates {topic} using a visual example.";
                var chatService = new ChatService(systemPrompt);
                var file = folder.RelativeFile(topic.ReplaceNonAlphaNumeric("_") + ".txt");
                chatService.Conversation.AppendUserInput(prompt);
                var task = chatService.Conversation.GetResponseFromChatbotAsync();
                var response = task.GetAwaiter().GetResult();
                Console.WriteLine(response);
                file.WriteAllText(response);
            }
        }
    }
}