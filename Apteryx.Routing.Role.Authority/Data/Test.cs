using Apteryx.MongoDB.Driver.Extend;
using MongoDB.Bson;

namespace Apteryx.Routing.Role.Authority
{
    public class Test : BaseMongoEntity
    {
        public Test()
        {
            this.Id = ObjectId.GenerateNewId().ToString();
        }
        //public string Name { get; set; } = $"test{new Random().Next(100, 999)}";
        //public int Age { get; set; } = new Random().Next(10, 99);
        //public bool IsAdmin { get; set; } = new Random().Next(0, 2) == 1;
        //public double Money { get; set; } = new Random().Next(100, 999);
        public decimal MoneyDecimal { get; set; } = new Random().Next(100, 999);
        //public long MoneyLong { get; set; } = new Random().Next(100, 999);
        //public float MoneyFloat { get; set; } = new Random().Next(100, 999);
        //public short MoneyShort { get; set; } = (short)new Random().Next(100, 999);
        //public Int32 MoneyInt32 { get; set; } = new Random().Next(100, 999);
        //public Int64 MoneyInt64 { get; set; } = new Random().Next(100, 999);
        //public List<string> Names { get; set; } = new List<string>() { "张三", "李四", "王五" };
        //public List<int> Ages { get; set; } = new List<int>() { 10, 20, 30 };
        //public List<bool> IsAdmins { get; set; } = new List<bool>() { true, false, true };
        //public List<DateTime> CreateTimes { get; set; } = new List<DateTime>() { DateTime.Now, DateTime.Now, DateTime.Now };
        //public List<double> Moneys { get; set; } = new List<double>() { 100, 200, 300 };
        public List<decimal> MoneyDecimals { get; set; } = new List<decimal>() { 100, 200, 300 };
        //public List<long> MoneyLongs { get; set; } = new List<long>() { 100, 200, 300 };
        //public List<float> MoneyFloats { get; set; } = new List<float>() { 100, 200, 300 };
        //public List<short> MoneyShorts { get; set; } = new List<short>() { 100, 200, 300 };
        //public List<Int32> MoneyInt32s { get; set; } = new List<Int32>() { 100, 200, 300 };
        //public List<Int64> MoneyInt64s { get; set; } = new List<Int64>() { 100, 200, 300 };
        public Test? TestInfo { get; set; }
        public List<Test>? Tests { get; set; }
    }
}
