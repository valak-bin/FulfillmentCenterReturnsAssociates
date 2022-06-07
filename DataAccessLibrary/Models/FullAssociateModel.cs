using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
    public class FullAssociateModel
    {
        public BasicAssociateModel BasicInfo { get; set; }        
        public List<LocationModel> Locations { get; set; } = new List<LocationModel>(); 
        public List<ShiftModel> Shifts { get; set; } = new List<ShiftModel>();
    }
}
