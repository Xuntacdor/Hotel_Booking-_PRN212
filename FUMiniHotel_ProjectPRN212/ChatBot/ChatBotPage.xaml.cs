using System.Windows.Controls;

namespace FUMiniHotel_ProjectPRN212.ChatBot
{
    public partial class ChatBotPage : Page
    {
        private ChatBotViewModel _vm;

        public ChatBotPage(int customerId = 0)
        {
            InitializeComponent();
            _vm = new ChatBotViewModel(ScrollToBottom, customerId);
            DataContext = _vm;
        }

        private void ScrollToBottom()
        {
            ChatScroll.ScrollToEnd();
        }
    }
}
