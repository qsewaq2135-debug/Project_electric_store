using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBIKES09
{
    public class CustomMessageBoxViewModel
    {
        public string Message { get; set; }
        public string Title { get; set; }

        public string CopyTitle { get; set; }
        public bool ShowCopyButton { get; set; }

        public CustomMessageBoxViewModel(string message, string title, string copyTitle, bool showCopyButton)
        {
            Message = message;
            Title = title;
            CopyTitle = copyTitle;
            ShowCopyButton = showCopyButton;
        }
    }
}
