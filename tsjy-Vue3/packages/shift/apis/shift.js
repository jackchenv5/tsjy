import { http } from '#/common/apis/http'

export const getShiftApi = () => {
  return http.get('/api/Shift')
}
export const updateShiftApi = (shifts) => {
  return http.put('/api/Shift', shifts)
}
export const getCurrentShiftApi = () => {
  return http.get('/api/Shift/Current')
}
