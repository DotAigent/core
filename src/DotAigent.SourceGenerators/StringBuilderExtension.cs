using System.Text;

namespace DotAigent.SourceGenerators;

public static class StringBuilderExtension
{
    public static void AppendLineIndent(this StringBuilder sb, int indentLeve, string value)
    {
        for (int i = 0; i < indentLeve; i++)
        {
            sb.Append("    ");
        }
        sb.AppendLine(value);
    }
}
