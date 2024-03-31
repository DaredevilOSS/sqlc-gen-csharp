namespace SqlcGenCsharp;

public static class Memoizer
{
    /// <summary>
    ///     Memoizes provided function. Function should provide deterministic results.
    ///     For the same input it should return the same result.
    ///     Memoized function for the specific input will be called once, further calls will use cache.
    /// </summary>
    /// <param name="func">function to be memoized</param>
    /// <typeparam name="TInput">Type of the function input value</typeparam>
    /// <typeparam name="TResult">Type of the function result</typeparam>
    /// <returns></returns>
    public static Func<TInput, TResult> Memoize<TInput, TResult>(this Func<TInput, TResult> func)
        where TInput : notnull
    {
        // create cache ("memo")
        var memo = new Dictionary<TInput, TResult>();

        // wrap provided function with cache handling
        return input =>
        {
            // check if result for set input was already cached
            if (memo.TryGetValue(input, out var fromMemo))
                // if yes, return value
                return fromMemo;

            // if no, call function
            var result = func(input);

            // cache the result
            memo.Add(input, result);

            // return result
            return result;
        };
    }
}