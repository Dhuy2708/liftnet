using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

public class UpdateFeedRequest
{
    public string? Content { get; set; }
    public List<IFormFile>? MediaFiles { get; set; }
} 