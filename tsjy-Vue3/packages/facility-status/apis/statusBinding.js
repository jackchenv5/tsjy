import { http } from '#/common/apis/http'

export const getStatusBindingsApi = (facilityId) => {
  return http.get(`/api/StatusBinding/${facilityId}`)
}
export const updateStatusBindingsApi = (binding) => {
  return http.put(`/api/StatusBinding`, binding)
}
