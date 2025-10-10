using System;
using System.Text;

// 拆分接口（避免大而全的接口）
public interface IPrintable
{
    void Print(string content);
}

public interface IScannable
{
    string Scan();
}

// 仅打印的设备
public class BasicPrinter : IPrintable
{
    public void Print(string content)
    {
        Console.WriteLine($"打印内容：{content}");
    }
}

// 仅扫描的设备
public class BasicScanner : IScannable
{
    public string Scan()
    {
        return "扫描的文档内容";
    }
}

// 多功能设备
public class AllInOneDevice : IPrintable, IScannable
{
    public void Print(string content)
    {
        Console.WriteLine($"多功能设备打印：{content}");
    }
    public string Scan()
    {
        return "多功能设备扫描的内容";
    }
}

// 程序入口
internal class Program
{
    static void Main()
    {
        // 必须放在最前
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding  = Encoding.UTF8;

        IPrintable printer = new BasicPrinter();
        printer.Print("测试页 A");

        IScannable scanner = new BasicScanner();
        Console.WriteLine(scanner.Scan());

        var allInOne = new AllInOneDevice();
        allInOne.Print("复合机打印任务");
        Console.WriteLine(allInOne.Scan());

        Console.WriteLine("演示结束，按任意键退出...");
        Console.ReadKey();
    }
}