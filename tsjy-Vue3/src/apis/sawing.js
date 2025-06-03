import { http } from '#/common/apis/http'

export const getRequestAllVariableApi = () => {
  return http.get('/api/Sawing/RequestAllVariable')
}
