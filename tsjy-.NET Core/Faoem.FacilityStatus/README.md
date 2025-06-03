# Faoem.Facility

## 项目简介

处理设备状态数据。

## 变量需求

需要 4 个布尔类型的变量：

- Running: 设备是否正在运行
- Standby: 设备是否处于待机状态
- Stopped: 设备是否处于停机状态
- Error: 设备是否处于故障状态

变量为 true 时表示设备处于对应状态。如果同时有多个变量为 true，则优先级为 Error > Stopped > Standby > Running.
如果变量均不为 true，则认为状态无效（Invalid）


