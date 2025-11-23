
using System.Text;
using System.Text.RegularExpressions;

namespace WinUIShell.Generator;

internal class CodeWriter
{
    private readonly StringBuilder _builder = new();
    private int _indentLevel = 0;
    private string _indentString = "";
    private bool _isFirstLineAfterIndent;
    private const int _tabSpace = 4;

    public void Append(string str)
    {
        var lines = Regex.Split(str, "\r\n|\n");

        for (int i = 0; i < lines.Length; ++i)
        {
            if (i == 0 && !_isFirstLineAfterIndent)
            {
                _ = _builder.Append("\r\n");
            }
            var line = lines[i];
            _ = _builder.Append(_indentString);
            _ = _builder.Append(line);
            _ = _builder.Append("\r\n");
        }

        _isFirstLineAfterIndent = false;
    }

    public void IncrementIndent()
    {
        ++_indentLevel;
        _indentString = new string(' ', _indentLevel * _tabSpace);
        _isFirstLineAfterIndent = true;
    }

    public void DecrementIndent()
    {
        --_indentLevel;
        _indentString = new string(' ', _indentLevel * _tabSpace);
        _isFirstLineAfterIndent = true;
    }

    public override string ToString()
    {
        return _builder.ToString();
    }
}
