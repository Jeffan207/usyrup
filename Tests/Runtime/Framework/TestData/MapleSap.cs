using System;

/// <summary>
///     Stolen from Canada
/// </summary>
public class MapleSap {
    public readonly string id;

    public MapleSap() => id = Guid.NewGuid().ToString();
}
