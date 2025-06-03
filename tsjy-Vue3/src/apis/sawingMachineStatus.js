import { http } from '#/common/apis/http'

export const getStatusBindingsApi = (facilityId) => {
  return http.get(`/api/SawingMachineStatus/Binding/${facilityId}`)
}
export const updateStatusBindingsApi = (binding) => {
  return http.put(`/api/SawingMachineStatus/Binding`, binding)
}
export const getSawingMachineStatusApi = (facility) => {
  return http.get(`/api/SawingMachineStatus/${facility}`)
}
