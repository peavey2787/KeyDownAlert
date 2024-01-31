using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyDownAlert
{
    public class Buttons
    {
        public List<Button> ButtonList { get; set; }
        public Buttons() 
        {
            ButtonList = new List<Button>();
        }
    }

    public class Button
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
        public Button()
        {
            Id = "";
            Name = "";
            Action = "";
        }
        public override string ToString()
        {
            return Action;
        }
    }
}
