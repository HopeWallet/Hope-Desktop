public class Tester
{
    [ReflectionProtect]
    private void DoStuff()
    {
    }

    [ReflectionProtect(typeof(int))]
    private int DoMoreStuff()
    {
        return 0;
    }
}
