using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModsPlus;

namespace PickupsPlus.Cards
{
    public class PickupsTestCard : CustomEffectCard<PickupsManager>
    {
        public override CardDetails Details => new CardDetails
        {
            Title = "Pickups Tester",
            ModName = "PP",
        };
    }
}
