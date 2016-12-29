
using System.Xml.Serialization;
using Reino.ClientConfig;

namespace Duncan.AI.Droid.Utils.EditControlManagement.Controls
{
    public class CustomTTControl : TTControl
    {
        // Association with a TextBoxBehavior

        public CustomTTControl()
        {
            BehaviorAndroid = null;
        }

        [XmlIgnore]
        public new EditControlBehavior BehaviorAndroid { get; set; }
    }
}