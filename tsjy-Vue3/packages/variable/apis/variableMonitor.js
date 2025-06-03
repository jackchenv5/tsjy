import { http } from '#/common/apis/http.js'

export const getDefinitionsApi = (filter) => {
  return http.post('/api/Variable/GetVariables', filter)
}
