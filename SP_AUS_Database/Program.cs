using SP_AUS_Database.Database;
using SP_AUS_Database.Tests;

namespace SP_AUS_Database
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PCRTestDatabase database = new PCRTestDatabase();
            DatabaseTests.GenerateData(database, 10_000, 10, 0.25);
        }
    }
}
