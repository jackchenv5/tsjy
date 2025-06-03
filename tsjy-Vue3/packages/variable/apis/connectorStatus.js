import { http } from '#/common/apis/http.js'

export const getConnectorStatusApi = () => {
  return http.get('/api/Variable/ConnectorStatus')
}
