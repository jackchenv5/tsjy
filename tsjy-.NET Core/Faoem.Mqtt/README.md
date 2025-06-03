# Faoem.Mqtt

## 项目简介

Faoem.Mqtt 是一个基于 .NET 8.0 的 MQTT 客户端项目，提供了对 MQTT 协议的支持，并通过 RESTful API 进行管理和操作。

## 功能

- 获取 MQTT 客户端状态
- 订阅主题
- 取消订阅主题
- 获取已订阅的主题列表
- 获取指定主题的消息

## API 说明

`GET /api/MqttClient/Status` 获取客户端状态

`POST /api/MqttClient/Subscribe` 订阅主题

`POST /api/MqttClient/Unsubscribe` 取消订阅主题

`GET /api/MqttClient/Subscriptions` 获取已订阅的主题列表

`POST /api/MqttClient/GetMessages` 获取指定主题的消息

## IMqttClientService

`IMqttClientService` 接口定义了用于管理 MQTT 客户端的服务方法。以下是该接口提供的功能说明：

### 方法

#### `Task<MqttClientStatusDto> GetMqttClientStatusAsync()`

获取 MQTT 客户端的当前状态。

- **返回值**: `Task<MqttClientStatusDto>` - 包含客户端状态信息的任务。

#### `Task SubscribeAsync(string topic, MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtMostOnce)`

订阅指定的主题。

- **参数**:
    - `topic` (`string`): 要订阅的主题。
    - `qos` (`MqttQualityOfServiceLevel`, 可选): 服务质量级别，默认为 `MqttQualityOfServiceLevel.AtMostOnce`。

- **返回值**: `Task` - 表示异步操作的任务。

#### `Task UnsubscribeAsync(string topic)`

取消订阅指定的主题。

- **参数**:
    - `topic` (`string`): 要取消订阅的主题。

- **返回值**: `Task` - 表示异步操作的任务。

#### `Task<List<string>> GetSubscribedTopicsAsync()`

获取已订阅的主题列表。

- **返回值**: `Task<List<string>>` - 包含已订阅主题列表的任务。

#### `Task<List<MqttMessageDto>> GetMessagesAsync(string topic)`

获取指定主题的消息。

- **参数**:
    - `topic` (`string`): 要获取消息的主题。

- **返回值**: `Task<List<MqttMessageDto>>` - 包含指定主题消息列表的任务。




