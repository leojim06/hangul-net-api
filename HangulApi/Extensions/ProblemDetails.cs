﻿namespace HangulApi.Extensions;

public class ProblemDetails
{
    public string Type { get; set; } = "about:blank";
    public string Title { get; set; } = string.Empty;
    public int? Status { get; set; }
    public string? Detail { get; set; }
    public string? Instance { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
}
