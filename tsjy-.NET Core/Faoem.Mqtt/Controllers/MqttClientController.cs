using System.ComponentModel;
using Faoem.Mqtt.Dtos;
using Faoem.Mqtt.Inputs;
using Faoem.Mqtt.Services.MqttClient;
using Microsoft.AspNetCore.Mvc;

namespace Faoem.Mqtt.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MqttClientController(IMqttClientService mqttClientService)
    : ControllerBase
{
    [HttpGet("Status")]
    [Description("获取 mqtt 客户端状态")]
    public async Task<ActionResult<MqttClientStatusDto>> GetClientStatusAsync()
    {
        return await mqttClientService.GetMqttClientStatusAsync();
    }

    [HttpPost("Subscribe")]
    [Description("订阅主题")]
    public async Task<IActionResult> SubscribeAsync(MqttInputs input)
    {
        if (string.IsNullOrEmpty(input.Topic))
        {
            return BadRequest();
        }

        await mqttClientService.SubscribeAsync(input.Topic, input.Qos ?? 0);
        return Ok();
    }

    [HttpPost("Unsubscribe")]
    [Description("取消订阅主题")]
    public async Task<IActionResult> UnsubscribeAsync(MqttInputs input)
    {
        if (string.IsNullOrEmpty(input.Topic))
        {
            return BadRequest();
        }

        await mqttClientService.UnsubscribeAsyncAsync(input.Topic);
        return Ok();
    }

    [HttpGet("Subscriptions")]
    [Description("获取所有订阅的主题")]
    public async Task<ActionResult<List<string>>> GetSubscriptionsAsync()
    {
        return await mqttClientService.GetSubscribedTopicsAsync();
    }

    [HttpPost("GetMessages")]
    [Description("获取 mqtt 消息")]
    public async Task<ActionResult<List<MqttMessageDto>>> GetMessagesAsync(MqttInputs input)
    {
        if (string.IsNullOrEmpty(input.Topic))
        {
            return BadRequest();
        }

        return await mqttClientService.GetMessagesAsync(input.Topic);
    }
}