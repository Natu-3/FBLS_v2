using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// "Middleware" for doing rate limited queries
public static class RateLimitUtility<T>
    where T : Enum
{
    static Dictionary<T, TimeSpan> s_RateLimits = null;
    static Dictionary<T, DateTime> s_LastApiCallTime = new Dictionary<T, DateTime>();
    static readonly TimeSpan k_AdditionalDelay = TimeSpan.FromMilliseconds(100);

    public static void SetRateLimits(Dictionary<T, TimeSpan> rateLimits)
    {
        s_RateLimits = rateLimits;
    }

    static async Task WaitForApiRateLimit(T api)
    {
        if (s_RateLimits == null)
        {
            throw new InvalidOperationException("Call 'SetRateLimits' to initialize the Rate Limits data before calling this function!");
        }

        if (s_LastApiCallTime.TryGetValue(api, out var lastCallTime))
        {
            var timeSinceLastCall = DateTime.UtcNow - lastCallTime;
            var timeUntilRateLimitExpires = s_RateLimits[api] - timeSinceLastCall + k_AdditionalDelay; // add a safety delay

            if (timeUntilRateLimitExpires > TimeSpan.Zero)
            {
                Debug.Log($"Calling {typeof(T).Name} {api} API but need to wait to avoid rate limit; waiting an additional {timeUntilRateLimitExpires.TotalMilliseconds} ms");
                await Task.Delay(timeUntilRateLimitExpires);
            }
        }
    }

    /// <summary>
    ///     Call an API while avoiding the rate limit.
    /// </summary>
    /// <typeparam name="T">The type returned by the call</typeparam>
    /// <param name="api">The API being called</param>
    /// <param name="asyncApiCall">A func that provides an async task to execute the API call</param>
    public static async Task<TReturn> CallApiWithRateLimit<TReturn>(T api, Func<Task<TReturn>> asyncApiCall)
    {
        try
        {
            await WaitForApiRateLimit(api);
            return await asyncApiCall.Invoke();
        }
        finally
        {
            s_LastApiCallTime[api] = DateTime.UtcNow;
        }
    }

    /// <summary>
    ///     Call an API while avoiding the rate limit.
    /// </summary>
    /// <param name="api">The API being called</param>
    /// <param name="asyncApiCall">A func that provides an async task to execute the API call</param>
    public static async Task CallApiWithRateLimit(T api, Func<Task> asyncApiCall)
    {
        try
        {
            await WaitForApiRateLimit(api);
            await asyncApiCall.Invoke();
        }
        finally
        {
            s_LastApiCallTime[api] = DateTime.UtcNow;
        }
    }
}
