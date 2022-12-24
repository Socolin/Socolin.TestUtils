namespace Socolin.TestUtils.JsonComparer.Examples;

public class MainClass
{
    public static void Main()
    {
        new SimpleExample().Test1();
        new MatchExample().Test1();
        new MatchExample().Test2();
        new MatchExample().Test3();
        new MatchExample().Test4();
        new MatchExample().Test5();
        new MatchExample().Test6();
        new MatchExample().Test7();
        new MatchExample().Test8();
        new CaptureExample().Test1();
        new CaptureExample().Test2();
        new CaptureExample().Test3();
        new CaptureExample().Test4();
        new CaptureExample().Test5();
        new PartialExample().Test1();
        new PartialExample().Test2();
        new PartialArrayExample().Test1();
        new PartialArrayExample().Test2();
        new PartialArrayExample().Test3();
        new IgnoreExample().Test1();
        new IgnoreExample().Test2();
        new InvalidJsonExample().Test1();
    }
}