import { http } from '#/common/apis/http'

export const getMaintainPlanApi = (partId) => {
  return http.get(`/api/MaintainPlan/${partId}`)
}
export const addMaintainPlanApi = (plan) => {
  return http.post('/api/MaintainPlan', plan)
}
export const getMaintainPlansApi = (input) => {
  return http.post('/api/MaintainPlan/Get', input)
}
export const deleteMaintainPlanApi = (planId) => {
  return http.delete(`/api/MaintainPlan/${planId}`)
}
