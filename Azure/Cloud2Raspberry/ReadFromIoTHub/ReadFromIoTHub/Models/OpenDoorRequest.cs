using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFromIoTHub.Models;

internal class OpenDoorRequest
{
    public int? Id { get; set; }
    public string CodePic { get; set; }
    public string? CodeCloud { get; set; }
    public int IdPic { get; set; }
    public int IdUser { get; set; }
    public string Name { get; set; }
    public string SurName { get; set; }
    public int IdHouse { get; set; }
    public int IdRoom { get; set; }
    public DateTime Time { get; set; }

    public OpenDoorRequest()
    {

    }
}