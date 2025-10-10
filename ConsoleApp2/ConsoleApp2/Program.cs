using System;
using System.Text;

// 支付抽象基类（现有代码）
public abstract class PaymentMethod
{
    public abstract bool ProcessPayment(decimal amount);
    public abstract string GetReceipt();
}

// 现有支付方式（无需修改）
public class CreditCardPayment : PaymentMethod
{
    public override bool ProcessPayment(decimal amount)
    {
        Console.WriteLine($"信用卡支付 {amount:C}");
        return true;
    }
    public override string GetReceipt() => "信用卡支付凭证";
}

public class PayPalPayment : PaymentMethod
{
    public override bool ProcessPayment(decimal amount)
    {
        Console.WriteLine($"PayPal支付 {amount:C}");
        return true;
    }
    public override string GetReceipt() => "PayPal支付凭证";
}

// 新增支付方式（符合基类行为契约）
public class CryptoPayment : PaymentMethod
{
    public override bool ProcessPayment(decimal amount)
    {
        Console.WriteLine($"数字货币支付 {amount:C}");
        return true; // 遵循基类"返回支付是否成功"的契约
    }
    public override string GetReceipt() => "数字货币支付凭证";
}

// 支付处理服务（现有代码，无需修改）
public class PaymentProcessor
{
    public void Process(PaymentMethod payment, decimal amount)
    {
        if (payment.ProcessPayment(amount))
        {
            Console.WriteLine($"支付成功：{payment.GetReceipt()}");
        }
    }
}

// 程序入口：演示三种支付方式
public class Program
{
    public static void Main()
    {
        // 解决中文乱码
        Console.OutputEncoding = Encoding.UTF8;

        var processor = new PaymentProcessor();
        PaymentMethod[] methods =
        {
            new CreditCardPayment(),
            new PayPalPayment(),
            new CryptoPayment()
        };

        foreach (var m in methods)
        {
            processor.Process(m, 123.45m);
        }

        Console.WriteLine("演示完成，按任意键退出...");
        Console.ReadKey();
    }
}