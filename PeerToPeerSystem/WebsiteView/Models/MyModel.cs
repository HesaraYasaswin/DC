using System.Net.NetworkInformation;
using SharedLibrary;

namespace WebsiteView.Models
{
    public class MyModel
    {
        public List<Client>? Clients { get; set; }
        public List<Work_Stat> Stats = new List<Work_Stat>();

        public void AddStat(Work_Stat value)
        {
            Stats.Add(value);
        }
    }
}
