namespace Common;
public class CircularCharArray(params char[] content)
{
    public static CircularCharArray ProgressSpinner => new('|', '/', '-', '\\');

    private int _pos = 0;

    public char Next()
    {
        if (++_pos >= content.Length)
        {
            _pos = 0;
        }

        return content[_pos];
    }
}
