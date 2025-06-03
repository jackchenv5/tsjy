import { http } from '#/common/apis/http'

export const addMaintainHistoryApi = (input) => {
  return http.post(`/api/MaintainHistory`, input)
}
export const getMaintainHistoriesApi = (input) => {
  return http.post('/api/MaintainHistory/Get', input)
}
