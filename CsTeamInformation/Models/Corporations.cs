using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;

namespace CsTeamInformation
{
    public partial class Corporations
    {
        public Corporations()
        {
            Teams = new HashSet<Teams>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        [DisplayName("Emblem Name")]
        public string Emblem { get; set; }

        public virtual ICollection<Teams> Teams { get; set; }
        [NotMapped]
        [DisplayName("Upload File")]
        public IFormFile ImageFile { get; set; }
    }
}
