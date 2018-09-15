using DataAccess.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entity
{
    [Table(name: DBTableConstant.Log)]
    public class Log
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(name: DBColumnConstant.Id)]
        public int Id { get; set; }

        [Column(name: DBColumnConstant.DateTime)]
        public int LogDateTime { get; set; }

        [Column(name: DBColumnConstant.Information)]
        public int Information { get; set; }
    }
}
