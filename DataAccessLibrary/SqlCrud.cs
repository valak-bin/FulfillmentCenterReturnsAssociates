using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary
{
    public class SqlCrud
    {
        private readonly string _connectionString;
        private SqlDataAccess db = new SqlDataAccess();

        public SqlCrud(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<BasicAssociateModel> GetAllAssociates()
        {
            string sql = "Select [Id], [FirstName], [LastName] From [dbo].[Associate];";

            return db.LoadData<BasicAssociateModel, dynamic>(sql, new { }, _connectionString);
        }

        public FullAssociateModel GetFullAssociateById(int id)
        {
            string sql = "Select [Id], [FirstName], [LastName] From [dbo].[Associate] Where [Id] = @Id";

            FullAssociateModel output = new FullAssociateModel();

            output.BasicInfo = db.LoadData<BasicAssociateModel, dynamic>(sql, new { Id = id }, _connectionString).FirstOrDefault();

            if (output.BasicInfo == null)
            {
                return null;
            }

            sql = @"Select [LocationName] 
                    From [dbo].[Location] 
                        Inner Join [dbo].[AssociateLocation] 
                            On [Location].[Id] = [AssociateLocation].[LocationId]
                    Where [AssociateLocation].[AssociateId] = @Id;";

            output.Locations = db.LoadData<LocationModel, dynamic>(sql, new { Id = id }, _connectionString);

            sql = @"Select [ShiftName] 
                    From [dbo].[Shift] 
                        Inner Join [dbo].[AssociateShift] 
                            On [Shift].[Id] = [AssociateShift].[ShiftId]
                    Where [AssociateShift].[AssociateId] = @Id;";

            output.Shifts = db.LoadData<ShiftModel, dynamic>(sql, new { Id = id }, _connectionString);

            return output;
        }

        public void CreateAssociate(FullAssociateModel associate)
        {
            string sql = "Insert Into [dbo].[Associate] ([FirstName], [LastName]) Values (@FirstName, @LastName);";

            // Save the basic contact info
            db.SaveData(sql, new { associate.BasicInfo.FirstName, associate.BasicInfo.LastName }, _connectionString);
            
            // Get the Id number of an associate, lookup by the FirstName and LastName
            sql = "Select [Id] From [dbo].[Associate] Where [FirstName] = @FirstName And [LastName] = @LastName;";
                       
            int associateId = db.LoadData<IdLookupModel, dynamic>(
                sql, new { associate.BasicInfo.FirstName, associate.BasicInfo.LastName }, _connectionString).First().Id;


            foreach (var location in associate.Locations)
            {
                if (location.Id == 0)
                {
                    sql = "Insert Into [dbo].[Location] ([LocationName]) Values (@LocationName);";

                    db.SaveData(sql, new { location.LocationName }, _connectionString);

                    sql = "Select [Id] From [dbo].[Location] Where [LocationName] = @LocationName;";

                    location.Id = db.LoadData<IdLookupModel, dynamic>(
                        sql,
                        new { location.LocationName },
                        _connectionString).First().Id;
                }

                sql = "Insert Into [dbo].[AssociateLocation] ([AssociateId], [LocationId]) Values (@AssociateId, @LocationId);";
                db.SaveData(sql, new { AssociateId = associateId, LocationId = location.Id }, _connectionString);
            }

            foreach (var shift in associate.Shifts)
            {
                if (shift.Id == 0)
                {
                    sql = "Insert Into [dbo].[Shift] ([ShiftName]) Values (@ShiftName);";

                    db.SaveData(sql, new { shift.ShiftName }, _connectionString);

                    sql = "Select [Id] From [dbo].[Shift] Where [ShiftName] = @ShiftName;";

                    shift.Id = db.LoadData<IdLookupModel, dynamic>(
                        sql,
                        new { shift.ShiftName },
                        _connectionString).First().Id;
                }

                sql = "Insert Into [dbo].[AssociateShift] ([AssociateId], [ShiftId]) Values (@AssociateId, @ShiftId);";
                db.SaveData(sql, new { AssociateId = associateId, ShiftId = shift.Id }, _connectionString);
            }












        }

        public void UpdateContactName(BasicAssociateModel associate)
        {
            string sql = "Update [dbo].[Associate] Set [FirstName] = @FirstName, [LastName] = @LastName Where [Id] = @Id;";

            db.SaveData(sql, associate, _connectionString);
        }

        public void RemoveLocationFromAssociate(int associateId, int locationId)
        {
            string sql = "Select [Id], [AssociateId], [LocationId] From [AssociateLocation] Where [LocationId] = @LocationId;";
            
            var links = db.LoadData<AssociateLocationModel, dynamic>(sql, new { LocationId = locationId }, _connectionString);

            sql = "Delete From [dbo].[AssociateLocation] Where [LocationId] = @LocationId And [AssociateId] = @AssociateId;";

            db.SaveData(sql, new { LocationId = locationId, AssociateId = associateId }, _connectionString);

            if (links.Count == 1)
            {
                sql = "Delete From [dbo].[Location] Where Id = @LocationId;";

                db.SaveData(sql, new { LocationId = locationId }, _connectionString);
            }
        }
        
    }
}

