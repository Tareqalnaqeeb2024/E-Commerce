//using System.Collections.Concurrent;
//using System.Diagnostics;
//using System.Security.Claims;

//namespace E_Commerce.Midlleware
//{
//    public class RateLimitingMiddleware
//    {
//        private readonly RequestDelegate  _next;
//       private static DateTime _LastRequestDate = DateTime.Now;
//        private static  int _Counter;

//        public RateLimitingMiddleware(RequestDelegate next)
//        {
//            _next = next;

//        }

//        public async Task Invoke(HttpContext context)
//        {
//            if (context.Request.Path.StartsWithSegments("/api/Product") && context.Request.Method.StartsWith("GET"))
//            {
//                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

//                _Counter++;


//                if (DateTime.Now.Subtract(_LastRequestDate).Seconds > 10)
//                {
//                    _Counter = 1;
//                    _LastRequestDate = DateTime.Now;
//                    await _next(context);
//                } else
//                {
//                    if (_Counter > 5) 
//                    {
//                        _LastRequestDate = DateTime.Now;
//                        context.Response.StatusCode = 429;
//                        await context.Response.WriteAsync(" Rate Limit Exceeds");
//                    }
//                    else
//                    {
//                        _LastRequestDate = DateTime.Now;
//                        await _next(context);


//                    }
//                }

//            }

//        }


//        }
//    }

using System.Collections.Concurrent;
using System.Security.Claims;

namespace E_Commerce.Midlleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;

        // userId -> list of request timestamps
        private static readonly ConcurrentDictionary<string, List<DateTime>> _userRequests = new();

        private const int LIMIT = 5; // عدد الطلبات المسموح بها
        private const int WINDOW_SECONDS = 10; // خلال هذه المدة الزمنية

        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // نطبق فقط على إنشاء الطلبات
            if (context.Request.Path.StartsWithSegments("/api/Order", StringComparison.OrdinalIgnoreCase) &&
                context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";

                var now = DateTime.UtcNow;

                var userRequestTimes = _userRequests.GetOrAdd(userId, _ => new List<DateTime>());

                lock (userRequestTimes)
                {
                    // تنظيف الطلبات القديمة
                    userRequestTimes.RemoveAll(t => (now - t).TotalSeconds > WINDOW_SECONDS);

                    if (userRequestTimes.Count >= LIMIT)
                    {
                        context.Response.StatusCode = 429; // Too Many Requests
                        context.Response.ContentType = "text/plain";
                        context.Response.Headers["Retry-After"] = WINDOW_SECONDS.ToString();
                        context.Response.Headers["X-RateLimit-Limit"] = LIMIT.ToString();
                        context.Response.Headers["X-RateLimit-Remaining"] = "0";
                        context.Response.Headers["X-RateLimit-Reset"] = (now.AddSeconds(WINDOW_SECONDS)).ToString("o");
                         context.Response.WriteAsync("Rate limit exceeded. Try again later.");
                        return;
                    }

                    // أضف الطلب الحالي
                    userRequestTimes.Add(now);
                }
            }

            // مرر للـ middleware التالي
            await _next(context);
        }
    }
}
