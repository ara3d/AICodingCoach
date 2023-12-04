using AICodingCoach.Services;

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
    }
}