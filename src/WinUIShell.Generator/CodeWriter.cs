
using System.Text;
using System.Text.RegularExpressions;

namespace WinUIShell.Generator;

internal class CodeWriter
{
    private readonly StringBuilder _builder = new();
    private int _indentLevel = 0;
    private string _indentString = "";
    private bool _isNewLineReserved;
    private const int _tabSpace = 4;

    public void AppendAndReserveNewLine(string str)
    {
        Append(str);
        _isNewLineReserved = true;
    }

    public void Append(string str)
    {
        if (string.IsNullOrEmpty(str))
            return;

        if (_isNewLineReserved)
        {
            _ = _builder.Append("\r\n");
            _isNewLineReserved = false;
        }

        var lines = Regex.Split(str, "\r\n|\n");
        foreach (var line in lines)
        {
            _ = _builder.Append(_indentString);
            _ = _builder.Append(line);
            _ = _builder.Append("\r\n");
        }
    }

    public void IncrementIndent()
    {
        ++_indentLevel;
        _indentString = new string(' ', _indentLevel * _tabSpace);
        _isNewLineReserved = false;
    }

    public void DecrementIndent()
    {
        --_indentLevel;
        _indentString = new string(' ', _indentLevel * _tabSpace);
        _isNewLineReserved = false;
    }

    public override string ToString()
    {
        return _builder.ToString();
    }
}
