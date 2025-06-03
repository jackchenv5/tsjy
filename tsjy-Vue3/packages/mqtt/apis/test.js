import { http } from '#/common/apis/http.js'

export const getMqttClientStatusApi = () => {
  return http.get('/api/MqttClient/Status')
}

export const subscribeApi = (topic, qos = 0) => {
  return http.post('/api/MqttClient/Subscribe', { topic, qos })
}
export const unsubscribeApi = (topic) => {
  return http.post('/api/MqttClient/Unsubscribe', { topic })
}

export const getSubscriptionsApi = () => {
  return http.get('/api/MqttClient/Subscriptions')
}

export const getMessagesApi = (topic) => {
  return http.post('/api/MqttClient/GetMessages', { topic })
}
