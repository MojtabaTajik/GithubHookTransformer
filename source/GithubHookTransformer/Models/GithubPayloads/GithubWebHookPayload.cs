using System;
using System.Collections.Generic;

namespace GithubHookTransformer.Models.GithubPayloads;

public class GithubWebHookPayload
{
    public string zen { get; set; }
    public int hook_id { get; set; }
    public Hook hook { get; set; }
    public Repository repository { get; set; }
    public Sender sender { get; set; }
}

public class Config
{
    public string content_type { get; set; }
    public string insecure_ssl { get; set; }
    public string url { get; set; }
}

public class Hook
{
    public string type { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public bool active { get; set; }
    public List<string> events { get; set; }
    public Config config { get; set; }
    public DateTime updated_at { get; set; }
    public DateTime created_at { get; set; }
    public string url { get; set; }
    public string test_url { get; set; }
    public string ping_url { get; set; }
    public string deliveries_url { get; set; }
    public LastResponse last_response { get; set; }
}

public class LastResponse
{
    public object code { get; set; }
    public string status { get; set; }
    public object message { get; set; }
}