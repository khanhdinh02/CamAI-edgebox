﻿using CamAI.EdgeBox.MassTransit;

namespace CamAI.EdgeBox.Consumers;

[Publisher("directExchange")]
public class TestMessage : DirectMessage;