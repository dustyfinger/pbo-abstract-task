using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        BosRobot Boss1 = new BosRobot("Boss EZ", 400, 50, 45, 100);
        RobotBiasa Basic1 = new RobotBiasa("Rakyat Sipil", 280, 30, 60);
        Healer Healer1 = new Healer("Healer Robot", 250, 20, 30);

        // Battle dimulai
        Console.WriteLine("Battle dimulai dalam 3..2..1..\n");
        Console.WriteLine("FIGHT!\n");

        // Rakyat Sipil menyerang Boss
        Basic1.serang(Boss1);
        Boss1.cetakInformasi();

        // Boss menyerang Rakyat Sipil
        Boss1.serang(Basic1);
        Basic1.cetakInformasi();

        // Robot penyembuh menggunakan kemampuan untuk menyembuhkan Rakyat Sipil
        Healer1.gunakanKemampuan(new Repair());
        Basic1.cetakInformasi();

        // Boss menyerang lagi
        Boss1.serang(Basic1);
        Basic1.cetakInformasi();

        // Healer Robot menggunakan Super Shield
        Healer1.gunakanKemampuan(new Super_Shield());
        Basic1.cetakInformasi();

        // Rakyat Sipil menyerang Boss EZ
        Basic1.serang(Boss1);
        Boss1.cetakInformasi();

        // Rakyat Sipil menggunakan Plasma Cannon untuk menyerang Boss
        Basic1.gunakanKemampuan(new Plasma_Cannon());
        Boss1.cetakInformasi();

        // Menggunakan Electric Shock pada Rakyat Sipil
        Healer1.gunakanKemampuan(new Electric_Shock());
        Basic1.cetakInformasi();

        // Memeriksa apakah Boss sudah mati
        Boss1.mati();

        Console.WriteLine("Rakyat Sipil WIN!\n");
    }
}

public abstract class Robot
{
    // Properti
    public string nama { get; set; }
    public int energi { get; set; }
    public int armor { get; set; }
    public int serangan { get; set; }
    public int kecepatanNormal { get; set; }
    public Dictionary<string, int> cooldowns = new Dictionary<string, int>();

    // Constructor
    public Robot(string nama, int energi, int armor, int serangan, int kecepatanNormal = 100)
    {
        this.nama = nama;
        this.energi = energi;
        this.armor = armor;
        this.serangan = serangan;
        this.kecepatanNormal = kecepatanNormal;
    }

    public abstract void serang(Robot target);

    public abstract void gunakanKemampuan(IKemampuan kemampuan);

    public void cetakInformasi()
    {
        Console.WriteLine($"\nNama Robot : {nama}");
        Console.WriteLine($"Energi : {energi}");
        Console.WriteLine($"Armor : {armor}");
        Console.WriteLine($"Serangan : {serangan}");
        Console.WriteLine($"Kecepatan : {kecepatanNormal}");
    }

    public void pulihkanEnergi()
    {
        Console.WriteLine($"{nama} memulihkan energi sebesar 20.");
        energi += 20;

        // Mengurangi cooldown setiap giliran
        foreach (var kemampuan in cooldowns.Keys)
        {
            if (cooldowns[kemampuan] > 0)
            {
                cooldowns[kemampuan]--;
            }
        }
    }
}

public class BosRobot : Robot
{
    public int pertahanan;

    public BosRobot(string nama, int energi, int armor, int serangan, int pertahanan)
        : base(nama, energi, armor, serangan)
    {
        this.pertahanan = pertahanan;
    }

    public override void serang(Robot target)
    {
        Console.WriteLine($"{this.nama} menyerang {target.nama}");
        int damage = Math.Max(0, this.serangan + pertahanan / 2 - target.armor);
        target.energi -= damage;
        Console.WriteLine($"{target.nama} menerima {damage} kerusakan. Energi tersisa: {target.energi}");
    }

    public override void gunakanKemampuan(IKemampuan kemampuan)
    {
        kemampuan.Gunakan(this);
    }

    public void mati()
    {
        if (this.energi <= 0)
        {
            Console.WriteLine($"{this.nama} telah dikalahkan dan mati");
        }
    }
}

