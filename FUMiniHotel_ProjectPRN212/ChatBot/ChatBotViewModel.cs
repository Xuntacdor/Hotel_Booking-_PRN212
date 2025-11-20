using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Configuration; // <-- Needed to read App.config
using DataAccessLayer;
using BusinessObjects;
using System.Linq;

namespace FUMiniHotel_ProjectPRN212.ChatBot
{
    public class ChatBotViewModel : INotifyPropertyChanged
    {
        private string _currentMessage;
        private readonly Action _scrollAction;
        private readonly RoomDAO _roomDAO;
        private readonly BookingDAO _bookingDAO;
        private readonly int _customerId;

        public ObservableCollection<ChatMessage> Messages { get; set; }
        public ICommand SendMessageCommand { get; set; }

        public string CurrentMessage
        {
            get => _currentMessage;
            set { _currentMessage = value; OnPropertyChanged(); }
        }

        public ChatBotViewModel(Action scrollAction, int customerId = 0)
        {
            Messages = new ObservableCollection<ChatMessage>();
            _scrollAction = scrollAction;
            _customerId = customerId;
            _roomDAO = new RoomDAO();
            _bookingDAO = new BookingDAO();
            SendMessageCommand = new RelayCommand(async _ => await SendMessageAsync());
        }

        public async Task SendMessageAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentMessage))
                return;

            // Add user message
            Messages.Add(new ChatMessage
            {
                Message = CurrentMessage,
                IsUser = true
            });
            _scrollAction();

            string userInput = CurrentMessage;
            CurrentMessage = "";

            // Check if user wants to book a room
            string bookingResult = await TryProcessBookingAsync(userInput);
            if (!string.IsNullOrEmpty(bookingResult))
            {
                Messages.Add(new ChatMessage
                {
                    Message = bookingResult,
                    IsUser = false
                });
                _scrollAction();
                return;
            }

            // 🔥 Call OpenAI API for general chat
            string aiResponse = await GetAIResponseAsync(userInput);

            Messages.Add(new ChatMessage
            {
                Message = aiResponse,
                IsUser = false
            });
            _scrollAction();
        }

        private async Task<string> TryProcessBookingAsync(string userMessage)
        {
            // Check if user wants to book (in Vietnamese or English)
            string lowerMessage = userMessage.ToLower();
            bool isBookingRequest = (lowerMessage.Contains("đặt phòng") || 
                                   lowerMessage.Contains("book") ||
                                   lowerMessage.Contains("reserve") ||
                                   (lowerMessage.Contains("đặt") && lowerMessage.Contains("phòng"))) &&
                                   _customerId > 0;

            if (!isBookingRequest)
                return string.Empty;

            try
            {
                // Check if user just wants to see available rooms (no specific dates/room mentioned)
                bool hasSpecificInfo = lowerMessage.Any(char.IsDigit) || 
                                      lowerMessage.Contains("ngày") ||
                                      lowerMessage.Contains("date") ||
                                      lowerMessage.Contains("từ") ||
                                      lowerMessage.Contains("đến") ||
                                      lowerMessage.Contains("from") ||
                                      lowerMessage.Contains("to");

                // If no specific info, show available rooms
                if (!hasSpecificInfo)
                {
                    return GetAvailableRoomsList();
                }

                // Use AI to extract booking information
                var bookingInfo = await ExtractBookingInfoAsync(userMessage);
                
                if (bookingInfo == null || bookingInfo.StartDate == DateTime.MinValue)
                {
                    return GetAvailableRoomsList() + 
                           "\n\n📝 Để đặt phòng, vui lòng cung cấp:\n" +
                           "- Số phòng (ví dụ: Phòng 101)\n" +
                           "- Ngày nhận phòng (ví dụ: 25/12/2024)\n" +
                           "- Ngày trả phòng (ví dụ: 27/12/2024)\n\n" +
                           "Ví dụ: 'Đặt phòng 101 từ 25/12/2024 đến 27/12/2024'";
                }

                // Validate dates
                if (bookingInfo.StartDate >= bookingInfo.EndDate)
                {
                    return "Ngày trả phòng phải sau ngày nhận phòng. Vui lòng kiểm tra lại.\n\n" + GetAvailableRoomsList();
                }

                if (bookingInfo.StartDate < DateTime.Today)
                {
                    return "Ngày nhận phòng không thể là ngày trong quá khứ. Vui lòng chọn ngày từ hôm nay trở đi.\n\n" + GetAvailableRoomsList();
                }

                // Find room by number or get first available room
                BusinessObjects.Room selectedRoom = null;
                
                if (!string.IsNullOrEmpty(bookingInfo.RoomNumber))
                {
                    var allRooms = _roomDAO.GetAllRooms();
                    selectedRoom = allRooms.FirstOrDefault(r => 
                        r.RoomNumber.Equals(bookingInfo.RoomNumber, StringComparison.OrdinalIgnoreCase));
                    
                    if (selectedRoom == null)
                    {
                        return $"Không tìm thấy phòng {bookingInfo.RoomNumber}. Vui lòng kiểm tra lại số phòng.";
                    }
                }
                else if (bookingInfo.RoomTypeId.HasValue)
                {
                    // Get first available room of this type
                    var availableRooms = _roomDAO.GetAvailableRooms(
                        bookingInfo.StartDate, 
                        bookingInfo.EndDate, 
                        bookingInfo.RoomTypeId);
                    
                    if (!availableRooms.Any())
                    {
                        return $"Không còn phòng loại này trong khoảng thời gian từ {bookingInfo.StartDate:dd/MM/yyyy} đến {bookingInfo.EndDate:dd/MM/yyyy}.";
                    }
                    
                    selectedRoom = availableRooms.First();
                }
                else
                {
                    // Get first available room
                    var availableRooms = _roomDAO.GetAvailableRooms(
                        bookingInfo.StartDate, 
                        bookingInfo.EndDate);
                    
                    if (!availableRooms.Any())
                    {
                        return $"Không còn phòng trống trong khoảng thời gian từ {bookingInfo.StartDate:dd/MM/yyyy} đến {bookingInfo.EndDate:dd/MM/yyyy}.";
                    }
                    
                    selectedRoom = availableRooms.First();
                }

                // Check if room is available
                bool isAvailable = _bookingDAO.IsRoomAvailable(
                    selectedRoom.RoomId, 
                    bookingInfo.StartDate, 
                    bookingInfo.EndDate);

                if (!isAvailable)
                {
                    return $"Phòng {selectedRoom.RoomNumber} đã được đặt trong khoảng thời gian này. Vui lòng chọn phòng khác hoặc thay đổi ngày.";
                }

                // Calculate total price
                int days = (bookingInfo.EndDate - bookingInfo.StartDate).Days;
                decimal totalPrice = selectedRoom.PricePerDay * days;

                // Create booking
                var booking = new BusinessObjects.Booking
                {
                    CustomerId = _customerId,
                    BookingDate = DateTime.Now,
                    Status = "Confirmed",
                    TotalPrice = totalPrice
                };

                var roomBookings = new List<BookingDetail>
                {
                    new BookingDetail
                    {
                        RoomId = selectedRoom.RoomId,
                        StartDate = bookingInfo.StartDate,
                        EndDate = bookingInfo.EndDate,
                        ActualPrice = selectedRoom.PricePerDay
                    }
                };

                var result = _bookingDAO.CreateBooking(booking, roomBookings, null);

                return $"✅ Đặt phòng thành công!\n\n" +
                       $"📋 Mã đặt phòng: {result.BookingId}\n" +
                       $"🏨 Phòng: {selectedRoom.RoomNumber} ({selectedRoom.RoomType?.TypeName ?? "N/A"})\n" +
                       $"📅 Từ: {bookingInfo.StartDate:dd/MM/yyyy}\n" +
                       $"📅 Đến: {bookingInfo.EndDate:dd/MM/yyyy}\n" +
                       $"💰 Tổng tiền: {totalPrice:N0} VNĐ\n\n" +
                       $"Cảm ơn bạn đã đặt phòng tại FU Mini Hotel!";
            }
            catch (Exception ex)
            {
                return $"❌ Lỗi khi đặt phòng: {ex.Message}\n\nVui lòng thử lại hoặc liên hệ bộ phận hỗ trợ.";
            }
        }

        private async Task<BookingInfo> ExtractBookingInfoAsync(string userMessage)
        {
            try
            {
                string apiKey = ConfigurationManager.AppSettings["OpenAIKey"];
                if (string.IsNullOrWhiteSpace(apiKey))
                    return null;

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                // Get available rooms for context
                var allRooms = _roomDAO.GetAllRooms();
                var roomList = string.Join(", ", allRooms.Take(10).Select(r => $"Phòng {r.RoomNumber} (Loại: {r.RoomType?.TypeName ?? "N/A"})"));

                var systemPrompt = @"You are a booking assistant. Extract booking information from user messages.
Return ONLY a JSON object with this exact format:
{
  ""roomNumber"": ""101"" or null if not specified,
  ""roomTypeId"": 1 or null if not specified,
  ""startDate"": ""2024-12-25"" (YYYY-MM-DD format) or null,
  ""endDate"": ""2024-12-27"" (YYYY-MM-DD format) or null
}
If date is not specified, use today as startDate and tomorrow as endDate.
If room is not specified, set both roomNumber and roomTypeId to null.
Return ONLY the JSON, no other text.";

                var requestBody = new
                {
                    model = "gpt-4o-mini",
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = $"Available rooms: {roomList}\n\nUser message: {userMessage}" }
                    },
                    temperature = 0.3
                };

                string json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

                string resultJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return null;

                using var doc = JsonDocument.Parse(resultJson);
                if (doc.RootElement.TryGetProperty("choices", out var choicesElement) &&
                    choicesElement.GetArrayLength() > 0)
                {
                    var firstChoice = choicesElement[0];
                    if (firstChoice.TryGetProperty("message", out var messageElement) &&
                        messageElement.TryGetProperty("content", out var contentElement))
                    {
                        string aiResponse = contentElement.GetString();
                        
                        // Extract JSON from response (might have markdown code blocks)
                        string jsonContent = aiResponse.Trim();
                        if (jsonContent.StartsWith("```json"))
                            jsonContent = jsonContent.Substring(7);
                        if (jsonContent.StartsWith("```"))
                            jsonContent = jsonContent.Substring(3);
                        if (jsonContent.EndsWith("```"))
                            jsonContent = jsonContent.Substring(0, jsonContent.Length - 3);
                        jsonContent = jsonContent.Trim();

                        using var bookingDoc = JsonDocument.Parse(jsonContent);
                        var root = bookingDoc.RootElement;

                        var bookingInfo = new BookingInfo();

                        if (root.TryGetProperty("roomNumber", out var roomNumberElement))
                        {
                            var roomNum = roomNumberElement.GetString();
                            if (!string.IsNullOrEmpty(roomNum) && roomNum != "null")
                                bookingInfo.RoomNumber = roomNum;
                        }

                        if (root.TryGetProperty("roomTypeId", out var roomTypeElement))
                        {
                            if (roomTypeElement.ValueKind != JsonValueKind.Null)
                                bookingInfo.RoomTypeId = roomTypeElement.GetInt32();
                        }

                        // Only set dates if they were actually provided in the message
                        bool hasDates = userMessage.ToLower().Any(c => char.IsDigit(c)) &&
                                       (userMessage.ToLower().Contains("ngày") ||
                                        userMessage.ToLower().Contains("date") ||
                                        userMessage.ToLower().Contains("từ") ||
                                        userMessage.ToLower().Contains("đến") ||
                                        userMessage.ToLower().Contains("from") ||
                                        userMessage.ToLower().Contains("to"));

                        if (hasDates)
                        {
                            DateTime startDate = DateTime.Today;
                            if (root.TryGetProperty("startDate", out var startDateElement))
                            {
                                var startDateStr = startDateElement.GetString();
                                if (!string.IsNullOrEmpty(startDateStr) && startDateStr != "null")
                                {
                                    if (DateTime.TryParse(startDateStr, out var parsedStart))
                                        startDate = parsedStart;
                                }
                            }
                            bookingInfo.StartDate = startDate;

                            DateTime endDate = DateTime.Today.AddDays(1);
                            if (root.TryGetProperty("endDate", out var endDateElement))
                            {
                                var endDateStr = endDateElement.GetString();
                                if (!string.IsNullOrEmpty(endDateStr) && endDateStr != "null")
                                {
                                    if (DateTime.TryParse(endDateStr, out var parsedEnd))
                                        endDate = parsedEnd;
                                }
                            }
                            bookingInfo.EndDate = endDate;
                        }
                        else
                        {
                            // No dates specified - set to MinValue to indicate missing
                            bookingInfo.StartDate = DateTime.MinValue;
                            bookingInfo.EndDate = DateTime.MinValue;
                        }

                        return bookingInfo;
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private class BookingInfo
        {
            public string RoomNumber { get; set; }
            public int? RoomTypeId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        private async Task<string> GetAIResponseAsync(string userMessage)
        {
            try
            {
                // ✅ Read API key from App.config
                string apiKey = ConfigurationManager.AppSettings["OpenAIKey"];
                if (string.IsNullOrWhiteSpace(apiKey))
                    return "Error: API key is missing.";

                // Check if user is asking about rooms
                string roomContext = GetRoomAvailabilityContext(userMessage);

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                // Build system message with room information
                var systemMessage = @"You are a helpful assistant for FU Mini Hotel. You help customers with room bookings and hotel information. 
When customers ask about available rooms or booked rooms, provide accurate information based on the room data provided.
You can also help customers book rooms. When they want to book, guide them to provide: room number (or room type), check-in date, and check-out date.
Example: 'Đặt phòng 101 từ 25/12/2024 đến 27/12/2024' or 'Book room 101 from 25/12/2024 to 27/12/2024'.
Respond in Vietnamese when the customer writes in Vietnamese, otherwise respond in English.";

                var messages = new List<object>
                {
                    new { role = "system", content = systemMessage }
                };

                // Add room context if available
                if (!string.IsNullOrEmpty(roomContext))
                {
                    messages.Add(new { role = "user", content = $"Room Information:\n{roomContext}\n\nCustomer Question: {userMessage}" });
                }
                else
                {
                    messages.Add(new { role = "user", content = userMessage });
                }

                var requestBody = new
                {
                    model = "gpt-4o-mini",
                    messages = messages
                };

                string json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

                string resultJson = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(resultJson);

                if (!response.IsSuccessStatusCode)
                {
                    if (doc.RootElement.TryGetProperty("error", out var errorElement) &&
                        errorElement.TryGetProperty("message", out var messageElement))
                    {
                        return $"OpenAI error: {messageElement.GetString()}";
                    }

                    return $"OpenAI error: HTTP {(int)response.StatusCode}";
                }

                if (doc.RootElement.TryGetProperty("choices", out var choicesElement) &&
                    choicesElement.GetArrayLength() > 0)
                {
                    var firstChoice = choicesElement[0];
                    if (firstChoice.TryGetProperty("message", out var messageElement) &&
                        messageElement.TryGetProperty("content", out var contentElement))
                    {
                        string aiReply = contentElement.GetString();
                        return aiReply ?? "No response from AI.";
                    }
                }

                return "OpenAI error: response payload missing chat content.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        private string GetAvailableRoomsList()
        {
            try
            {
                var allRooms = _roomDAO.GetAllRooms();
                var today = DateTime.Today;
                var nextWeek = today.AddDays(7);

                // Get available rooms for next week
                var availableRooms = _roomDAO.GetAvailableRooms(today, nextWeek);

                var roomInfo = new StringBuilder();
                roomInfo.AppendLine("🏨 **DANH SÁCH PHÒNG CÓ SẴN**\n");

                if (availableRooms.Any())
                {
                    roomInfo.AppendLine($"Hiện có {availableRooms.Count} phòng có sẵn:\n");
                    
                    foreach (var room in availableRooms.Take(15))
                    {
                        roomInfo.AppendLine($"• **Phòng {room.RoomNumber}**");
                        roomInfo.AppendLine($"  - Loại: {room.RoomType?.TypeName ?? "N/A"}");
                        roomInfo.AppendLine($"  - Giá: {room.PricePerDay:N0} VNĐ/ngày");
                        roomInfo.AppendLine($"  - Sức chứa: {room.MaxCapacity} người");
                        if (!string.IsNullOrEmpty(room.Description))
                            roomInfo.AppendLine($"  - Mô tả: {room.Description}");
                        roomInfo.AppendLine();
                    }

                    if (availableRooms.Count > 15)
                        roomInfo.AppendLine($"... và {availableRooms.Count - 15} phòng khác\n");

                    roomInfo.AppendLine("💡 Để đặt phòng, vui lòng cho tôi biết:");
                    roomInfo.AppendLine("   - Số phòng bạn muốn đặt");
                    roomInfo.AppendLine("   - Ngày nhận phòng (check-in)");
                    roomInfo.AppendLine("   - Ngày trả phòng (check-out)");
                    roomInfo.AppendLine("\nVí dụ: 'Đặt phòng 101 từ 25/12/2024 đến 27/12/2024'");
                }
                else
                {
                    roomInfo.AppendLine("Hiện tại không còn phòng trống trong tuần tới.");
                    roomInfo.AppendLine("Vui lòng thử lại với khoảng thời gian khác.");
                }

                return roomInfo.ToString();
            }
            catch (Exception ex)
            {
                return $"Lỗi khi lấy danh sách phòng: {ex.Message}";
            }
        }

        private string GetRoomAvailabilityContext(string userMessage)
        {
            // Check if user is asking about rooms (in Vietnamese or English)
            string lowerMessage = userMessage.ToLower();
            bool isRoomQuery = lowerMessage.Contains("phòng") || 
                              lowerMessage.Contains("room") ||
                              lowerMessage.Contains("đặt phòng") ||
                              lowerMessage.Contains("book") ||
                              lowerMessage.Contains("available") ||
                              lowerMessage.Contains("trống") ||
                              lowerMessage.Contains("có sẵn") ||
                              lowerMessage.Contains("đã đặt") ||
                              lowerMessage.Contains("booked");

            if (!isRoomQuery)
                return string.Empty;

            try
            {
                var allRooms = _roomDAO.GetAllRooms();
                var today = DateTime.Today;
                var nextMonth = today.AddMonths(1);

                // Get all rooms with their current status
                var roomInfo = new StringBuilder();
                roomInfo.AppendLine("=== THÔNG TIN PHÒNG KHÁCH SẠN ===");
                roomInfo.AppendLine();

                // Get booked rooms for today
                var bookedRoomIds = _bookingDAO.GetAllBookingsWithDetails()
                    .Where(b => b.Status != "Cancelled")
                    .SelectMany(b => b.BookingDetails)
                    .Where(bd => bd.StartDate <= today && bd.EndDate >= today)
                    .Select(bd => bd.RoomId)
                    .Distinct()
                    .ToList();

                // Get available rooms (not booked today)
                var availableRooms = allRooms
                    .Where(r => r.Status == "Available" || r.Status == "Active")
                    .Where(r => !bookedRoomIds.Contains(r.RoomId))
                    .ToList();

                var bookedRooms = allRooms
                    .Where(r => bookedRoomIds.Contains(r.RoomId))
                    .ToList();

                roomInfo.AppendLine($"TỔNG SỐ PHÒNG: {allRooms.Count}");
                roomInfo.AppendLine($"PHÒNG CÓ SẴN (hôm nay): {availableRooms.Count}");
                roomInfo.AppendLine($"PHÒNG ĐÃ ĐẶT (hôm nay): {bookedRooms.Count}");
                roomInfo.AppendLine();

                if (availableRooms.Any())
                {
                    roomInfo.AppendLine("--- PHÒNG CÓ SẴN ---");
                    foreach (var room in availableRooms.Take(20)) // Limit to 20 rooms
                    {
                        roomInfo.AppendLine($"• Phòng {room.RoomNumber} - {room.RoomType?.TypeName ?? "N/A"} - " +
                                          $"Giá: {room.PricePerDay:N0} VNĐ/ngày - " +
                                          $"Sức chứa: {room.MaxCapacity} người");
                        if (!string.IsNullOrEmpty(room.Description))
                            roomInfo.AppendLine($"  Mô tả: {room.Description}");
                    }
                    if (availableRooms.Count > 20)
                        roomInfo.AppendLine($"... và {availableRooms.Count - 20} phòng khác");
                    roomInfo.AppendLine();
                }

                if (bookedRooms.Any())
                {
                    roomInfo.AppendLine("--- PHÒNG ĐÃ ĐẶT (hôm nay) ---");
                    var bookingDetails = _bookingDAO.GetAllBookingsWithDetails()
                        .Where(b => b.Status != "Cancelled")
                        .SelectMany(b => b.BookingDetails)
                        .Where(bd => bd.StartDate <= today && bd.EndDate >= today)
                        .ToList();

                    foreach (var detail in bookingDetails.Take(15))
                    {
                        var room = detail.Room;
                        if (room != null)
                        {
                            roomInfo.AppendLine($"• Phòng {room.RoomNumber} - " +
                                              $"Từ {detail.StartDate:dd/MM/yyyy} đến {detail.EndDate:dd/MM/yyyy}");
                        }
                    }
                    if (bookingDetails.Count > 15)
                        roomInfo.AppendLine($"... và {bookingDetails.Count - 15} đặt phòng khác");
                }

                return roomInfo.ToString();
            }
            catch (Exception ex)
            {
                return $"Lỗi khi lấy thông tin phòng: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
