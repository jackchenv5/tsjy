import { http } from '#/common/apis/http'

export const getStatusRecordApi = (query) => {
  return http.post('/api/FacilityStatus/GetStatus', query)
}
export const getAllStatusApi = (query) => {
  return http.post('/api/FacilityStatus/GetAllStatus', query)
}
export const getCurrentShiftStatusApi = (facilityId) => {
  return http.get('/api/FacilityStatus/GetCurrentShiftStatus', {
    params: { facilityId },
  })
}