public class RobotBiasa : Robot
{
    public RobotBiasa(string nama, int energi, int armor, int serangan) : base(nama, energi, armor, serangan)
    {

    }

    public override void serang(Robot target)
    {
        Console.WriteLine($"{nama} menyerang {target.nama}");
        int damage = Math.Max(0, this.serangan - target.armor);
        target.energi -= damage;
        Console.WriteLine($"{target.nama} menerima {damage} kerusakan. Energi tersisa: {target.energi}");
    }

    public override void gunakanKemampuan(IKemampuan kemampuan)
    {
        if (cooldowns.ContainsKey(kemampuan.GetType().Name) && cooldowns[kemampuan.GetType().Name] > 0)
        {
            Console.WriteLine($"{nama} tidak bisa menggunakan {kemampuan.GetType().Name}, masih dalam cooldown.");
        }
        else
        {
            kemampuan.Gunakan(this);
            cooldowns[kemampuan.GetType().Name] = 3; // cooldown 3 giliran
        }
    }
}

public class Healer : Robot
{
    public Healer(string nama, int energi, int armor, int serangan)
        : base(nama, energi, armor, serangan)
    {

    }

    public override void serang(Robot target)
    {
        Console.WriteLine($"{nama} menyerang {target.nama}");
        int damage = Math.Max(0, this.serangan - target.armor);
        target.energi -= damage;
        Console.WriteLine($"{target.nama} menerima {damage} kerusakan. Energi tersisa: {target.energi}");
    }

    public override void gunakanKemampuan(IKemampuan kemampuan)
    {
        if (cooldowns.ContainsKey(kemampuan.GetType().Name) && cooldowns[kemampuan.GetType().Name] > 0)
        {
            Console.WriteLine($"{nama} tidak bisa menggunakan {kemampuan.GetType().Name}, masih dalam cooldown.");
        }
        else
        {
            kemampuan.Gunakan(this);
            cooldowns[kemampuan.GetType().Name] = 3; // cooldown 3 giliran
        }
    }

    public void menyembuhkan(Robot target)
    {
        Console.WriteLine($"{nama} menyembuhkan {target.nama} sebesar 30 energi.");
        target.energi += 30;
        Console.WriteLine($"Energi {target.nama} setelah disembuhkan: {target.energi}");
    }
}


public interface IKemampuan
{
    void Gunakan(Robot target);
}

public class Repair : IKemampuan
{
    public void Gunakan(Robot target)
    {
        Console.WriteLine($"Energi {target.nama} telah dipulihkan sebesar 10");
        target.energi += 10;
        Console.WriteLine($"Energi {target.nama} dipulihkan, sisa energi : {target.energi}");
    }
}

public class Electric_Shock : IKemampuan
{
    public void Gunakan(Robot target)
    {
        Console.WriteLine($"{target.nama} mengalami penurunan kecepatan sebesar 5");
        target.kecepatanNormal -= 5;
        Console.WriteLine($"Kecepatan {target.nama} sekarang: {target.kecepatanNormal}");
    }
}

public class Plasma_Cannon : IKemampuan
{
    public void Gunakan(Robot target)
    {
        Console.WriteLine($"{target.nama} terkena serangan Plasma Cannon!");
        int destroy_armor = target.armor;
        target.armor = 0;
        Console.WriteLine($"{target.nama} armor hancur, armor sekarang: {target.armor}");
    }
}

public class Super_Shield : IKemampuan
{
    public void Gunakan(Robot target)
    {
        Console.WriteLine($"Super Shield diaktifkan untuk {target.nama}. Armor bertambah sementara sebesar 20");
        int addedArmor = 20;
        target.armor += addedArmor;

        for (int i = 0; i < 3; i++)
        {
            Console.WriteLine($"Super Shield melindungi serangan {i + 1}. Armor tersisa: {target.armor}");
        }

        target.armor -= addedArmor;
        Console.WriteLine($"Super Shield habis. Armor {target.nama} kembali ke nilai awal: {target.armor}");
    }
}
