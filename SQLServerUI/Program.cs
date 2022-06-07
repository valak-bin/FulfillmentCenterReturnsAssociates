

using DataAccessLibrary;
using DataAccessLibrary.Models;
using Microsoft.Extensions.Configuration;



static string GetConnectionString(string connectionStringName = "Default")
{
    string output = "";

    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json");

    var config = builder.Build();

    output = config.GetConnectionString(connectionStringName);

    return output;
}

SqlCrud sql = new SqlCrud(GetConnectionString());

// CRUD Calls

ReadAllAssociates(sql);
//ReadAssociate(sql, 2);
//CreateNewAssociate(sql);
//UpdateContact(sql);

// Remove Natasha's Location // AssociateId = 2007, LocationId = 1008
//RemoveLocationFromContact(sql, 2007, 1008);

// CRUD Methods

static void ReadAllAssociates(SqlCrud sql)
{
    var rows = sql.GetAllAssociates();

    foreach (var row in rows)
    {
        Console.WriteLine($"{row.Id}: {row.FirstName} {row.LastName}");
    }
}

static void ReadAssociate(SqlCrud sql, int associateId)
{
    var associate = sql.GetFullAssociateById(associateId);

    Console.WriteLine($"Associate: {associate.BasicInfo.FirstName} {associate.BasicInfo.LastName}");

    foreach (var locations in associate.Locations)
    {
        Console.WriteLine($"Locations: {locations.LocationName}");
    }

    foreach (var shifts in associate.Shifts)
    {
        Console.WriteLine($"Shifts: {shifts.ShiftName}");
    }
}

static void CreateNewAssociate(SqlCrud sql)
{
    FullAssociateModel user = new FullAssociateModel()
    {
        BasicInfo = new BasicAssociateModel
        {
            FirstName = "Natasha",
            LastName = "Romanoff"
        }
    };

    user.Locations.Add(new LocationModel {LocationName = "S.H.I.E.L.D Headquarters Merchandise Returns" });

    user.Shifts.Add(new ShiftModel {ShiftName = "A" });

    sql.CreateAssociate(user);
}

void UpdateContact(SqlCrud sql)
{
    BasicAssociateModel associate = new BasicAssociateModel()
    {
        Id = 1,
        FirstName = "Elon",
        LastName = "Musk"
    };

    sql.UpdateContactName(associate);
}

static void RemoveLocationFromContact(SqlCrud sql, int associateId, int locationId)
{
    sql.RemoveLocationFromAssociate(associateId, locationId);
}
