﻿using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Services.MassTransit;
using MassTransit;

namespace CamAI.EdgeBox.Services.AI;

[Publisher(Constants.HumanCount)]
[MessageUrn("HumanCountModel")]
public class HumanCountModel
{
    public DateTime Time { get; set; }
    public int Total { get; set; }
    public Guid ShopId { get; set; }
}

public static class ActionType
{
    public const string Working = "working";
    public const string Phone = "phone";
}
